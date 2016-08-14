using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Service.IService
{
    /// <summary>
    /// Service 授权验证模块对应接口
    /// add yuangang by 2016-05-19
    /// </summary>
    public interface IPermissionManage : IRepository<Domain.SYS_PERMISSION>
    {
        /// <summary>
        /// 根据系统ID获取所有模块的权限ID集合
        /// </summary>
        List<int> GetPermissionIdBySysId(string sysId);
    }
}