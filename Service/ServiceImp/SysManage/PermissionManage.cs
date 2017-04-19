using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

namespace Service.ServiceImp
{
    /// <summary>
    /// Service 授權模組關係處理類
    /// add yuangang by 2016-05-19
    /// </summary>
    public class PermissionManage : RepositoryBase<Domain.SYS_PERMISSION>, IService.IPermissionManage
    {
        /// <summary>
        /// 根據系統ID獲取所有模組的許可權ID集合
        /// </summary>
        public List<int> GetPermissionIdBySysId(string sysId)
        {
            try
            {
                string sql = "select p.id from sys_permission p where exists(select 1 from sys_module t where t.fk_belongsystem=@sysid and t.id=p.moduleid)";
                DbParameter para = new SqlParameter("@sysid", sysId);
                return this.SelectBySql<int>(sql, para);
            }
            catch (Exception e)
            {
                throw e;
            }
        }
    }
}