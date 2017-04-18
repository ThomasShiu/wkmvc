using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Common.JsonHelper;
using Domain;
using Service.IService;
using WebPage.Controllers;

namespace WebPage.Areas.SysManage.Controllers
{
    public class HomeController : BaseController
    {
        IModuleManage ModuleManage { get; set; }

        IDepartmentManage DepartmentManage
        {
            get;
            set;
        }
        IUserOnlineManage UserOnlineManage
        {
            get;
            set;
        }
        // GET: Home
        public ActionResult Index()
        {
            //系統模組清單
            ViewData["Module"] = ModuleManage.GetModule(CurrentUser.Id, CurrentUser.Permissions, CurrentUser.System_Id);
            ViewData["Contacts"] = Contacts();
            return View(CurrentUser);
        }

        public ActionResult Default()
        {
            ViewData["MissionList"] = new List<Domain.PRO_PROJECT_TEAMS>();
            ViewData["week"] = DateTime.Now.DayOfWeek;
            ViewData["month"] = DateTime.Now.Month;
            ViewData["AttendanceList"] = new List<Domain.COM_WORKATTENDANCE>();
            return View(CurrentUser);
        }


        private object Contacts()
        {
            var obj = from m in (from m in DepartmentManage.LoadAll(m => m.BUSINESSLEVEL == 1)
                                 orderby m.SHOWORDER
                                 select m).ToList()
                      select new
                      {
                          ID = m.ID,
                          DepartName = m.NAME,
                          UserList = GetDepartUsers(m.ID)
                      };
            return JsonConverter.JsonClass(obj);
        }

        private object GetDepartUsers(string departId)
        {
            List<string> departs = (from p in getAllChildrenDeptIds(departId) // DepartmentManage.LoadAll(p => p.PARENTID == departId)
                                    orderby p.SHOWORDER
                                    select p.ID).ToList();
            departs.Add(departId); //加上頂層id，否則在頂層部門使用者不顯示
            var users = UserManage.LoadListAll(p => p.ID != CurrentUser.Id && departs.Any(e => e == p.DPTID))
                .OrderBy(p => p.LEVELS).ThenBy(p => p.CREATEDATE);
            var ret = users
                .Select(p =>
                {
                    return new
                    {
                        ID = p.ID,
                        FaceImg = (string.IsNullOrEmpty(p.FACE_IMG) ? ("/Pro/Project/User_Default_Avatat?name=" + p.NAME.Substring(0, 1)) : p.FACE_IMG),
                        NAME = p.NAME,
                        InsideEmail = p.ACCOUNT + base.EmailDomain,
                        LEVELS = p.LEVELS,
                        ConnectId = ((UserOnlineManage.LoadAll(m => m.FK_UserId == p.ID).FirstOrDefault() == null) ? "" : UserOnlineManage.LoadAll(m => m.FK_UserId == p.ID).FirstOrDefault().ConnectId),
                        IsOnline = (UserOnlineManage.LoadAll(m => m.FK_UserId == p.ID).FirstOrDefault() != null && UserOnlineManage.LoadAll(m => m.FK_UserId == p.ID).FirstOrDefault().IsOnline)
                    };
                })
                .OrderBy(p => p.IsOnline);
            return ret.ToList();
        }

        /// <summary>
        /// 迴圈查找部門的下屬部門
        /// </summary>
        /// <param name="topDeptId"></param>
        /// <returns></returns>
        private List<SYS_DEPARTMENT> getAllChildrenDeptIds(string topDeptId)
        {
            List<SYS_DEPARTMENT> ret = new List<SYS_DEPARTMENT>();

            var depts = DepartmentManage.LoadAll(p => p.PARENTID == topDeptId);
            ret.AddRange(depts);

            depts.ToList().ForEach(d =>
            {
                ret.AddRange(getAllChildrenDeptIds(d.ID));
            });


            return ret;
        }

    }
}