using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Service.IService;
using WebPage.Controllers;

namespace WebPage.Areas.SysManage.Controllers
{
    public class HomeController : BaseController
    {
        IModuleManage ModuleManage { get; set; }
        // GET: Home
        public ActionResult Index()
        {
            //系统模块列表
            ViewData["Module"] = ModuleManage.GetModule(CurrentUser.Id, CurrentUser.Permissions, CurrentUser.System_Id);
            return View(CurrentUser);
        }

        public ActionResult Default()
        {
            ViewData["MissionList"] = new List<Domain.PRO_PROJECT_TEAMS>();
            ViewData["week"] = DateTime.Now.DayOfWeek;
            ViewData["month"] = DateTime.Now.Month;            
            ViewData["AttendanceList"] =new List<Domain.COM_WORKATTENDANCE>();
            return View(CurrentUser);
        }
    }
}