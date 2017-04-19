using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Service.IService;

namespace Service.ServiceImp
{
    public class SystemManage : RepositoryBase<Domain.SYS_SYSTEM>, ISystemManage
    {
        /// <summary>
        /// 獲取系統ID、NAME
        /// </summary>
        /// <param name="systems">使用者擁有操作許可權的系統</param>
        /// <returns></returns>
        public dynamic LoadSystemInfo(List<string> systems)
        {
            //return Common.JsonHelper.JsonConverter.JsonClass(this.LoadAll(null).Select(p => new { p.ID }).ToList());//??p.Name
            return Common.JsonHelper.JsonConverter.JsonClass(this.LoadAll(p => systems.Any(e => e == p.ID)).Select(p => new { p.ID, p.NAME }).ToList());
        }
    }
}