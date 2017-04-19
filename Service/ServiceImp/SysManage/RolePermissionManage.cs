using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Domain;
using Service.IService;

namespace Service.ServiceImp
{
    /// <summary>
    /// Service層角色授權關係介面
    /// add yuangang by 2015-05-22
    /// </summary>
    public class RolePermissionManage : RepositoryBase<Domain.SYS_ROLE_PERMISSION>, IService.IRolePermissionManage
    {
        IPermissionManage PermissionManage { get; set; }
        /// <summary>
        /// 保存角色許可權
        /// </summary>
        public bool SetRolePermission(int roleId, string newper, string sysId)
        {
            try
            {
                //1、獲取當前系統的模組ID集合
                var permissionId = this.PermissionManage.GetPermissionIdBySysId(sysId).Cast<int>().ToList();
                //2、獲取角色許可權，是否存在，存在即刪除，只刪除當前選擇的系統
                if (this.IsExist(p => p.ROLEID == roleId && permissionId.Any(e => e == p.PERMISSIONID)))
                {
                    //3、刪除角色許可權
                    this.Delete(p => p.ROLEID == roleId && permissionId.Any(e => e == p.PERMISSIONID));
                }
                //4、添加角色許可權
                if (string.IsNullOrEmpty(newper)) return true;
                //Trim 保證資料安全
                var str = newper.Trim(',').Split(',');
                foreach (var per in str.Select(t => new Domain.SYS_ROLE_PERMISSION()
                {
                    PERMISSIONID = int.Parse(t),
                    ROLEID = roleId
                }))
                {
                    this._Context.Set<SYS_ROLE_PERMISSION>().Add(per);
                }
                //5、Save
                return this._Context.SaveChanges() > 0;
            }
            catch (Exception e) { throw e.InnerException; }
        }
    }
}