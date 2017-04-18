using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Common.JsonHelper;
using Service.IService;
using WebPage.Controllers;

namespace WebPage.Areas.SysManage.Controllers
{
    public class PostController : BaseController
    {
        #region 聲明容器
        /// <summary>
        /// 職位
        /// </summary>
        IPostManage PostManage { get; set; }
        /// <summary>
        /// 部門
        /// </summary>
        IDepartmentManage DepartmentManage { get; set; }
        /// <summary>
        /// 字典編碼
        /// </summary>
        ICodeManage CodeManage { get; set; }
        /// <summary>
        /// 職位人員
        /// </summary>
        IPostUserManage PostUserManage { get; set; }
        #endregion

        [UserAuthorize(ModuleAlias = "Post", OperaAction = "View")]
        public ActionResult Home()
        {
            return base.View();
        }
        /// <summary>
        /// 職位管理 職位列表
        /// </summary>
        /// <returns></returns>
        [UserAuthorizeAttribute(ModuleAlias = "Post", OperaAction = "View")]
        public ActionResult Index()
        {
            try
            {
                #region 處理查詢參數
                //獲取部門ID
                var departId = Request.QueryString["departId"] ?? (Request.Form["departId"] ?? "");
                //職位類型
                string posttype = Request.QueryString["posttype"];

                ViewBag.Search = base.keywords;
                #endregion

                //如果部門ID不為空或NULL
                if (!string.IsNullOrEmpty(departId))
                {
                    //部門資訊
                    var department = this.DepartmentManage.Get(p => p.ID == departId);

                    ViewBag.Department = department;

                    ViewData["post"] = posttype;

                    ViewData["PostType"] = this.CodeManage.GetCode("POSTTYPE");

                    return View(BindList(posttype, departId));
                }

                return View();
            }
            catch (Exception e)
            {
                WriteLog(Common.Enums.enumOperator.Select, "對模組許可權按鈕的管理載入主頁：", e);
                throw e.InnerException;
            }
        }

        /// <summary>
        /// 分頁查詢職位列表
        /// </summary>
        private Common.PageInfo BindList(string posttype, string departId)
        {
            //基礎資料
            var query = this.PostManage.LoadAll(null);
            //職位類型
            if (!string.IsNullOrEmpty(posttype))
            {
                query = query.Where(p => p.POSTTYPE == posttype && p.FK_DEPARTID == departId);
            }
            else
            {
                query = query.Where(p => p.FK_DEPARTID == departId);
            }
            //查詢關鍵字
            if (!string.IsNullOrEmpty(keywords))
            {
                query = query.Where(p => p.POSTNAME.Contains(keywords));
            }
            //排序
            query = query.OrderBy(p => p.SHOWORDER);
            //分頁
            var result = this.PostManage.Query(query, page, pagesize);

            var list = result.List.Select(p => new
            {
                p.ID,
                p.POSTNAME,
                POSTTYPE = this.CodeManage.GetCode("POSTTYPE", p.POSTTYPE).First().NAMETEXT,
                p.CREATEDATE,
                USERCOUNT = PostUserManage.LoadAll(m => m.FK_POSTID == p.ID).Count()

            }).ToList();

            return new Common.PageInfo(result.Index, result.PageSize, result.Count, JsonConverter.JsonClass(list));
        }

        /// <summary>
        /// 載入詳情
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [UserAuthorizeAttribute(ModuleAlias = "Post", OperaAction = "Detail")]
        public ActionResult Detail(string id)
        {
            try
            {
                //職位類型
                ViewData["PostType"] = this.CodeManage.GetCode("POSTTYPE");
                //獲取部門ID
                var departId = Request.QueryString["departId"];

                var _entity = this.PostManage.Get(p => p.ID == id) ?? new Domain.SYS_POST() { FK_DEPARTID = departId };

                return View(_entity);
            }
            catch (Exception e)
            {
                WriteLog(Common.Enums.enumOperator.Select, "職位管理載入詳情：", e);
                throw e.InnerException;
            }
        }

        /// <summary>
        /// 保存職位
        /// </summary>
        [UserAuthorizeAttribute(ModuleAlias = "Post", OperaAction = "Add,Edit")]
        public ActionResult Save(Domain.SYS_POST entity)
        {

            bool isEdit = false;
            var json = new JsonHelper() { Msg = "保存職位成功", Status = "n", ReUrl = "/Post/Index" };
            try
            {
                if (entity != null)
                {
                    //添加
                    if (string.IsNullOrEmpty(entity.ID))
                    {
                        entity.ID = Guid.NewGuid().ToString();
                        entity.CREATEDATE = DateTime.Now;
                        entity.CREATEUSER = CurrentUser.Name;
                        entity.UPDATEDATE = DateTime.Now;
                        entity.UPDATEUSER = this.CurrentUser.Name;
                    }
                    else //修改
                    {
                        entity.UPDATEDATE = DateTime.Now;
                        entity.UPDATEUSER = this.CurrentUser.Name;
                        isEdit = true;
                    }
                    //判斷職位是否重名 
                    if (!this.PostManage.IsExist(p => p.POSTNAME == entity.POSTNAME && p.ID != entity.ID))
                    {
                        if (PostManage.SaveOrUpdate(entity, isEdit))
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
                        json.Msg = "職位" + entity.POSTNAME + "已存在，不能重複添加";
                    }
                }
                else
                {
                    json.Msg = "未找到需要保存的職位";
                }
                if (isEdit)
                {
                    WriteLog(Common.Enums.enumOperator.Edit, "修改職位，結果：" + json.Msg, Common.Enums.enumLog4net.INFO);
                }
                else
                {
                    WriteLog(Common.Enums.enumOperator.Add, "添加職位，結果：" + json.Msg, Common.Enums.enumLog4net.INFO);
                }
            }
            catch (Exception e)
            {
                json.Msg = "保存職位發生內部錯誤！";
                WriteLog(Common.Enums.enumOperator.None, "保存職位：", e);
            }
            return Json(json);


        }

        /// <summary>
        /// 刪除職位
        /// </summary>
        [UserAuthorizeAttribute(ModuleAlias = "Post", OperaAction = "Remove")]
        public ActionResult Delete(string idList)
        {
            JsonHelper json = new JsonHelper() { Msg = "刪除職位完畢", Status = "n" };
            try
            {
                if (!string.IsNullOrEmpty(idList))
                {
                    idList = idList.Trim(',');
                    //判斷職位是否分配人員
                    if (!this.PostUserManage.IsExist(p => idList.Contains(p.FK_POSTID)))
                    {
                        this.PostManage.Delete(p => idList.Contains(p.ID));
                        json.Status = "y";
                    }
                    else
                    {
                        json.Msg = "該職位已經分配人員，不能刪除";
                    }
                }
                else
                {
                    json.Msg = "未找到要刪除的職位記錄";
                }
                WriteLog(Common.Enums.enumOperator.Remove, "刪除職位，結果：" + json.Msg, Common.Enums.enumLog4net.WARN);
            }
            catch (Exception e)
            {
                json.Msg = "刪除職位發生內部錯誤！";
                WriteLog(Common.Enums.enumOperator.Remove, "刪除職位：", e);
            }
            return Json(json);
        }

        /// <summary>
        /// 獲取職位列表
        /// </summary>
        /// <returns></returns>
        public ActionResult GetPostByDepart()
        {
            var departId = Request.Form["departId"];
            if (!string.IsNullOrEmpty(departId))
            {
                return Json(this.PostManage.LoadAll(p => p.FK_DEPARTID == departId)
                .Select(p => new
                {
                    ID = p.ID,
                    NAME = p.POSTNAME
                }).ToList(), JsonRequestBehavior.AllowGet);
            }
            return new EmptyResult();
        }
    }
}