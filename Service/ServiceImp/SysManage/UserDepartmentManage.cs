using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Service.ServiceImp
{
    /// <summary>
    /// 用戶部門關係業務實現類
    /// add yuangang by 2016-05-19
    /// </summary>
    public class UserDepartmentManage : RepositoryBase<Domain.SYS_USER_DEPARTMENT>, IService.IUserDepartmentManage
    {
        /// <summary>
        /// 根據部門ID獲取當前部門的所有使用者ID集合
        /// </summary>
        public List<Domain.SYS_USER> GetUserListByDptId(List<string> dptId)
        {
            return this.LoadAll(p => dptId.Contains(p.DEPARTMENT_ID)).Select(p => p.SYS_USER).ToList();
        }
        /// <summary>
        /// 根據使用者ID獲取所在的部門ID集合
        /// </summary>
        public List<Domain.SYS_DEPARTMENT> GetDptListByUserId(int userId)
        {
            return this.LoadAll(p => p.USER_ID == userId).Select(p => p.SYS_DEPARTMENT).ToList();
        }

        /// <summary>
        /// 保存用戶部門關係
        /// </summary>
        /// <param name="userId">用戶ID</param>
        /// <param name="dptId">部門ID集合</param>
        public bool SaveUserDpt(int userId, string dptId)
        {
            try
            {
                //原始部門人員關係是否與當前設置一致，不一致重新構造
                if (this.IsExist(p => p.USER_ID == userId))
                {
                    //存在之後再對比是否一致 
                    var oldCount = this.LoadAll(p => p.USER_ID == userId && dptId.Contains(p.DEPARTMENT_ID)).Select(p => p.DEPARTMENT_ID).ToList();
                    var newdptid = dptId.Split(',').OrderBy(c => c).ToList();
                    if (oldCount.Count == newdptid.Count && oldCount.All(newdptid.Contains)) return true;
                    //刪除原有關係
                    this.Delete(p => p.USER_ID == userId);
                }
                if (!string.IsNullOrEmpty(dptId))
                {
                    //添加現有關係
                    var list = dptId.Split(',').Select(item => new Domain.SYS_USER_DEPARTMENT()
                    {
                        DEPARTMENT_ID = item,
                        USER_ID = userId
                    }).ToList();
                    return SaveList(list);
                }
                return true;
            }
            catch (Exception e) { throw e.InnerException; }
        }
    }
}