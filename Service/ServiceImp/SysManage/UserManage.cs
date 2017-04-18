﻿using System;
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
        /// 管理用户登录验证
        /// add yuangang by 2016-05-12
        /// </summary>
        /// <param name="useraccount">用户名</param>
        /// <param name="password">加密密码（AES）</param>
        /// <returns></returns>
        public Domain.SYS_USER UserLogin(string useraccount, string password)
        {
            var entity = this.Get(p => p.ACCOUNT == useraccount);

            //因为我们用的是AES的动态加密算法，也就是没有统一的密钥，那么两次同样字符串的加密结果是不一样的，所以这里要通过解密来匹配
            //而不能通过再次加密输入的密码来匹配
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
            //通过用户ID获取角色
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
        /// 根据用户ID获取用户名
        /// </summary>
        /// <param name="Id">用户ID</param>
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
        /// 根据用户ID获取部门名称
        /// </summary>
        public string GetUserDptName(int id)
        {
            if (id <= 0)
                return "";
            var dptid = this.Get(p => p.ID == id).DPTID;
            return this.DepartmentManage.Get(p => p.ID == dptid).NAME;
        }

        /// <summary>
        /// 根据用户ID删除用户相关记录
        /// 删除原则：1、删除用户档案
        ///           2、删除用户角色关系
        ///           3、删除用户权限关系
        ///           4、删除用户岗位关系
        ///           5、删除用户部门关系
        ///           6、删除用户
        /// </summary>
        public bool Remove(int userId)
        {
            try
            {
                //档案
                if (this.UserInfoManage.IsExist(p => p.USERID == userId))
                {
                    this.UserInfoManage.Delete(p => p.USERID == userId);
                }
                //用户角色
                if (this.UserRoleManage.IsExist(p => p.FK_USERID == userId))
                {
                    this.UserRoleManage.Delete(p => p.FK_USERID == userId);
                }
                //用户权限
                if (this.UserPermissionManage.IsExist(p => p.FK_USERID == userId))
                {
                    this.UserPermissionManage.Delete(p => p.FK_USERID == userId);
                }
                //用户岗位
                if (this.PostUserManage.IsExist(p => p.FK_USERID == userId))
                {
                    this.PostUserManage.Delete(p => p.FK_USERID == userId);
                }
                //用户部门
                if (this.UserDepartmentManage.IsExist(p => p.USER_ID == userId))
                {
                    this.UserDepartmentManage.Delete(p => p.USER_ID == userId);
                }
                //用户自身
                if (this.IsExist(p => p.ID == userId))
                {
                    this.Delete(p => p.ID == userId);
                }
                return true;
            }
            catch (Exception e) { throw e.InnerException; }
        }

        /// <summary>
        /// 根据用户信息获取用户所有的权限
        /// </summary>
        private List<Domain.SYS_PERMISSION> GetPermissionByUser(Domain.SYS_USER users)
        {
            //1、超級管理員拥有所有权限
            if (IsAdmin(users.ID))
                return PermissionManage.LoadListAll(null);
            //2、普通用户，合并当前用户权限与角色权限
            var perlist = new List<Domain.SYS_PERMISSION>();
            //2.1合并用户权限
            perlist.AddRange(users.SYS_USER_PERMISSION.Select(p => p.SYS_PERMISSION).ToList());
            //2.2合同角色权限
            ////todo:经典多对多的数据查询Linq方法
            perlist.AddRange(users.SYS_USER_ROLE.Select(p => p.SYS_ROLE.SYS_ROLE_PERMISSION.Select(c => c.SYS_PERMISSION)).SelectMany(c => c.Select(e => e)).Cast<Domain.SYS_PERMISSION>().ToList());
            //3、去重
            ////todo:通过重写IEqualityComparer<T>实现对象去重
            perlist = perlist.Distinct(new PermissionDistinct()).ToList();
            return perlist;
        }
        /// <summary>
        /// 根据用户构造用户基本信息
        /// </summary>
        public Account GetAccountByUser(Domain.SYS_USER user)
        {
            if (user == null) return null;
            //用户授权--->注意用户的授权是包括角色权限与自身权限的
            var permission = GetPermissionByUser(user);
            //用户角色
            var role = user.SYS_USER_ROLE.Select(p => p.SYS_ROLE).ToList();
            //用户部门
            var dpt = user.SYS_USER_DEPARTMENT.Select(p => p.SYS_DEPARTMENT).ToList();
            //用户岗位
            var post = user.SYS_POST_USER.ToList();
            //用户主部门
            var dptInfo = this.DepartmentManage.Get(p => p.ID == user.DPTID);
            //用户模块
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
        /// 从Cookie中获取用户信息
        /// </summary>
        public Account GetAccountByCookie()
        {
            var cookie = CookieHelper.GetCookie("cookie_rememberme");
            if (cookie != null)
            {
                //验证json的有效性
                if (!string.IsNullOrEmpty(cookie.Value))
                {
                    //解密
                    var cookievalue = new Common.CryptHelper.AESCrypt().Decrypt(cookie.Value);
                    //是否为json
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
    /// 权限去重，非常重要
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
