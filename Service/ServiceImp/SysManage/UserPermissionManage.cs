using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Domain;
using Service.IService;

namespace Service.ServiceImp
{
    /// <summary>
    /// Service層使用者授權介面
    /// add yuangang by 2016-05-19
    /// </summary>
    public class UserPermissionManage : RepositoryBase<SYS_USER_PERMISSION>, IUserPermissionManage
    {
        IPermissionManage PermissionManage { get; set; }
        /// <summary>
        /// 保存用戶許可權
        /// </summary>
        public bool SetUserPermission(int userId, string newper)//, string sysId)
        {
            try
            {
                ////1、獲取當前系統的模組ID集合
                //var permissionId = this.PermissionManage.GetPermissionIdBySysId(sysId).Cast<int>().ToList();
                //2、獲取用戶許可權，是否存在，存在即刪除
                if (this.IsExist(p => p.FK_USERID == userId)) // && permissionId.Any(e => e == p.FK_PERMISSIONID)))
                {
                    //3、刪除用戶許可權
                    this.Delete(p => p.FK_USERID == userId); // && permissionId.Any(e => e == p.FK_PERMISSIONID));
                }
                //4、添加用戶許可權
                var str = newper.Trim(',').Split(',');
                foreach (var per in str.Select(t => new Domain.SYS_USER_PERMISSION()
                {
                    FK_USERID = userId,
                    FK_PERMISSIONID = int.Parse(t)
                }))
                {
                    this._Context.Set<SYS_USER_PERMISSION>().Add(per);
                }
                //5、Save
                return this._Context.SaveChanges() > 0;
            }
            catch (Exception e) { throw e; }
        }
    }
}