using Common;
using Service.IService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Common.JsonHelper;
using Common.Log4NetHelper;

namespace WebPage.Areas.SysManage.Controllers
{
    public class AccountController : Controller
    {
        #region 聲明容器
        /// <summary>
        /// 用戶管理
        /// add yuangang by 2016-05-16
        /// </summary>
        IUserManage UserManage { get; set; }
        /// <summary>
        /// 用戶線上管理
        /// </summary>
        IUserOnlineManage UserOnlineManage { get; set; }
        /// <summary>
        /// 日誌記錄
        /// </summary>
        IExtLog log = ExtLogManager.GetLogger("dblog");
        #endregion

        #region 基本視圖
        public ActionResult Index()
        {
            //移除Session
            SessionHelper.Remove("CurrentUser");
            CookieHelper.ClearCookie("cookie_rememberme");
            return View();
        }
        /// <summary>
        /// 登錄驗證
        /// add yuangang by 2016-05-16
        /// </summary>
        [ValidateAntiForgeryToken]
        public ActionResult Login(Domain.SYS_USER item)
        {
            var json = new JsonHelper() { Msg = "登錄成功", Status = "n" };
            try
            {
                //獲取表單驗證碼
                var code = Request.Form["code"];
                if (Session["gif"] != null)
                {
                    //判斷用戶輸入的驗證碼是否正確
                    if (!string.IsNullOrEmpty(code) && code.ToLower() == Session["gif"].ToString().ToLower())
                    {
                        //調用登錄驗證介面 返回使用者實體類
                        var users = UserManage.UserLogin(item.ACCOUNT.Trim(), item.PASSWORD.Trim());
                        if (users != null)
                        {
                            //是否鎖定
                            if (users.ISCANLOGIN)
                            {
                                json.Msg = "用戶已鎖定，禁止登錄，請聯繫管理員進行解鎖";
                                log.Warn(Utils.GetIP(), item.ACCOUNT, Request.Url.ToString(), "Login", "系統登錄，登錄結果：" + json.Msg);
                                return Json(json);
                            }

                            var acconut = this.UserManage.GetAccountByUser(users);

                            //系統訪問正常
                            if (acconut.System_Id.Count > 0)
                            {
                                //是否啟用單用戶登錄
                                if (System.Configuration.ConfigurationManager.AppSettings["IsSingleLogin"] == "True")
                                {
                                    var UserOnline = UserOnlineManage.LoadListAll(p => p.FK_UserId == users.ID).FirstOrDefault();
                                    if (UserOnline != null && UserOnline.IsOnline)
                                    {
                                        json.Msg = "當前使用者已登錄，系統不允許重複登錄！登錄IP：" + UserOnline.UserIP;
                                        log.Error(Utils.GetIP(), item.ACCOUNT, Request.Url.ToString(), "Login", "重複登錄：" + json.Msg);
                                    }
                                    else
                                    {
                                        //寫入Session 當前登錄用戶
                                        SessionHelper.SetSession("CurrentUser", acconut);

                                        //記錄使用者資訊到Cookies
                                        string cookievalue = "{\"id\":\"" + acconut.Id + "\",\"username\":\"" + acconut.LogName +
                                                             "\",\"password\":\"" + acconut.PassWord + "\",\"ToKen\":\"" +
                                                             Session.SessionID + "\"}";
                                        CookieHelper.SetCookie("cookie_rememberme", new Common.CryptHelper.AESCrypt().Encrypt(cookievalue),
                                            null);

                                        json.Status = "y";
                                        json.ReUrl = "/Sys/Home/Index";
                                        log.Info(Utils.GetIP(), item.ACCOUNT, Request.Url.ToString(), "Login", "系統登錄，登錄結果：" + json.Msg);
                                    }
                                }
                                else
                                {
                                    //寫入Session 當前登錄用戶
                                    SessionHelper.SetSession("CurrentUser", acconut);

                                    //記錄使用者資訊到Cookies
                                    string cookievalue = "{\"id\":\"" + acconut.Id + "\",\"username\":\"" + acconut.LogName +
                                                         "\",\"password\":\"" + acconut.PassWord + "\",\"ToKen\":\"" +
                                                         Session.SessionID + "\"}";
                                    CookieHelper.SetCookie("cookie_rememberme", new Common.CryptHelper.AESCrypt().Encrypt(cookievalue),
                                        null);

                                    json.Status = "y";
                                    json.ReUrl = "/Sys/Home/Index";
                                    log.Info(Utils.GetIP(), item.ACCOUNT, Request.Url.ToString(), "Login", "系統登錄，登錄結果：" + json.Msg);
                                }
                            }
                            else
                            {
                                json.Msg = "網站來源不可信，系統拒絕登錄";
                                log.Warn(Utils.GetIP(), "其他系統訪問者", "", "Login", "其他系統登錄失敗，原因：系統驗證錯誤，系統拒絕登錄");
                            }

                        }
                        else
                        {
                            json.Msg = "用戶名或密碼不正確";
                            log.Error(Utils.GetIP(), item.ACCOUNT, Request.Url.ToString(), "Login", "系統登錄，登錄結果：" + json.Msg);
                        }
                    }
                    else
                    {
                        json.Msg = "驗證碼不正確";
                        log.Error(Utils.GetIP(), item.ACCOUNT, Request.Url.ToString(), "Login", "系統登錄，登錄結果：" + json.Msg);
                    }
                }
                else
                {
                    json.Msg = "驗證碼已過期，請刷新驗證碼";
                    log.Error(Utils.GetIP(), item.ACCOUNT, Request.Url.ToString(), "Login", "系統登錄，登錄結果：" + json.Msg);
                }
            }
            catch (Exception e)
            {
                json.Msg = e.Message;
                log.Error(Utils.GetIP(), item.ACCOUNT, Request.Url.ToString(), "Login", "系統登錄，登錄結果：" + json.Msg);
            }
            return Json(json, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region 幫助方法
        /// <summary>
        /// 驗證碼
        /// </summary>
        public FileContentResult ValidateCode()
        {
            string code = "";
            System.IO.MemoryStream ms = new Models.VerifyCode().Create(out code);
            Session["gif"] = code;//驗證碼存儲在Session中，供驗證。  
            Response.ClearContent();//清空輸出流 
            return File(ms.ToArray(), @"image/png");
        }
        #endregion
    }
}