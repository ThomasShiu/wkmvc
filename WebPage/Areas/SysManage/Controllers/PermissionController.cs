using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using Common.JsonHelper;
using Service.IService;
using WebPage.Controllers;

namespace WebPage.Areas.SysManage.Controllers
{
    public class PermissionController : BaseController
    {
        #region 聲明容器
        /// <summary>
        /// 系統管理
        /// </summary>
        ISystemManage SystemManage { get; set; }
        /// <summary>
        /// 許可權管理
        /// </summary>
        IPermissionManage PermissionManage { get; set; }
        /// <summary>
        /// 模組管理
        /// </summary>
        IModuleManage ModuleManage { get; set; }

        /// <summary>
        /// 預置編碼管理
        /// </summary>
        ICodeManage CodeManage { get; set; }

        /// <summary>
        /// 角色許可權管理
        /// </summary>
        IRolePermissionManage RolePermissionManage { get; set; }

        /// <summary>
        /// 用戶許可權管理
        /// </summary>
        IUserPermissionManage UserPermissionManage { get; set; }

        IRoleManage RoleManage { get; set; }
        #endregion

        /// <summary>
        /// 許可權管理 預設頁面
        /// </summary>
        /// <returns></returns>
        [UserAuthorizeAttribute(ModuleAlias = "Permission", OperaAction = "View")]
        public ActionResult Home()
        {
            try
            {
                //獲取使用者可操作的系統清單
                ViewData["Systemlist"] = this.SystemManage.LoadSystemInfo(CurrentUser.System_Id);
            }
            catch (Exception e)
            {
                WriteLog(Common.Enums.enumOperator.Select, "對模組許可權按鈕的管理載入導航頁：", e);
            }

            return View();
        }

        /// <summary>
        /// 許可權管理 許可權列表
        /// </summary>
        /// <returns></returns>
        [UserAuthorizeAttribute(ModuleAlias = "Permission", OperaAction = "View")]
        public ActionResult Index()
        {
            try
            {
                //獲取模組ID
                var moduleId = Request.QueryString["moduleId"] ?? (Request["moduleId"] ?? "");

                //如果模組ID不為空或NULL
                if (!string.IsNullOrEmpty(moduleId))
                {
                    //把模組ID轉為Int
                    int module_Id = int.Parse(moduleId);

                    //模組資訊
                    var module = this.ModuleManage.Get(p => p.ID == module_Id);

                    //繫結欄位表
                    var query = this.PermissionManage.LoadAll(p => p.MODULEID == module.ID);

                    //關鍵字查詢
                    if (!string.IsNullOrEmpty(keywords))
                    {
                        query = query.Where(p => p.NAME.Contains(keywords));
                    }
                    //輸出結果
                    var result = query.OrderBy(p => p.SHOWORDER).ToList();

                    ViewBag.Search = base.keywords;

                    ViewBag.Module = module;

                    return View(result);
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
        /// 載入許可權詳情
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [UserAuthorizeAttribute(ModuleAlias = "Permission", OperaAction = "Detail")]
        public ActionResult Detail(int? id)
        {
            try
            {
                var _entity = this.PermissionManage.Get(p => p.ID == id) ?? new Domain.SYS_PERMISSION();

                //獲取模組ID
                var moduleId = Request.QueryString["moduleId"];

                if (!string.IsNullOrEmpty(moduleId))
                {
                    int newmoduleid = int.Parse(moduleId);
                    _entity.MODULEID = newmoduleid;
                }
                ViewData["PresetValue"] = CodeManage.GetCode("ROLEVALUE");
                return View(_entity);
            }
            catch (Exception e)
            {
                WriteLog(Common.Enums.enumOperator.Select, "對模組許可權按鈕的管理載入詳情：", e);
                throw e.InnerException;
            }
        }
        /// <summary>
        /// 保存許可權
        /// </summary>
        [UserAuthorizeAttribute(ModuleAlias = "Permission", OperaAction = "Add,Edit")]
        public ActionResult Save(Domain.SYS_PERMISSION entity)
        {
            bool isEdit = false;
            JsonHelper json = new JsonHelper() { Msg = "保存許可權成功", Status = "n" };
            try
            {
                if (entity != null)
                {
                    if (System.Text.Encoding.GetEncoding("gb2312").GetBytes(entity.NAME.Trim()).Length > 50)
                    {
                        json.Msg = "許可權的名稱長度不能超過50個字元";
                        return Json(json);
                    }
                    entity.ICON = Request.Form["ICON"];
                    var nextpervalue = Request.Form["NEXTPERVALUE"];
                    if (!string.IsNullOrEmpty(nextpervalue))
                    {
                        if (!Regex.IsMatch(nextpervalue, @"^[A-Za-z0-9]{1,20}$"))
                        {
                            json.Msg = "許可權值只能以英文數字組成，長度不能超過20個字元";
                            return Json(json);
                        }
                        entity.PERVALUE = nextpervalue;
                    }
                    //添加
                    if (entity.ID <= 0)
                    {
                        entity.CREATEDATE = DateTime.Now;
                        entity.UPDATEDATE = DateTime.Now;
                        entity.UPDATEUSER = this.CurrentUser.Name;
                        entity.CREATEUSER = this.CurrentUser.Name;
                    }
                    else //編輯
                    {
                        entity.UPDATEUSER = this.CurrentUser.Name;
                        entity.UPDATEDATE = DateTime.Now;
                        isEdit = true;
                    }
                    //同一模組下許可權不能重複
                    if (!this.PermissionManage.IsExist(p => p.NAME.Equals(entity.NAME) && p.ID != entity.ID && p.MODULEID == entity.MODULEID))
                    {
                        if (PermissionManage.SaveOrUpdate(entity, isEdit))
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
                        json.Msg = "許可權" + entity.NAME + "同一模組下已存在，不能重複添加";
                    }
                }
                else
                {
                    json.Msg = "未找到要保存的許可權記錄";
                }
                if (isEdit)
                {
                    WriteLog(Common.Enums.enumOperator.Edit, "修改許可權，結果：" + json.Msg, Common.Enums.enumLog4net.INFO);
                }
                else
                {
                    WriteLog(Common.Enums.enumOperator.Add, "添加許可權，結果：" + json.Msg, Common.Enums.enumLog4net.INFO);
                }
            }
            catch (Exception e)
            {
                json.Msg = "保存許可權發生內部錯誤！";
                WriteLog(Common.Enums.enumOperator.None, "對模組許可權按鈕的管理保存許可權：", e);
            }
            return Json(json);
        }
        /// <summary>
        /// 刪除許可權
        /// </summary>
        [UserAuthorizeAttribute(ModuleAlias = "Permission", OperaAction = "Remove")]
        public ActionResult Delete(string idList)
        {
            var json = new JsonHelper() { Msg = "刪除許可權成功", Status = "n" };
            try
            {
                if (!string.IsNullOrEmpty(idList))
                {
                    var idList1 = idList.Trim(',').Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries).Select(p => int.Parse(p)).ToList();
                    //判斷查找角色是否調用
                    if (!this.RolePermissionManage.IsExist(p => idList1.Any(e => e == p.PERMISSIONID)))
                    {
                        //判斷查找用戶是否調用
                        if (!this.UserPermissionManage.IsExist(p => idList1.Any(e => e == p.FK_PERMISSIONID)))
                        {
                            this.PermissionManage.Delete(p => idList1.Any(e => e == p.ID));
                            json.Status = "y";
                        }
                        else
                        {
                            json.Msg = "有用戶正在使用該許可權，不能刪除!";
                        }
                    }
                    else
                    {
                        json.Msg = "有角色正在使用該許可權，不能刪除!";
                    }
                }
                else
                {
                    json.Msg = "未找到要刪除的許可權記錄";
                }
                WriteLog(Common.Enums.enumOperator.Remove, "刪除許可權，結果：" + json.Msg, Common.Enums.enumLog4net.WARN);
            }
            catch (Exception e)
            {
                json.Msg = e.InnerException.Message;
                WriteLog(Common.Enums.enumOperator.Remove, "對模組許可權按鈕的管理刪除許可權：", e);
            }
            return Json(json);
        }


        /// <summary>
        /// 初始化許可權，默認增刪改查詳情
        /// <param name="id">模組ID</param>
        /// </summary>
        [UserAuthorizeAttribute(ModuleAlias = "Permission", OperaAction = "Reset")]
        public ActionResult Reset(string id)
        {
            var json = new JsonHelper() { Status = "n", Msg = "初始化完畢" };
            try
            {
                //判斷模組ID 是否符合規範
                if (string.IsNullOrEmpty(id) || !Regex.IsMatch(id, @"^\d+$"))
                {
                    json.Msg = "模組參數錯誤";
                    WriteLog(Common.Enums.enumOperator.Allocation, "初始化許可權，結果：" + json.Msg, Common.Enums.enumLog4net.ERROR);
                    return Json(json);
                }
                //將 ID 轉為 Int
                int newid = int.Parse(id);

                //判斷許可權裡 模組是否有了許可權
                if (this.PermissionManage.IsExist(p => p.MODULEID == newid))
                {
                    json.Msg = "該模組已存在許可權，無法初始化";
                    WriteLog(Common.Enums.enumOperator.Allocation, "初始化許可權，結果：" + json.Msg, Common.Enums.enumLog4net.ERROR);
                    return Json(json);
                }
                //添加默認許可權 
                var per = new string[] { "查看,View", "列表,List", "詳情,Detail", "添加,Add", "修改,Edit", "刪除,Remove" };
                var list = new List<Domain.SYS_PERMISSION>();
                foreach (var item in per)
                {
                    list.Add(new Domain.SYS_PERMISSION()
                    {
                        CREATEDATE = DateTime.Now,
                        CREATEUSER = this.CurrentUser.Name,
                        NAME = item.Split(',')[0],
                        PERVALUE = item.Split(',')[1],
                        UPDATEDATE = DateTime.Now,
                        UPDATEUSER = this.CurrentUser.Name,
                        MODULEID = newid,
                        SHOWORDER = 0
                    });
                }
                //批量添加
                if (this.PermissionManage.SaveList(list))
                {
                    json.Status = "y";
                }
                else
                {
                    json.Msg = "初始化失敗";
                }
                WriteLog(Common.Enums.enumOperator.Allocation, "初始化許可權，結果：" + json.Msg, Common.Enums.enumLog4net.INFO);
            }
            catch (Exception e)
            {
                json.Msg = e.InnerException.Message;
                WriteLog(Common.Enums.enumOperator.Allocation, "對模組許可權按鈕的管理初始化許可權：", e);
            }
            return Json(json);
        }

        /// <summary>
        /// 獲取模組樹形功能表
        /// </summary>
        public ActionResult GetTree()
        {
            var json = new JsonHelper() { Msg = "Success", Status = "y" };

            //獲取系統ID
            var sysId = Request.Form["sysId"];

            //判斷系統ID是否傳入
            if (string.IsNullOrEmpty(sysId))
            {
                json.Status = "n";
                json.Msg = "獲取模組失敗！";
                return Json(json);
            }
            try
            {
                //獲取系統下的模組清單 按照 SHOWORDER欄位 昇冪排列
                var query = this.ModuleManage.LoadAll(p => p.FK_BELONGSYSTEM == sysId).OrderBy(p => p.SHOWORDER).ToList();

                //這裡就是按照jsTree的格式 輸出一下 模組資訊
                var result = query.Select(m => new
                {
                    id = m.ID,
                    parent = m.PARENTID > 0 ? m.PARENTID.ToString() : "#",
                    text = m.NAME,
                    icon = m.LEVELS == 0 ? "fa fa-circle text-danger" : "fa fa-circle text-navy"
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
        /// 設置角色、用戶許可權
        /// </summary>
        public ActionResult SaveAllocation()
        {
            var json = new JsonHelper() { Msg = "分配許可權完畢", Status = "n" };
            //類型
            string tp = Request.Form["tp"];
            //對象ID
            string id = Request.Form["id"];
            //許可權ID集合
            string perid = Request.Form["perid"];

            string sysid = Request.Form["sysid"];

            if (string.IsNullOrEmpty(id))
            {
                json.Msg = "未要分配許可權的對象";
                WriteLog(Common.Enums.enumOperator.Allocation, "設置角色許可權，結果：" + json.Msg, Common.Enums.enumLog4net.ERROR);
                return Json(json);
            }

            if (string.IsNullOrEmpty(tp))
            {
                json.Msg = "未要分配許可權的類型";
                WriteLog(Common.Enums.enumOperator.Allocation, "設置角色許可權，結果：" + json.Msg, Common.Enums.enumLog4net.ERROR);
                return Json(json);
            }

            perid = perid.Trim(',');

            try
            {
                if (tp == "user")
                {
                    if (!this.UserPermissionManage.SetUserPermission(int.Parse(id), perid)) { json.Msg = "保存失敗"; WriteLog(Common.Enums.enumOperator.Allocation, "設置用戶許可權，結果：" + json.Msg, Common.Enums.enumLog4net.ERROR); return Json(json); }
                }
                else if (tp == "role")
                {
                    if (!this.RolePermissionManage.SetRolePermission(int.Parse(id), perid, sysid)) { json.Msg = "保存失敗"; WriteLog(Common.Enums.enumOperator.Allocation, "設置角色許可權，結果：" + json.Msg, Common.Enums.enumLog4net.ERROR); return Json(json); }
                }

                json.Status = "y";

                WriteLog(Common.Enums.enumOperator.Allocation, "設置角色許可權，結果：" + json.Msg, Common.Enums.enumLog4net.INFO);
            }
            catch (Exception e)
            {
                json.Msg = "設置角色許可權發生內部錯誤！";
                WriteLog(Common.Enums.enumOperator.Allocation, "設置角色許可權：", e);
            }
            return Json(json);
        }


        /// <summary>
        /// 角色、用戶分配許可權
        /// </summary>
        [UserAuthorizeAttribute(ModuleAlias = "Role", OperaAction = "Allocation")]
        public ActionResult PerAllocation()
        {
            //系統
            string sysname = "所有可作業系統";
            //用戶或角色ID
            string id = Request["id"];

            //許可權類型，user/role
            string tp = Request["tp"];

            string sysid = "";
            //搜索關鍵字
            ViewBag.Search = base.keywords;

            if (string.IsNullOrEmpty(tp))
            {
                return Content("<script>alert('未接收到需要分配許可權的類型')</script>");
            }
            if (string.IsNullOrEmpty(id))
            {
                return Content("<script>alert('未接收到需要分配許可權的物件')</script>");
            }

            int newid = int.Parse(id);

            //模組
            var moduleList = new List<Domain.SYS_MODULE>();


            if (tp == "role")
            {
                var Role = RoleManage.Get(p => p.ID == newid);
                var sys = SystemManage.Get(p => p.ID == Role.FK_BELONGSYSTEM.ToString());
                sysname = sys.NAME;
                sysid = sys.ID;

                //獲取角色所屬系統模組
                moduleList = this.ModuleManage.RecursiveModule(this.ModuleManage.LoadAll(p => p.FK_BELONGSYSTEM == Role.FK_BELONGSYSTEM).ToList());
            }
            else if (tp == "user")
            {
                //獲取管理員可作業系統模組
                moduleList = this.ModuleManage.RecursiveModule(this.ModuleManage.LoadAll(p => CurrentUser.System_Id.Any(e => e == p.FK_BELONGSYSTEM)).ToList());
            }

            //搜索關鍵字
            if (!string.IsNullOrEmpty(keywords))
            {
                moduleList = moduleList.Where(p => p.NAME.Contains(keywords.ToLower())).ToList();
            }

            ViewData["ModuleList"] = JsonConverter.JsonClass(moduleList.Select(p => new { p.ID, MODULENAME = GetModuleName(p.NAME, p.LEVELS), p.ICON, p.PARENTID, p.LEVELS }));

            //獲取許可權
            var moduleId = moduleList.Select(p => p.ID).ToList();

            ViewData["PermissionList"] = this.PermissionManage.LoadAll(p => moduleId.Any(e => e == p.MODULEID)).ToList();

            //根據類型獲取使用者/角色已選中的許可權
            var selectper = new List<string>();
            if (tp == "user")
            {
                selectper =
                    this.UserPermissionManage.LoadAll(p => p.FK_USERID == newid)
                        .Select(p => p.FK_PERMISSIONID)
                        .Cast<string>()
                        .ToList();
            }
            else if (tp == "role")
            {
                selectper =
                    this.RolePermissionManage.LoadAll(p => p.ROLEID == newid)
                        .Select(p => p.PERMISSIONID)
                        .Cast<string>()
                        .ToList();
            }

            ViewData["selectper"] = selectper;

            ViewData["PermissionType"] = tp;

            ViewData["objId"] = id;

            ViewData["systemName"] = sysname;

            ViewData["systemId"] = sysid;

            return View();
        }

        private object GetModuleName(string name, decimal? level)
        {
            if (level > 0)
            {
                string nbsp = "";
                for (int i = 0; i < level; i++)
                {
                    nbsp += "　　";
                }
                name = nbsp + "|--" + name;
            }
            return name;
        }
    }
}