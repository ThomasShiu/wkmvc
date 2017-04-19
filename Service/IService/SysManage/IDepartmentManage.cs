using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Service.IService
{
    /// <summary>
    /// Service層部門管理介面
    /// add yuangang by 2016-05-19
    /// </summary>
    public interface IDepartmentManage : IRepository<Domain.SYS_DEPARTMENT>
    {
        /// <summary>
        /// 遞迴部門列表，返回按級別排序
        /// add yuangang by 2016-05-19
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        List<Domain.SYS_DEPARTMENT> RecursiveDepartment(List<Domain.SYS_DEPARTMENT> list);

        /// <summary>
        /// 根據部門ID遞迴部門清單，返回子部門+本部門的物件集合
        /// add yuangang by 2016-05-19
        /// </summary>
        List<Domain.SYS_DEPARTMENT> RecursiveDepartment(string parentId);
        /// <summary>
        /// 自動創建部門編號
        /// add yuangang by 2016-05-19
        /// </summary>
        string CreateCode(string parentCode);

        /// <summary>
        /// 部門是否存在下級部門
        /// add huafg by 2015-06-03
        /// </summary>
        bool DepartmentIsExists(string idlist);

        /// <summary>
        /// 根據部門ID獲取部門名稱，不存在返回空
        /// </summary>
        string GetDepartmentName(string id);
        /// <summary>
        /// 顯示錯層方法
        /// </summary>
        object GetDepartmentName(string name, decimal? level);
        /// <summary>
        /// 獲取部門父級列表
        /// </summary>
        System.Collections.IList GetDepartmentByDetail();

    }
}