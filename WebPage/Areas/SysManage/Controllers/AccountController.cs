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
        #region 声明容器
        /// <summary>
        /// 用户管理
        /// add yuangang by 2016-05-16
        /// </summary>
        IUserManage UserManage { get; set; }
        /// <summary>
        /// 用户在线管理
        /// </summary>
        IUserOnlineManage UserOnlineManage { get; set; }
        /// <summary>
        /// 日志记录
        /// </summary>
        IExtLog log = ExtLogManager.GetLogger("dblog");
        #endregion

        #region 基本视图
        public ActionResult Index()
        {
            //移除Session
            SessionHelper.Remove("CurrentUser");
            CookieHelper.ClearCookie("cookie_rememberme");
            return View();
        }
        /// <summary>
        /// 登录验证
        /// add yuangang by 2016-05-16
        /// </summary>
        [ValidateAntiForgeryToken]
        public ActionResult Login(Domain.SYS_USER item)
        {
            var json = new JsonHelper() { Msg = "登录成功", Status = "n" };
            try
            {
                //获取表单验证码
                var code = Request.Form["code"];
                if (Session["gif"] != null)
                {
                    //判断用户输入的验证码是否正确
                    if (!string.IsNullOrEmpty(code) && code.ToLower() == Session["gif"].ToString().ToLower())
                    {
                        //调用登录验证接口 返回用户实体类
                        var users = UserManage.UserLogin(item.ACCOUNT.Trim(), item.PASSWORD.Trim());
                        if (users != null)
                        {
                            //是否锁定
                            if (users.ISCANLOGIN)
                            {
                                json.Msg = "用户已锁定，禁止登录，请联系管理员进行解锁";
                                log.Warn(Utils.GetIP(), item.ACCOUNT, Request.Url.ToString(), "Login", "系统登录，登录结果：" + json.Msg);
                                return Json(json);
                            }

                            var acconut = this.UserManage.GetAccountByUser(users);

                            //系统访问正常
                            if (acconut.System_Id.Count > 0)
                            {
                                //是否启用单用户登录
                                if (System.Configuration.ConfigurationManager.AppSettings["IsSingleLogin"] == "True")
                                {
                                    var UserOnline = UserOnlineManage.LoadListAll(p => p.FK_UserId == users.ID).FirstOrDefault();
                                    if (UserOnline != null && UserOnline.IsOnline)
                                    {
                                        json.Msg = "当前用户已登录，系统不允许重复登录！登录IP：" + UserOnline.UserIP;
                                        log.Error(Utils.GetIP(), item.ACCOUNT, Request.Url.ToString(), "Login", "重复登录：" + json.Msg);
                                    }
                                    else
                                    {
                                        //写入Session 当前登录用户
                                        SessionHelper.SetSession("CurrentUser", acconut);

                                        //记录用户信息到Cookies
                                        string cookievalue = "{\"id\":\"" + acconut.Id + "\",\"username\":\"" + acconut.LogName +
                                                             "\",\"password\":\"" + acconut.PassWord + "\",\"ToKen\":\"" +
                                                             Session.SessionID + "\"}";
                                        CookieHelper.SetCookie("cookie_rememberme", new Common.CryptHelper.AESCrypt().Encrypt(cookievalue),
                                            null);

                                        json.Status = "y";
                                        json.ReUrl = "/Sys/Home/Index";
                                        log.Info(Utils.GetIP(), item.ACCOUNT, Request.Url.ToString(), "Login", "系统登录，登录结果：" + json.Msg);
                                    }
                                }
                                else
                                {
                                    //写入Session 当前登录用户
                                    SessionHelper.SetSession("CurrentUser", acconut);

                                    //记录用户信息到Cookies
                                    string cookievalue = "{\"id\":\"" + acconut.Id + "\",\"username\":\"" + acconut.LogName +
                                                         "\",\"password\":\"" + acconut.PassWord + "\",\"ToKen\":\"" +
                                                         Session.SessionID + "\"}";
                                    CookieHelper.SetCookie("cookie_rememberme", new Common.CryptHelper.AESCrypt().Encrypt(cookievalue),
                                        null);

                                    json.Status = "y";
                                    json.ReUrl = "/Sys/Home/Index";
                                    log.Info(Utils.GetIP(), item.ACCOUNT, Request.Url.ToString(), "Login", "系统登录，登录结果：" + json.Msg);
                                }
                            }
                            else
                            {
                                json.Msg = "站点来源不可信，系统拒绝登录";
                                log.Warn(Utils.GetIP(), "其他系统访问者", "", "Login", "其他系统登录失败，原因：系统验证错误，系统拒绝登录");
                            }

                        }
                        else
                        {
                            json.Msg = "用户名或密码不正确";
                            log.Error(Utils.GetIP(), item.ACCOUNT, Request.Url.ToString(), "Login", "系统登录，登录结果：" + json.Msg);
                        }
                    }
                    else
                    {
                        json.Msg = "验证码不正确";
                        log.Error(Utils.GetIP(), item.ACCOUNT, Request.Url.ToString(), "Login", "系统登录，登录结果：" + json.Msg);
                    }
                }
                else
                {
                    json.Msg = "验证码已过期，请刷新验证码";
                    log.Error(Utils.GetIP(), item.ACCOUNT, Request.Url.ToString(), "Login", "系统登录，登录结果：" + json.Msg);
                }
            }
            catch (Exception e)
            {
                json.Msg = e.Message;
                log.Error(Utils.GetIP(), item.ACCOUNT, Request.Url.ToString(), "Login", "系统登录，登录结果：" + json.Msg);
            }
            return Json(json, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region 帮助方法
        /// <summary>
        /// 验证码
        /// </summary>
        public FileContentResult ValidateCode()
        {
            string code = "";
            System.IO.MemoryStream ms = new Models.VerifyCode().Create(out code);
            Session["gif"] = code;//验证码存储在Session中，供验证。  
            Response.ClearContent();//清空输出流 
            return File(ms.ToArray(), @"image/png");
        }
        #endregion
    }
}