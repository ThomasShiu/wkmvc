using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Common;
using Common.Enums;
using Domain;
using Service.IService;

namespace Service.ServiceImp
{
    public class UserManage : RepositoryBase<Domain.SYS_USER>, IService.IUserManage
    {
        IDepartmentManage DepartmentManage { get; set; }
        IUserDepartmentManage UserDepartmentManage { get; set; }
        IUserInfoManage UserInfoManage { get; set; }
        IUserRoleManage UserRoleManage { get; set; }
        IUserPermissionManage UserPermissionManage { get; set; }
        IPostUserManage PostUserManage { get; set; }
        IPermissionManage PermissionManage { get; set; }
        /// <summary>
        /// 管理用戶登錄驗證
        /// add yuangang by 2016-05-12
        /// </summary>
        /// <param name="useraccount">用戶名</param>
        /// <param name="password">加密密碼（AES）</param>
        /// <returns></returns>
        public Domain.SYS_USER UserLogin(string useraccount, string password)
        {
            var entity = this.Get(p => p.ACCOUNT == useraccount);

            //因為我們用的是AES的動態加密演算法，也就是沒有統一的金鑰，那麼兩次同樣字串的加密結果是不一樣的，所以這裡要通過解密來匹配
            //而不能通過再次加密輸入的密碼來匹配
            if (entity != null && new Common.CryptHelper.AESCrypt().Decrypt(entity.PASSWORD) == password)
            {
                return entity;
            }
            return null;
        }

        /// <summary>
        /// 是否超級管理員
        /// </summary>
        public bool IsAdmin(int userId)
        {
            //通過用戶ID獲取角色
            Domain.SYS_USER entity = this.Get(p => p.ID == userId);
            if (entity == null) return false;
            var roles = entity.SYS_USER_ROLE.Select(p => new Domain.SYS_ROLE
            {
                ID = p.SYS_ROLE.ID
            });
            //return roles.ToList().Any(item => item.ID == ClsDic.DicRole["超級管理員"]);
            return roles.ToList().Any(item => item.ID == ClsDic.DicRole["超級管理員"]);
        }

        /// <summary>
        /// 根據使用者ID獲取用戶名
        /// </summary>
        /// <param name="Id">用戶ID</param>
        /// <returns></returns>
        public string GetUserName(int Id)
        {
            var query = this.LoadAll(c => c.ID == Id);
            if (query == null || !query.Any())
            {
                return "";
            }
            return query.First().NAME;
        }

        /// <summary>
        /// 根據使用者ID獲取部門名稱
        /// </summary>
        public string GetUserDptName(int id)
        {
            if (id <= 0)
                return "";
            var dptid = this.Get(p => p.ID == id).DPTID;
            return this.DepartmentManage.Get(p => p.ID == dptid).NAME;
        }

        /// <summary>
        /// 根據使用者ID刪除使用者相關記錄
        /// 刪除原則：1、刪除使用者檔案
        ///           2、刪除用戶角色關係
        ///           3、刪除用戶許可權關係
        ///           4、刪除用戶崗位關係
        ///           5、刪除用戶部門關係
        ///           6、刪除用戶
        /// </summary>
        public bool Remove(int userId)
        {
            try
            {
                //檔案
                if (this.UserInfoManage.IsExist(p => p.USERID == userId))
                {
                    this.UserInfoManage.Delete(p => p.USERID == userId);
                }
                //用戶角色
                if (this.UserRoleManage.IsExist(p => p.FK_USERID == userId))
                {
                    this.UserRoleManage.Delete(p => p.FK_USERID == userId);
                }
                //用戶許可權
                if (this.UserPermissionManage.IsExist(p => p.FK_USERID == userId))
                {
                    this.UserPermissionManage.Delete(p => p.FK_USERID == userId);
                }
                //用戶崗位
                if (this.PostUserManage.IsExist(p => p.FK_USERID == userId))
                {
                    this.PostUserManage.Delete(p => p.FK_USERID == userId);
                }
                //用戶部門
                if (this.UserDepartmentManage.IsExist(p => p.USER_ID == userId))
                {
                    this.UserDepartmentManage.Delete(p => p.USER_ID == userId);
                }
                //用戶自身
                if (this.IsExist(p => p.ID == userId))
                {
                    this.Delete(p => p.ID == userId);
                }
                return true;
            }
            catch (Exception e) { throw e.InnerException; }
        }

        /// <summary>
        /// 根據使用者資訊獲取使用者所有的許可權
        /// </summary>
        private List<Domain.SYS_PERMISSION> GetPermissionByUser(Domain.SYS_USER users)
        {
            //1、超級管理員擁有所有權限
            if (IsAdmin(users.ID))
                return PermissionManage.LoadListAll(null);
            //2、普通用戶，合併當前用戶許可權與角色許可權
            var perlist = new List<Domain.SYS_PERMISSION>();
            //2.1合併用戶許可權
            perlist.AddRange(users.SYS_USER_PERMISSION.Select(p => p.SYS_PERMISSION).ToList());
            //2.2合同角色許可權
            ////todo:經典多對多的資料查詢Linq方法
            perlist.AddRange(users.SYS_USER_ROLE.Select(p => p.SYS_ROLE.SYS_ROLE_PERMISSION.Select(c => c.SYS_PERMISSION)).SelectMany(c => c.Select(e => e)).Cast<Domain.SYS_PERMISSION>().ToList());
            //3、去重
            ////todo:通過重寫IEqualityComparer<T>實現物件去重
            perlist = perlist.Distinct(new PermissionDistinct()).ToList();
            return perlist;
        }
        /// <summary>
        /// 根據使用者構造使用者基本資訊
        /// </summary>
        public Account GetAccountByUser(Domain.SYS_USER user)
        {
            if (user == null) return null;
            //用戶授權--->注意用戶的授權是包括角色許可權與自身許可權的
            var permission = GetPermissionByUser(user);
            //用戶角色
            var role = user.SYS_USER_ROLE.Select(p => p.SYS_ROLE).ToList();
            //用戶部門
            var dpt = user.SYS_USER_DEPARTMENT.Select(p => p.SYS_DEPARTMENT).ToList();
            //用戶崗位
            var post = user.SYS_POST_USER.ToList();
            //用戶主部門
            var dptInfo = this.DepartmentManage.Get(p => p.ID == user.DPTID);
            //使用者模組
            var module = permission.Select(p => p.SYS_MODULE).ToList().Distinct(new ModuleDistinct()).ToList();

            var systemid = new List<string> { "fddeab19-3588-4fe1-83b6-c15d4abb942d" };
            Account account = new Account()
            {
                Id = user.ID,
                Name = user.NAME,
                LogName = user.ACCOUNT,
                PassWord = user.PASSWORD,
                IsAdmin = IsAdmin(user.ID),
                DptInfo = dptInfo,
                Dpt = dpt,
                Face_Img = user.FACE_IMG,
                Permissions = permission,
                Roles = role,
                PostUser = post,
                Modules = module,
                System_Id = systemid,
                Levels = user.LEVELS,
                PinYin = user.PINYIN1,
            };
            return account;
        }

        /// <summary>
        /// 從Cookie中獲取使用者資訊
        /// </summary>
        public Account GetAccountByCookie()
        {
            var cookie = CookieHelper.GetCookie("cookie_rememberme");
            if (cookie != null)
            {
                //驗證json的有效性
                if (!string.IsNullOrEmpty(cookie.Value))
                {
                    //解密
                    var cookievalue = new Common.CryptHelper.AESCrypt().Decrypt(cookie.Value);
                    //是否為json
                    if (!Common.JsonHelper.JsonSplit.IsJson(cookievalue)) return null;
                    try
                    {
                        var jsonFormat = Common.JsonHelper.JsonConverter.ConvertJson(cookievalue);
                        if (jsonFormat != null)
                        {
                            var users = UserLogin(jsonFormat.username, new Common.CryptHelper.AESCrypt().Decrypt(jsonFormat.password));
                            if (users != null)
                                return GetAccountByUser(users);
                        }
                    }
                    catch { return null; }
                }
            }
            return null;
        }
    }


    /// <summary>
    /// 許可權去重覆，非常重要
    /// add yuangang by 2015-08-03
    /// </summary>
    public class PermissionDistinct : IEqualityComparer<Domain.SYS_PERMISSION>
    {
        public bool Equals(Domain.SYS_PERMISSION x, Domain.SYS_PERMISSION y)
        {
            return x.ID == y.ID;
        }

        public int GetHashCode(Domain.SYS_PERMISSION obj)
        {
            return obj.ToString().GetHashCode();
        }
    }
}

