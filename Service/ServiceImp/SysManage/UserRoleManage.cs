using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Domain;

namespace Service.ServiceImp
{
    /// <summary>
    /// Service層使用者與角色關係介面
    /// add yuangang by 2016-05-19
    /// </summary>
    public class UserRoleManage : RepositoryBase<Domain.SYS_USER_ROLE>, IService.IUserRoleManage
    {
        /// <summary>
        /// 設置用戶角色
        /// add yuangang by 2016-05-19
        /// </summary>
        /// <param name="userId">用戶ID</param>
        /// <param name="roleId">角色ID字串</param>
        public bool SetUserRole(int userId, string roleId)
        {
            try
            {
                //1、刪除用戶角色
                this.Delete(p => p.FK_USERID == userId);
                //2、設置當前用戶的角色
                if (string.IsNullOrEmpty(roleId)) return true;
                foreach (var entity in roleId.Split(',').Select(t => new Domain.SYS_USER_ROLE()
                {
                    FK_USERID = userId,
                    FK_ROLEID = int.Parse(t)
                }))
                {
                    _Context.Set<SYS_USER_ROLE>().Add(entity);
                }
                return this._Context.SaveChanges() > 0;
            }
            catch (Exception e) { throw e; }
        }
    }
}