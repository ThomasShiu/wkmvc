using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Common;
using Common.Enums;
using Common.Log4NetHelper;
using Service;
using Service.IService;

namespace WebPage.Controllers
{
    public class BaseController : Controller
    {
        #region 公用變數
        /// <summary>
        /// 查詢關鍵字
        /// </summary>
        public string keywords { get; set; }
        /// <summary>
        /// 視圖傳遞的分頁頁碼
        /// </summary>
        public int page { get; set; }
        /// <summary>
        /// 視圖傳遞的分頁條數
        /// </summary>
        public int pagesize { get; set; }
        /// <summary>
        /// 用戶容器，公用
        /// </summary>
        public IUserManage UserManage = Spring.Context.Support.ContextRegistry.GetContext().GetObject("Service.User") as IUserManage;
        #endregion

        protected IExtLog _log = ExtLogManager.GetLogger("dblog");

        #region 使用者物件
        /// <summary>
        /// 獲取當前使用者物件
        /// </summary>
        public Account CurrentUser
        {
            get
            {
                //從Session中獲取使用者物件
                if (SessionHelper.GetSession("CurrentUser") != null)
                {
                    return SessionHelper.GetSession("CurrentUser") as Account;
                }
                //Session過期 通過Cookies中的資訊 重新獲取使用者物件 並存儲於Session中
                var account = UserManage.GetAccountByCookie();
                SessionHelper.SetSession("CurrentUser", account);
                return account;
            }
        }

        public string EmailDomain => ConfigurationManager.AppSettings["EmailDomain"];
        #endregion

        // GET: Base
        //public ActionResult Index()
        //{
        //    return View();
        //}


        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            #region 登錄用戶驗證
            //1、判斷Session物件是否存在
            if (filterContext.HttpContext.Session == null)
            {
                filterContext.HttpContext.Response.Write(
                       " <script type='text/javascript'> alert('~登錄已過期，請重新登錄');window.top.location='/sys/account'; </script>");
                filterContext.RequestContext.HttpContext.Response.End();
                filterContext.Result = new EmptyResult();
                return;
            }
            //2、登錄驗證
            if (CurrentUser == null)
            {
                filterContext.HttpContext.Response.Write(
                    " <script type='text/javascript'> alert('登錄已過期，請重新登錄'); window.top.location='/sys/account';</script>");
                filterContext.RequestContext.HttpContext.Response.End();
                filterContext.Result = new EmptyResult();
                return;
            }

            #endregion


            #region 公共Get變數
            //分頁頁碼
            object p = filterContext.HttpContext.Request["page"];
            if (p == null || p.ToString() == "") { page = 1; } else { page = int.Parse(p.ToString()); }

            //搜索關鍵字
            string search = filterContext.HttpContext.Request.QueryString["Search"];
            if (!string.IsNullOrEmpty(search)) { keywords = search; }
            //顯示分頁條數
            string size = filterContext.HttpContext.Request.QueryString["example_length"];
            if (!string.IsNullOrEmpty(size) && System.Text.RegularExpressions.Regex.IsMatch(size.ToString(), @"^\d+$")) { pagesize = int.Parse(size.ToString()); } else { pagesize = 10; }
            #endregion
        }

        public void WriteLog(enumOperator action, string message, enumLog4net logLevel)
        {
            switch (logLevel)
            {
                case enumLog4net.INFO:
                    _log.Info(Utils.GetIP(), CurrentUser.Name, Request.Url.ToString(), action.ToString(), message);
                    return;
                case enumLog4net.WARN:
                    _log.Warn(Utils.GetIP(), CurrentUser.Name, Request.Url.ToString(), action.ToString(), message);
                    return;
                default:
                    _log.Error(Utils.GetIP(), CurrentUser.Name, Request.Url.ToString(), action.ToString(), message);
                    return;
            }
        }

        public void WriteLog(enumOperator action, string message, Exception e)
        {
            _log.Fatal(Utils.GetIP(), CurrentUser.Name, Request.Url.ToString(), action.ToString(), message + e.Message, e);
        }
    }


    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false)]
    public class UserAuthorizeAttribute : AuthorizeAttribute
    {
        #region 欄位和屬性
        /// <summary>
        /// 模組別名，可配置更改
        /// </summary>
        public string ModuleAlias { get; set; }
        /// <summary>
        /// 許可權動作
        /// </summary>
        public string OperaAction { get; set; }
        /// <summary>
        /// 許可權存取控制器參數
        /// </summary>
        private string Sign { get; set; }
        /// <summary>
        /// 基類產生實體
        /// </summary>
        public BaseController baseController = new BaseController();

        #endregion


        public override void OnAuthorization(AuthorizationContext filterContext)
        {
            //1、判斷模組是否對應
            if (string.IsNullOrEmpty(ModuleAlias))
            {
                filterContext.HttpContext.Response.Write(" <script type='text/javascript'> alert('^您沒有訪問該頁面的許可權！'); </script>");
                filterContext.RequestContext.HttpContext.Response.End();
                filterContext.Result = new EmptyResult();
                return;
            }

            //2、判斷用戶是否存在
            if (baseController.CurrentUser == null)
            {
                filterContext.HttpContext.Response.Write(" <script type='text/javascript'> alert('^登錄已過期，請重新登錄！');window.top.location='/'; </script>");
                filterContext.RequestContext.HttpContext.Response.End();
                filterContext.Result = new EmptyResult();
                return;
            }

            //對比變數，用於許可權認證
            var alias = ModuleAlias;

            #region 配置Sign調取控制器標識
            Sign = filterContext.RequestContext.HttpContext.Request.QueryString["sign"];
            if (!string.IsNullOrEmpty(Sign))
            {
                if (("," + ModuleAlias.ToLower()).Contains("," + Sign.ToLower()))
                {
                    alias = Sign;
                    filterContext.Controller.ViewData["Sign"] = Sign;
                }
            }
            #endregion

            //3、調用下面的方法，驗證是否有訪問此頁面的許可權，查看加操作
            var moduleId = baseController.CurrentUser.Modules.Where(p => p.ALIAS.ToLower() == alias.ToLower()).Select(p => p.ID).FirstOrDefault();
            bool _blAllowed = IsAllowed(baseController.CurrentUser, moduleId, OperaAction);
            if (!_blAllowed)
            {
                filterContext.HttpContext.Response.Write(" <script type='text/javascript'> alert('您沒有訪問當前頁面的許可權！');</script>");
                filterContext.RequestContext.HttpContext.Response.End();
                filterContext.Result = new EmptyResult();
                return;
            }

            //4、有許可權訪問頁面，將此頁面的許可權集合傳給頁面
            filterContext.Controller.ViewData["PermissionList"] = GetPermissByJson(baseController.CurrentUser, moduleId);
        }
        /// <summary>
        /// 獲取操作許可權Json字串，供視圖JS判斷使用
        /// </summary>
        string GetPermissByJson(Account account, int moduleId)
        {
            //操作許可權
            var _varPerListThisModule = account.Permissions.Where(p => p.MODULEID == moduleId).Select(R => new { R.PERVALUE }).ToList();
            return Common.JsonHelper.JsonConverter.Serialize(_varPerListThisModule);
        }

        /// <summary>
        /// 功能描述：判斷使用者是否有此模組的操作許可權
        /// </summary>
        bool IsAllowed(Account user, int moduleId, string action)
        {
            //判斷入口
            if (user == null || user.Id <= 0 || moduleId == 0 || string.IsNullOrEmpty(action)) return false;
            //驗證許可權
            var permission = user.Permissions.Where(p => p.MODULEID == moduleId);
            action = action.Trim(',');
            if (action.IndexOf(',') > 0)
            {
                permission = permission.Where(p => action.ToLower().Contains(p.PERVALUE.ToLower()));
            }
            else
            {
                permission = permission.Where(p => p.PERVALUE.ToLower() == action.ToLower());
            }
            return permission.Any();
        }
    }

    /// <summary>
    /// 模型去重覆，非常重要
    /// add yuangang by 2016-05-25
    /// </summary>
    public class ModuleDistinct : IEqualityComparer<Domain.SYS_MODULE>
    {
        public bool Equals(Domain.SYS_MODULE x, Domain.SYS_MODULE y)
        {
            return x.ID == y.ID;
        }

        public int GetHashCode(Domain.SYS_MODULE obj)
        {
            return obj.ToString().GetHashCode();
        }
    }
}
