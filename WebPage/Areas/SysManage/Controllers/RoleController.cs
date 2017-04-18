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
    public class RoleController : BaseController
    {
        IRoleManage RoleManage { get; set; }

        IUserRoleManage UserRoleManage { get; set; }

        IRolePermissionManage RolePermissionManage { get; set; }

        ISystemManage SystemManage { get; set; }
        /// <summary>
        /// 載入主頁
        /// </summary>
        /// <returns></returns>
        [UserAuthorizeAttribute(ModuleAlias = "Role", OperaAction = "View")]
        public ActionResult Index()
        {
            try
            {
                #region 處理查詢參數

                //系統ID
                string System = Request.QueryString["System"];
                ViewData["System"] = System;

                //搜索的關鍵字（用於輸出給前臺的Input顯示）
                ViewBag.Search = base.keywords;
                #endregion

                //輸出使用者所擁有的系統清單到視圖頁
                ViewData["Systemlist"] = this.SystemManage.LoadSystemInfo(CurrentUser.System_Id);

                //輸出分頁查詢列表
                return View(BindList(System));
            }
            catch (Exception e)
            {
                WriteLog(Common.Enums.enumOperator.Select, "載入角色列表：", e);
                throw e.InnerException;
            }
        }
        /// <summary>
        /// 載入詳情
        /// </summary>
        /// <returns></returns>
        [UserAuthorizeAttribute(ModuleAlias = "Role", OperaAction = "Detail")]
        public ActionResult Detail(int? id)
        {
            var _entity = new Domain.SYS_ROLE() { ISCUSTOM = 0 };

            if (id != null && id > 0)
            {
                _entity = RoleManage.Get(p => p.ID == id);
            }
            else
            {
                if (!string.IsNullOrEmpty(Request.QueryString["systemId"]))
                {
                    _entity.FK_BELONGSYSTEM = Request.QueryString["systemId"];
                }
            }

            ViewData["Systemlist"] = this.SystemManage.LoadSystemInfo(CurrentUser.System_Id);

            return View(_entity);
        }
        /// <summary>
        /// 保存角色
        /// </summary>
        [UserAuthorizeAttribute(ModuleAlias = "Role", OperaAction = "Add,Edit")]
        public ActionResult Save(Domain.SYS_ROLE entity)
        {
            bool isEdit = false;
            var json = new JsonHelper() { Msg = "保存成功", Status = "n" };
            try
            {
                if (entity != null)
                {
                    //判斷角色名是否漢字
                    if (System.Text.RegularExpressions.Regex.IsMatch(entity.ROLENAME.Trim(), "^[\u4e00-\u9fa5]+$"))
                    {
                        if (entity.ROLENAME.Length > 36)
                        {
                            json.Msg = "角色名稱最多只能能包含36個漢字";
                            return Json(json);
                        }

                        //添加
                        if (entity.ID <= 0)
                        {
                            entity.CREATEDATE = DateTime.Now;
                            entity.CREATEPERID = this.CurrentUser.Name;
                            entity.UPDATEDATE = DateTime.Now;
                            entity.UPDATEUSER = this.CurrentUser.Name;
                        }
                        else //修改
                        {
                            entity.UPDATEDATE = DateTime.Now;
                            entity.UPDATEUSER = this.CurrentUser.Name;
                            isEdit = true;
                        }
                        //判斷角色是否重名 
                        if (!this.RoleManage.IsExist(p => p.ROLENAME == entity.ROLENAME && p.ID != entity.ID))
                        {
                            if (isEdit)
                            {
                                //系統更換 刪除所有權限
                                var _entity = RoleManage.Get(p => p.ID == entity.ID);
                                if (_entity.FK_BELONGSYSTEM != entity.FK_BELONGSYSTEM)
                                {
                                    RolePermissionManage.Delete(p => p.ROLEID == _entity.ID);
                                }
                            }
                            if (RoleManage.SaveOrUpdate(entity, isEdit))
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
                            json.Msg = "角色名" + entity.ROLENAME + "已被使用，請修改角色名稱再提交";
                        }

                    }
                    else
                    {
                        json.Msg = "角色名稱只能包含漢字";
                    }

                }
                else
                {
                    json.Msg = "未找到需要保存的角色資訊";
                }
                if (isEdit)
                {
                    WriteLog(Common.Enums.enumOperator.Edit, "修改用戶角色，結果：" + json.Msg, Common.Enums.enumLog4net.INFO);
                }
                else
                {
                    WriteLog(Common.Enums.enumOperator.Add, "添加用戶角色，結果：" + json.Msg, Common.Enums.enumLog4net.INFO);
                }
            }
            catch (Exception e)
            {
                json.Msg = "保存使用者角色發生內部錯誤！";
                WriteLog(Common.Enums.enumOperator.None, "保存用戶角色：", e);
            }
            return Json(json);
        }
        /// <summary>
        /// 刪除角色
        /// </summary>
        [UserAuthorizeAttribute(ModuleAlias = "Role", OperaAction = "Remove")]
        public ActionResult Delete(string idList)
        {
            var json = new JsonHelper() { Msg = "刪除角色完畢", Status = "n" };
            var id = idList.Trim(',').Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries).Select(p => int.Parse(p)).ToList();
            if (id.Contains(Common.Enums.ClsDic.DicRole["超級管理員"]))
            {
                json.Msg = "刪除失敗，不能刪除系統固有角色!";
                WriteLog(Common.Enums.enumOperator.Remove, "刪除用戶角色：" + json.Msg, Common.Enums.enumLog4net.ERROR);
                return Json(json);
            }
            if (this.UserRoleManage.IsExist(p => id.Contains(p.FK_ROLEID)))
            {
                json.Msg = "刪除失敗，不能刪除系統中正在使用的角色!";
                WriteLog(Common.Enums.enumOperator.Remove, "刪除用戶角色：" + json.Msg, Common.Enums.enumLog4net.ERROR);
                return Json(json);
            }
            try
            {
                //1、刪除角色許可權
                RolePermissionManage.Delete(p => id.Contains(p.ROLEID));
                //2、刪除角色
                RoleManage.Delete(p => id.Contains(p.ID));
                json.Status = "y";
                WriteLog(Common.Enums.enumOperator.Remove, "刪除用戶角色：" + json.Msg, Common.Enums.enumLog4net.WARN);
            }
            catch (Exception e)
            {
                json.Msg = "刪除使用者角色發生內部錯誤！";
                WriteLog(Common.Enums.enumOperator.Remove, "刪除用戶角色：", e);
            }
            return Json(json);
        }


        /// <summary>
        /// 用戶角色分配
        /// </summary>
        /// <param name="id">用戶ID</param>
        /// <returns></returns>
        [UserAuthorizeAttribute(ModuleAlias = "User", OperaAction = "AllocationRole")]
        public ActionResult RoleCall(int? id)
        {
            try
            {
                if (id != null && id > 0)
                {
                    //用戶ID
                    ViewData["userId"] = id;
                    //獲取使用者角色資訊
                    var userRoleList = this.UserRoleManage.LoadAll(p => p.FK_USERID == id).Select(p => p.FK_ROLEID).ToList();
                    return View(JsonConverter.JsonClass(this.RoleManage.LoadAll(p => this.CurrentUser.System_Id.Any(e => e == p.FK_BELONGSYSTEM)).OrderBy(p => p.FK_BELONGSYSTEM).OrderByDescending(p => p.ID).ToList().Select(p => new { p.ID, p.ROLENAME, ISCUSTOMSTATUS = p.ISCUSTOM == 1 ? "<i class=\"fa fa-circle text-navy\"></i>" : "<i class=\"fa fa-circle text-danger\"></i>", SYSNAME = SystemManage.Get(m => m.ID == p.FK_BELONGSYSTEM).NAME, IsChoosed = userRoleList.Contains(p.ID) })));
                }
                else
                {
                    return View();
                }

            }
            catch (Exception e)
            {
                WriteLog(Common.Enums.enumOperator.Select, "獲取用戶分配的角色：", e);
                throw e.InnerException;
            }
        }

        /// <summary>
        /// 設置用戶角色
        /// </summary>
        [UserAuthorizeAttribute(ModuleAlias = "Role", OperaAction = "Allocation")]
        public ActionResult UserRole()
        {
            var json = new JsonHelper()
            {
                Msg = "設置用戶角色成功",
                Status = "n"
            };
            string userId = Request.Form["UserId"];
            string roleId = Request.Form["checkbox_name"] ?? "";
            if (string.IsNullOrEmpty(userId))
            {
                json.Msg = "未找到要分配角色用戶";
                return Json(json);
            }
            roleId = roleId.TrimEnd(',');

            try
            {
                //設置用戶角色
                this.UserRoleManage.SetUserRole(int.Parse(userId), roleId);
                json.Status = "y";
                WriteLog(Common.Enums.enumOperator.Allocation, "設置用戶角色：" + json.Msg, Common.Enums.enumLog4net.INFO);
            }
            catch (Exception e)
            {
                json.Msg = "設置失敗，錯誤：" + e.Message;
                WriteLog(Common.Enums.enumOperator.Allocation, "設置用戶角色：", e);
            }
            return Json(json);
        }


        /// <summary>
        /// 分頁查詢角色列表
        /// </summary>
        private Common.PageInfo BindList(string system)
        {
            //基礎資料
            var query = this.RoleManage.LoadAll(null);
            //系統
            if (!string.IsNullOrEmpty(system))
            {
                int SuperAdminId = Common.Enums.ClsDic.DicRole["超級管理員"];
                query = query.Where(p => p.FK_BELONGSYSTEM == system || p.ISCUSTOM == 1);
            }
            else
            {
                query = query.Where(p => this.CurrentUser.System_Id.Any(e => e == p.FK_BELONGSYSTEM));
            }
            //查詢關鍵字
            if (!string.IsNullOrEmpty(keywords))
            {
                query = query.Where(p => p.ROLENAME.Contains(keywords));
            }
            //排序
            query = query.OrderByDescending(p => p.CREATEDATE);
            //分頁
            var result = this.RoleManage.Query(query, page, pagesize);

            var list = result.List.Select(p => new
            {
                //以下是視圖需要展示的內容，加動態可迴圈
                p.CREATEDATE,
                p.ROLENAME,
                p.ROLEDESC,
                USERNAME = p.CREATEPERID,
                p.ID,
                SYSNAME = SystemManage.Get(m => m.ID == p.FK_BELONGSYSTEM).NAME,
                ISCUSTOMSTATUS = p.ISCUSTOM == 1 ? "<i class=\"fa fa-circle text-navy\"></i>" : "<i class=\"fa fa-circle text-danger\"></i>"
            }).ToList();

            return new Common.PageInfo(result.Index, result.PageSize, result.Count, Common.JsonHelper.JsonConverter.JsonClass(list));
        }
    }
}