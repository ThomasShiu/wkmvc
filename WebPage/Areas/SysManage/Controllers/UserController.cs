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
    public class UserController : BaseController
    {
        #region 聲明容器
        /// <summary>
        /// 部門
        /// </summary>
        IDepartmentManage DepartmentManage { get; set; }
        /// <summary>
        /// 職位管理
        /// </summary>
        IPostManage PostManage { get; set; }
        /// <summary>
        /// 用戶職位
        /// </summary>
        IPostUserManage PostUserManage { get; set; }
        /// <summary>
        /// 使用者資訊
        /// </summary>
        IUserInfoManage UserInfoManage { get; set; }
        /// <summary>
        /// 字典編碼
        /// </summary>
        ICodeManage CodeManage { get; set; }
        /// <summary>
        /// 角色
        /// </summary>
        IRoleManage RoleManage { get; set; }
        #endregion


        /// <summary>
        /// 載入首頁
        /// </summary>
        /// <returns></returns>
        [UserAuthorizeAttribute(ModuleAlias = "User", OperaAction = "View")]
        public ActionResult Index()
        {
            try
            {

                #region 處理查詢參數
                string DepartId = Request.QueryString["DepartId"];
                ViewBag.Search = base.keywords;
                ViewData["DepartId"] = DepartId;
                #endregion

                ViewBag.dpt = this.DepartmentManage.GetDepartmentByDetail();

                return View(BindList(DepartId));
            }
            catch (Exception e)
            {
                WriteLog(Common.Enums.enumOperator.Select, "員工管理載入主頁：", e);
                throw e.InnerException;
            }

        }

        /// <summary>
        /// 載入使用者詳情資訊（基本）
        /// </summary>
        [UserAuthorizeAttribute(ModuleAlias = "User", OperaAction = "Detail")]
        public ActionResult Detail(int? id)
        {
            try
            {
                var _entity = new Domain.SYS_USER();

                var Postlist = "";

                if (id != null && id > 0)
                {
                    _entity = UserManage.Get(p => p.ID == id);
                    Postlist = String.Join(",", _entity.SYS_POST_USER.Select(p => p.FK_POSTID).ToList());
                }
                ViewBag.dpt = this.DepartmentManage.GetDepartmentByDetail();
                ViewBag.zw = this.CodeManage.LoadAll(p => p.CODETYPE == "ZW").ToList();
                ViewData["Postlist"] = Postlist;
                return View(_entity);
            }
            catch (Exception e)
            {
                WriteLog(Common.Enums.enumOperator.Select, "載入使用者詳情發生錯誤：", e);
                throw e.InnerException;
            }
        }


        /// <summary>
        /// 保存人員基本資訊
        /// </summary>
        [ValidateInput(false)]
        [UserAuthorizeAttribute(ModuleAlias = "User", OperaAction = "Add,Edit")]
        public ActionResult Save(Domain.SYS_USER entity)
        {
            bool isEdit = false;
            var json = new JsonHelper() { Msg = "保存成功", Status = "n" };
            try
            {
                if (entity != null)
                {
                    if (entity.ID <= 0) //添加
                    {
                        entity.CREATEDATE = DateTime.Now;
                        entity.CREATEPER = this.CurrentUser.Name;
                        entity.UPDATEDATE = DateTime.Now;
                        entity.UPDATEUSER = this.CurrentUser.Name;
                        entity.PASSWORD = new Common.CryptHelper.AESCrypt().Encrypt("111111");
                        entity.PINYIN1 = Common.ConvertHzToPz.Convert(entity.NAME).ToLower();
                        entity.PINYIN2 = Common.ConvertHzToPz.ConvertFirst(entity.NAME).ToLower();
                    }
                    else //修改
                    {
                        entity.UPDATEUSER = this.CurrentUser.Name;
                        entity.UPDATEDATE = DateTime.Now;
                        entity.PINYIN1 = Common.ConvertHzToPz.Convert(entity.NAME).ToLower();
                        entity.PINYIN2 = Common.ConvertHzToPz.ConvertFirst(entity.NAME).ToLower();
                        isEdit = true;
                    }
                    //檢測此用戶名是否重複
                    if (!this.UserManage.IsExist(p => p.ACCOUNT.Equals(entity.ACCOUNT) && p.ID != entity.ID))
                    {
                        if (this.UserManage.SaveOrUpdate(entity, isEdit))
                        {
                            //員工職位
                            var postlist = Request.Form["postlist"];
                            if (!string.IsNullOrEmpty(postlist))
                            {
                                //刪除員工職位
                                if (PostUserManage.IsExist(p => p.FK_USERID == entity.ID))
                                {
                                    PostUserManage.Delete(p => p.FK_USERID == entity.ID);
                                }
                                //添加新的員工職位
                                List<Domain.SYS_POST_USER> PostUser = new List<Domain.SYS_POST_USER>();
                                foreach (var item in postlist.Trim(',').Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries).Select(p => p).ToList())
                                {
                                    PostUser.Add(new Domain.SYS_POST_USER() { FK_POSTID = item, FK_USERID = entity.ID });
                                }
                                PostUserManage.SaveList(PostUser);
                            }
                        }
                        json.Status = "y";
                    }
                    else
                    {
                        json.Msg = "登錄帳號已被使用，請修改後再提交!";
                    }
                }
                else
                {
                    json.Msg = "未找到要操作的使用者記錄";
                }
                if (isEdit)
                {
                    WriteLog(Common.Enums.enumOperator.Edit, "修改用戶，結果：" + json.Msg, Common.Enums.enumLog4net.INFO);
                }
                else
                {
                    WriteLog(Common.Enums.enumOperator.Add, "添加用戶，結果：" + json.Msg, Common.Enums.enumLog4net.INFO);
                }
            }
            catch (Exception e)
            {
                json.Msg = "保存人員資訊發生內部錯誤！";
                WriteLog(Common.Enums.enumOperator.None, "保存使用者錯誤：", e);
            }
            return Json(json);

        }
        /// <summary>
        /// 方法注解：刪除用戶
        /// 驗證規則：1、超級管理員不能刪除
        ///           2、當前登錄用戶不能刪除
        ///           3、正常狀態使用者不能刪除
        ///           4、上級部門用戶不能刪除
        /// 刪除原則：1、刪除使用者檔案
        ///           2、刪除用戶角色關係
        ///           3、刪除用戶許可權關係
        ///           4、刪除用戶職位關係
        ///           5、刪除用戶部門關係
        ///           6、刪除用戶
        /// </summary>
        [UserAuthorizeAttribute(ModuleAlias = "User", OperaAction = "Remove")]
        public ActionResult Delete(string idList)
        {
            var json = new JsonHelper() { Status = "n", Msg = "刪除用戶成功" };
            try
            {
                //是否為空
                if (string.IsNullOrEmpty(idList)) { json.Msg = "未找到要刪除的用戶"; return Json(json); }
                string[] id = idList.Trim(',').Split(',');
                for (int i = 0; i < id.Length; i++)
                {
                    int userId = int.Parse(id[i]);
                    if (this.UserManage.IsAdmin(userId))
                    {
                        json.Msg = "被刪除用戶存在超級管理員，不能刪除!";
                        WriteLog(Common.Enums.enumOperator.Remove, "刪除用戶：" + json.Msg, Common.Enums.enumLog4net.ERROR);
                        return Json(json);
                    }
                    if (this.CurrentUser.Id == userId)
                    {
                        json.Msg = "當前登錄用戶，不能刪除!";
                        WriteLog(Common.Enums.enumOperator.Remove, "刪除用戶：" + json.Msg, Common.Enums.enumLog4net.ERROR);
                        return Json(json);
                    }
                    if (this.UserManage.Get(p => p.ID == userId).ISCANLOGIN)
                    {
                        json.Msg = "用戶未鎖定，不能刪除!";
                        WriteLog(Common.Enums.enumOperator.Remove, "刪除用戶：" + json.Msg, Common.Enums.enumLog4net.ERROR);
                        return Json(json);
                    }
                    if (this.CurrentUser.DptInfo != null)
                    {
                        string dptid = this.UserManage.Get(p => p.ID == userId).DPTID;
                        if (this.DepartmentManage.Get(m => m.ID == dptid).BUSINESSLEVEL < this.CurrentUser.DptInfo.BUSINESSLEVEL)
                        {
                            json.Msg = "不能刪除上級部門用戶!";
                            WriteLog(Common.Enums.enumOperator.Remove, "刪除用戶：" + json.Msg, Common.Enums.enumLog4net.ERROR);
                            return Json(json);
                        }
                    }
                    this.UserManage.Remove(userId);
                    json.Status = "y";
                    WriteLog(Common.Enums.enumOperator.Remove, "刪除用戶：" + json.Msg, Common.Enums.enumLog4net.WARN);
                }
            }
            catch (Exception e)
            {
                json.Msg = "刪除使用者發生內部錯誤！";
                WriteLog(Common.Enums.enumOperator.Remove, "刪除用戶：", e);
            }
            return Json(json);
        }

        /// <summary>
        /// 方法描述:根據傳入的使用者編號重置當前使用者密碼
        /// </summary>
        /// <param name="Id">用戶編號</param>
        /// <returns></returns>
        [UserAuthorizeAttribute(ModuleAlias = "User", OperaAction = "PwdReset")]
        public ActionResult ResetPwd(string idList)
        {
            var json = new JsonHelper() { Status = "n", Msg = "操作成功" };
            try
            {
                //校驗用戶編號是否為空
                if (string.IsNullOrEmpty(idList))
                {
                    json.Msg = "校驗失敗，用戶編號不能為空";
                    WriteLog(Common.Enums.enumOperator.Edit, "重置當前使用者密碼：" + json.Msg, Common.Enums.enumLog4net.ERROR);
                    return Json(json);
                }
                var idlist1 = idList.Trim(',').Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries).Select(p => int.Parse(p)).ToList();
                if (idlist1 != null && idlist1.Count > 0)
                {
                    foreach (var newid in idlist1)
                    {
                        var _user = UserManage.Get(p => p.ID == newid);
                        _user.PASSWORD = new Common.CryptHelper.AESCrypt().Encrypt(_user.ACCOUNT);
                        UserManage.Update(_user);
                    }
                }
                json.Status = "y";
                WriteLog(Common.Enums.enumOperator.Edit, "重置當前使用者密碼：" + json.Msg, Common.Enums.enumLog4net.INFO);
            }
            catch (Exception e)
            {
                json.Msg = "操作失敗";
                WriteLog(Common.Enums.enumOperator.Edit, "重置當前使用者密碼：", e);
            }
            return Json(json);
        }

        /// <summary>
        /// 載入人員檔案
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [UserAuthorizeAttribute(ModuleAlias = "User", OperaAction = "UserInfo")]
        public ActionResult UserInfo(int? userid)
        {
            try
            {
                //是否為人事部
                var IsMatters = true;

                var entity = new Domain.SYS_USERINFO();

                var UserName = CurrentUser.Name;

                if (userid != null && userid > 0)
                {
                    entity = UserInfoManage.Get(p => p.USERID == userid) ?? new Domain.SYS_USERINFO() { USERID = int.Parse(userid.ToString()) };
                    UserName = UserManage.Get(p => p.ID == userid).NAME;
                    if ((CurrentUser.DptInfo != null && CurrentUser.DptInfo.NAME != "人事部") || !CurrentUser.IsAdmin)
                    {
                        IsMatters = false;
                    }
                }
                else
                {
                    entity = UserInfoManage.Get(p => p.USERID == CurrentUser.Id) ?? new Domain.SYS_USERINFO() { USERID = CurrentUser.Id };
                }

                ViewData["UserName"] = UserName;

                ViewBag.IsMatters = IsMatters;

                Dictionary<string, string> dic = Common.Enums.ClsDic.DicCodeType;
                var dictype = this.CodeManage.GetDicType();
                //在職狀態
                string zgzt = dic["在職狀態"];
                ViewData["zgzt"] = dictype.Where(p => p.CODETYPE == zgzt).ToList();
                //婚姻狀況
                string hyzk = dic["婚姻狀況"];
                ViewData["hunyin"] = dictype.Where(p => p.CODETYPE == hyzk).ToList();
                //政治面貌
                string zzmm = dic["政治面貌"];
                ViewData["zzmm"] = dictype.Where(p => p.CODETYPE == zzmm).ToList();
                //民族
                string mz = dic["民族"];
                ViewData["mz"] = dictype.Where(p => p.CODETYPE == mz).ToList();
                //職稱級別
                string zcjb = dic["職稱"];
                ViewData["zcjb"] = dictype.Where(p => p.CODETYPE == zcjb).ToList();
                //學歷
                string xl = dic["學歷"];
                ViewData["xl"] = dictype.Where(p => p.CODETYPE == xl).ToList();

                return View(entity);
            }
            catch (Exception e)
            {
                WriteLog(Common.Enums.enumOperator.Select, "載入人員檔案：", e);
                throw e.InnerException;
            }

        }
        /// <summary>
        /// 保存人員檔案
        /// </summary>
        public ActionResult SetUserInfo(Domain.SYS_USERINFO entity)
        {
            bool isEdit = false;
            var json = new JsonHelper() { Msg = "保存人員檔案成功", Status = "n" };
            try
            {
                if (entity != null)
                {
                    #region 獲取html標籤值

                    //籍貫
                    entity.HomeTown = Request.Form["jgprov"] + "," + Request.Form["jgcity"] + "," +
                                      Request.Form["jgcountry"];
                    //戶口所在地
                    entity.HuJiSuoZaiDi = Request.Form["hkprov"] + "," + Request.Form["hkcity"] + "," +
                                          Request.Form["hkcountry"];

                    #endregion

                    //添加
                    if (entity.ID <= 0)
                    {
                        entity.CREATEUSER = CurrentUser.Name;
                        entity.CREATEDATE = DateTime.Now;
                        entity.UPDATEUSER = CurrentUser.Name;
                        entity.UPDATEDATE = DateTime.Now;
                    }
                    else
                    {
                        entity.UPDATEUSER = CurrentUser.Name;
                        entity.UPDATEDATE = DateTime.Now;
                        isEdit = true;
                    }



                    //修改使用者檔案
                    if (this.UserInfoManage.SaveOrUpdate(entity, isEdit))
                    {
                        json.Status = "y";

                    }
                    else
                    {
                        json.Msg = "保存使用者檔案失敗";

                    }

                }
                else
                {
                    json.Msg = "未找到要編輯的使用者記錄";
                }
                if (isEdit)
                {
                    WriteLog(Common.Enums.enumOperator.Edit, "保存人員檔案：" + json.Msg, Common.Enums.enumLog4net.INFO);
                }
                else
                {
                    WriteLog(Common.Enums.enumOperator.Add, "保存人員檔案：" + json.Msg, Common.Enums.enumLog4net.INFO);
                }
            }
            catch (Exception e)
            {
                json.Msg = e.InnerException.Message;
                WriteLog(Common.Enums.enumOperator.None, "保存人員檔案：", e);
            }
            return Json(json);
        }

        #region private
        /// <summary>
        /// 分頁查詢用戶列表
        /// </summary>
        private Common.PageInfo BindList(string DepartId)
        {
            //基礎資料
            var query = this.UserManage.LoadAll(p => p.ID > 0);

            //部門(本部門用戶及所有下級部門用戶)
            if (!string.IsNullOrEmpty(DepartId))
            {
                var childDepart = this.DepartmentManage.LoadAll(p => p.PARENTID == DepartId).Select(p => p.ID).ToList();
                query = query.Where(p => p.DPTID == DepartId || childDepart.Any(e => e == p.DPTID));
            }

            //查詢關鍵字
            if (!string.IsNullOrEmpty(keywords))
            {
                keywords = keywords.ToLower();
                query = query.Where(p => p.NAME.Contains(keywords) || p.ACCOUNT.Contains(keywords) || p.PINYIN2.Contains(keywords) || p.PINYIN1.Contains(keywords));
            }
            //排序
            query = query.OrderBy(p => p.SHOWORDER1).OrderByDescending(p => p.CREATEDATE);
            //分頁
            var result = this.UserManage.Query(query, page, pagesize);

            var list = result.List.Select(p => new
            {
                p.ID,
                p.NAME,
                p.ACCOUNT,
                DPTNAME = this.DepartmentManage.GetDepartmentName(p.DPTID),
                POSTNAME = GetPostName(p.SYS_POST_USER),
                ROLENAME = GetRoleName(p.SYS_USER_ROLE),
                p.CREATEDATE,
                ZW = this.CodeManage.Get(m => m.CODEVALUE == p.LEVELS && m.CODETYPE == "ZW").NAMETEXT,
                ISCANLOGIN = !p.ISCANLOGIN ? "<i class=\"fa fa-circle text-navy\"></i>" : "<i class=\"fa fa-circle text-danger\"></i>"

            }).ToList();

            return new Common.PageInfo(result.Index, result.PageSize, result.Count, JsonConverter.JsonClass(list));
        }
        /// <summary>
        /// 根據職位集合獲取職位名稱
        /// </summary>
        private string GetPostName(ICollection<Domain.SYS_POST_USER> collection)
        {
            string retval = string.Empty;
            if (collection != null && collection.Count > 0)
            {
                var postlist = String.Join(",", collection.Select(p => p.FK_POSTID).ToList()).Trim(',').Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries).Select(p => p).ToList();
                retval = String.Join(",", PostManage.LoadAll(p => postlist.Any(e => e == p.ID)).Select(p => p.POSTNAME).ToList());
            }
            return retval = retval.TrimEnd(',');
        }
        /// <summary>
        /// 根據角色集合獲取角色名稱
        /// </summary>
        private string GetRoleName(ICollection<Domain.SYS_USER_ROLE> collection)
        {
            string retval = string.Empty;
            if (collection != null && collection.Count > 0)
            {
                var rolelist = String.Join(",", collection.Select(p => p.FK_ROLEID).ToList()).Trim(',').Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries).Select(p => int.Parse(p)).ToList();
                retval = String.Join(",", RoleManage.LoadAll(p => rolelist.Any(e => e == p.ID)).Select(p => p.ROLENAME).ToList());
            }
            return retval = retval.TrimEnd(',');
        }

        #endregion

    }
}
