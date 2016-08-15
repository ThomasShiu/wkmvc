using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Common;
using Common.JsonHelper;
using Common.Log4NetHelper;

namespace WebPage.Areas.SysManage.Controllers
{
    public class AccountController : Controller
    {
        Service.IService.IUserManage UserManage { get; set; }
        IExtLog log = ExtLogManager.GetLogger("dblog");

        public ActionResult Index()
        {
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
                var code = Request.Form["code"]; //验证码
                if(Session["gif"] != null)
                {
                    if(!String.IsNullOrEmpty(code) && code.ToLower() == Session["gif"].ToString().ToLower())
                    {
                        //调用登录验证接口 返回用户实体类
                        var user = UserManage.UserLogin(item.ACCOUNT.Trim(), item.PASSWORD.Trim());
                        if (user != null)
                        {
                            //是否锁定
                            if (!user.ISCANLOGIN)
                            {
                                json.Msg = "用户已锁定，禁止登录，请联系管理员进行解锁";
                                log.Warn(Utils.GetIP(), item.ACCOUNT, Request.Url.ToString(), "Login", "系统登录，登录结果：" + json.Msg);
                                return Json(json);
                            }


                            var account = UserManage.GetAccountByUser(user);
                            //写入Session
                            SessionHelper.SetSession("CurrentUser", account);

                            //记录用户信息到Cookie
                            string cookievalue = $@"{{""id"":""{account.Id}"",""username"":""{account.LogName}"",""password"":""{account.PassWord}"",""ToKen"":""{Session.SessionID}""}}";
                            CookieHelper.SetCookie("cookie_rememberme", new Common.CryptHelper.AESCrypt().Encrypt(cookievalue));

                            ////更新用户登录IP
                            //user.LastLoginIP = Utils.GetIP();
                            //UserManage.Update(user); //保存会出错



                            json.ReUrl = "/Sys/Home/Index";
                            json.Status = "y";
                            log.Info(Utils.GetIP(), item.ACCOUNT, Request.Url.ToString(), "Login", "系统登录，登录结果：" + json.Msg);

                        }
                        else
                        {
                            json.Msg = "用户名或密码不正确";
                            log.Error(Utils.GetIP(), item.ACCOUNT, Request.Url.ToString(), "Login", "系统登录，登录结果：" + json.Msg);

                        }
                    }
                    else
                    {
                        json.Msg = "验证码不正确。";
                        log.Error(Utils.GetIP(), item.ACCOUNT, Request.Url.ToString(), "Login", "系统登录，登录结果：" + json.Msg);

                    }

                }
                else
                {
                    json.Msg = "验证码已过期。";
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

        public FileContentResult ValidateCode()
        {
            string code = "";
            MemoryStream ms = new Models.VerifyCode().Create(out code);
            Session["gif"] = code;
            Response.ClearContent();
            return File(ms.ToArray(),@"image/png");
        }

    }
}