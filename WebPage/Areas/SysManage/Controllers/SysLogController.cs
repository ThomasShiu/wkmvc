using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Common;
using Common.Enums;
using Common.JsonHelper;
using Domain;
using Service.IService;
using WebPage.Controllers;

namespace WebPage.Areas.SysManage.Controllers
{
    public class SysLogController : BaseController
    {
        ISyslogManage SyslogManage { get; set; }

        [UserAuthorize(ModuleAlias = "Syslog", OperaAction = "View")]
        public ActionResult Index()
        {
            string lv = Request.QueryString["level"];
            string act = Request.QueryString["actions"];
            object model = BindList(lv, act);
            base.ViewData["logaction"] = Tools.BindEnums(typeof(enumOperator));
            base.ViewData["sellog"] = act;
            base.ViewData["lev"] = lv;
            return base.View(model);
        }


        [UserAuthorize(ModuleAlias = "Syslog", OperaAction = "Detail")]
        public ActionResult Detail(int? id)
        {
            var model = SyslogManage.Get(p => p.ID == id) ?? new SYS_LOG();
            return View(model);
        }


        [UserAuthorize(ModuleAlias = "Syslog", OperaAction = "Remove")]
        public ActionResult Delete(string idList)
        {
            var jsonHelper = new JsonHelper
            {
                Msg = "删除日志完毕",
                Status = "n"
            };
            List<int> id = (from p in idList.Trim(new []{','}).Split(new []{","}, StringSplitOptions.RemoveEmptyEntries)
                            select int.Parse(p)).ToList();
            try
            {
                this.SyslogManage.Delete(p => id.Contains(p.ID));
                jsonHelper.Status = "y";
                WriteLog(enumOperator.Remove, "删除系统日志：" + jsonHelper.Msg, enumLog4net.WARN);
            }
            catch (Exception e)
            {
                jsonHelper.Msg = "删除系统日志发生内部错误！";
                WriteLog(enumOperator.Remove, "删除系统日志：", e);
            }
            return Json(jsonHelper);
        }


        private object BindList(string level, string action)
        {
            var expression = PredicateBuilder.True<SYS_LOG>();
            if (!string.IsNullOrEmpty(level))
            {
                expression = expression.And(p => p.LEVELS == level);
            }
            if (!string.IsNullOrEmpty(action))
            {
                expression = expression.And(p => p.ACTION == action);
            }
            if (!string.IsNullOrEmpty(keywords))
            {
                expression = expression.And(p => p.MESSAGE.Contains(keywords));
            }
            if (!CurrentUser.IsAdmin)
            {
                expression = expression.And(p => p.CLIENTUSER == CurrentUser.Name);
            }
            return SyslogManage.Query(from p in SyslogManage.LoadAll(expression)
                                           orderby p.DATES descending
                                           select p, base.page, base.pagesize);
        }
    }
}