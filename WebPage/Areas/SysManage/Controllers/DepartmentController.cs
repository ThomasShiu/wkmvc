using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Common;
using Common.JsonHelper;
using Service.IService;
using WebPage.Controllers;

namespace WebPage.Areas.SysManage.Controllers
{
    public class DepartmentController : BaseController
    {
        IDepartmentManage DepartmentManage { get; set; }

        /// <summary>
        /// 職位管理
        /// </summary>
        IPostManage PostManage { get; set; }

        /// <summary>
        /// 載入主頁
        /// </summary>
        [UserAuthorizeAttribute(ModuleAlias = "Department", OperaAction = "View")]
        public ActionResult Index()
        {
            try
            {
                ViewBag.Search = base.keywords;
                return View(BindList());
            }
            catch (Exception e)
            {
                WriteLog(Common.Enums.enumOperator.Select, "部門管理載入主頁：", e);
                throw e.InnerException;
            }
        }

        /// <summary>
        /// 載入詳情頁
        /// </summary>
        [UserAuthorizeAttribute(ModuleAlias = "Department", OperaAction = "Detail")]
        public ActionResult Detail(string id)
        {
            try
            {
                var entity = new Domain.SYS_DEPARTMENT();

                ViewBag.moduleparent = this.DepartmentManage.GetDepartmentByDetail();

                //添加子部門
                string parentId = Request.QueryString["parentId"];

                if (!string.IsNullOrEmpty(parentId))
                {
                    entity.PARENTID = parentId;
                }
                if (!string.IsNullOrEmpty(id))
                {
                    entity = this.DepartmentManage.Get(p => p.ID == id);
                }
                return View(entity);
            }
            catch (Exception e)
            {
                WriteLog(Common.Enums.enumOperator.Select, "部門管理載入詳情頁：", e);
                throw e.InnerException;
            }
        }


        /// <summary>
        /// 保存部門
        /// </summary>
        [ValidateInput(false)]
        [UserAuthorizeAttribute(ModuleAlias = "Department", OperaAction = "Add,Edit")]
        public ActionResult Save(Domain.SYS_DEPARTMENT entity)
        {
            bool isEdit = false;
            var json = new JsonHelper() { Msg = "保存成功", Status = "n" };
            try
            {
                var _entity = new Domain.SYS_DEPARTMENT();
                if (entity != null)
                {
                    if (!string.IsNullOrEmpty(entity.ID))
                    {
                        #region 修改
                        _entity = this.DepartmentManage.Get(p => p.ID == entity.ID);
                        entity.CREATEDATE = _entity.CREATEDATE;
                        entity.CREATEPERID = _entity.CREATEPERID;
                        entity.UPDATEDATE = DateTime.Now;
                        entity.UPDATEUSER = this.CurrentUser.Name;
                        if (entity.PARENTID != _entity.PARENTID)
                        {
                            entity.CODE = this.DepartmentManage.CreateCode(entity.PARENTID);
                        }
                        else
                        {
                            entity.CODE = _entity.CODE;
                        }
                        //獲取父級記錄
                        if (string.IsNullOrEmpty(_entity.PARENTID))
                        {
                            //業務等級
                            entity.BUSINESSLEVEL = 1;
                            entity.PARENTCODE = null;
                        }
                        else
                        {
                            var parententity = this.DepartmentManage.Get(p => p.ID == entity.PARENTID);
                            entity.BUSINESSLEVEL = parententity.BUSINESSLEVEL + 1;
                            entity.PARENTCODE = parententity.CODE;
                        }
                        #endregion
                        isEdit = true;
                        _entity = entity;
                    }
                    else
                    {
                        #region 添加
                        _entity = entity;
                        _entity.ID = Guid.NewGuid().ToString();
                        _entity.CREATEDATE = DateTime.Now;
                        _entity.CREATEPERID = this.CurrentUser.Name;
                        _entity.UPDATEDATE = DateTime.Now;
                        _entity.UPDATEUSER = this.CurrentUser.Name;
                        //根據上級部門的ID確定當前部門的CODE
                        _entity.CODE = this.DepartmentManage.CreateCode(entity.PARENTID);
                        //獲取父級記錄
                        if (string.IsNullOrEmpty(entity.PARENTID))
                        {
                            //業務等級
                            entity.BUSINESSLEVEL = 1;
                            entity.PARENTCODE = null;
                        }
                        else
                        {
                            var parententity = this.DepartmentManage.Get(p => p.ID == entity.PARENTID);
                            entity.BUSINESSLEVEL = parententity.BUSINESSLEVEL + 1;
                            entity.PARENTCODE = parententity.CODE;
                        }
                        #endregion
                    }
                    //判斷同一個部門下，是否重名 
                    var predicate = PredicateBuilder.True<Domain.SYS_DEPARTMENT>();
                    predicate = predicate.And(p => p.PARENTID == _entity.PARENTID);
                    predicate = predicate.And(p => p.NAME == _entity.NAME);
                    predicate = predicate.And(p => p.ID != _entity.ID);
                    if (!this.DepartmentManage.IsExist(predicate))
                    {
                        if (this.DepartmentManage.SaveOrUpdate(_entity, isEdit))
                        {
                            json.Status = "y";
                        }
                        else
                        {
                            json.Msg = "保存失敗";
                        }
                    }
                    else
                    {
                        json.Msg = "部門" + entity.NAME + "已存在，不能重複添加";
                    }
                }
                else
                {
                    json.Msg = "未找到需要保存的部門資訊";
                }
                if (isEdit)
                {
                    WriteLog(Common.Enums.enumOperator.Edit, "修改部門資訊，結果：" + json.Msg, Common.Enums.enumLog4net.INFO);
                }
                else
                {
                    WriteLog(Common.Enums.enumOperator.Add, "添加部門資訊，結果：" + json.Msg, Common.Enums.enumLog4net.INFO);
                }
            }
            catch (Exception e)
            {
                json.Msg = "保存部門資訊發生內部錯誤！";
                WriteLog(Common.Enums.enumOperator.None, "保存部門資訊：", e);
            }
            return Json(json);

        }


        /// <summary>
        /// 刪除部門
        /// </summary>
        [UserAuthorizeAttribute(ModuleAlias = "Department", OperaAction = "Remove")]
        public ActionResult Delete(string idList)
        {
            JsonHelper json = new JsonHelper() { Msg = "刪除部門成功", ReUrl = "/Department/Index", Status = "n" };
            try
            {
                if (!string.IsNullOrEmpty(idList))
                {
                    idList = idList.TrimEnd(',');
                    //判斷是否有下屬部門
                    if (!this.DepartmentManage.DepartmentIsExists(idList))
                    {
                        //判斷該部門是否有職位
                        if (!this.PostManage.IsExist(p => idList.Contains(p.FK_DEPARTID)))
                        {
                            var idList1 = idList.Split(',').ToList();
                            this.DepartmentManage.Delete(p => idList.Contains(p.ID));
                            json.Status = "y";
                        }
                        else
                        {
                            json.Msg = "該部門有職位資訊不能刪除";
                        }
                    }
                    else
                    {
                        json.Msg = "該部門有下屬部門不能刪除";
                    }
                }
                else
                {
                    json.Msg = "未找到要刪除的部門記錄";
                }
                WriteLog(Common.Enums.enumOperator.Remove, "刪除部門：" + json.Msg, Common.Enums.enumLog4net.WARN);
            }
            catch (Exception e)
            {
                json.Msg = "刪除部門發生內部錯誤！";
                WriteLog(Common.Enums.enumOperator.Remove, "刪除部門：", e);
            }
            return Json(json);
        }

        /// <summary>
        /// 獲取部門樹形功能表
        /// </summary>
        public ActionResult GetTree()
        {
            var json = new JsonHelper() { Msg = "Success", Status = "y" };

            try
            {
                //獲取部門列表 按照 SHOWORDER欄位 昇冪排列
                var query = this.DepartmentManage.LoadAll(null).OrderBy(p => p.SHOWORDER).ToList();
                var result = query.Select(m => new
                {
                    id = m.ID,
                    parent = !string.IsNullOrEmpty(m.PARENTID) ? m.PARENTID.ToString() : "#",
                    text = m.NAME,
                    icon = m.BUSINESSLEVEL == 0 ? "fa fa-circle text-warning" : "fa fa-circle text-navy"
                }).ToList();

                json.Data = result;
            }
            catch (Exception e)
            {
                json.Status = "n";
                json.Msg = "伺服器忙，請稍後再試！";
                WriteLog(Common.Enums.enumOperator.Select, "許可權管理，獲取模組樹：", e);
            }
            return Json(json);
        }


        /// <summary>
        /// 分頁查詢部門列表
        /// </summary>
        private object BindList()
        {
            //基礎資料
            var query = this.DepartmentManage.LoadAll(null);

            //遞迴排序（無分頁）
            var result = this.DepartmentManage.RecursiveDepartment(query.ToList())
                .Select(p => new
                {
                    p.ID,
                    p.NAME,
                    DepartName = DepartmentManage.GetDepartmentName(p.NAME, p.BUSINESSLEVEL),
                    p.BUSINESSLEVEL,
                    p.SHOWORDER,
                    p.CREATEDATE
                });

            //查詢關鍵字
            if (!string.IsNullOrEmpty(keywords))
            {
                result = result.Where(p => p.NAME.Contains(keywords));
            }

            return JsonConverter.JsonClass(result);
        }
    }
}
