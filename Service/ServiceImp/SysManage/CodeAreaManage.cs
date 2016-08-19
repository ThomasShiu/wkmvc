using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain;
using Service.IService;

namespace Service.ServiceImp
{
    public class CodeAreaManage : RepositoryBase<SYS_CODE_AREA>, ICodeAreaManage
    {
        public IQueryable<SYS_CODE_AREA> LoadProvince()
        {
            return this.LoadAll((SYS_CODE_AREA p) => (int)p.LEVELS == 1);
        }

        public IQueryable<SYS_CODE_AREA> LoadCity(string provinceId)
        {
            return this.LoadAll((SYS_CODE_AREA p) => (int)p.LEVELS == 2 && p.PID == provinceId);
        }

        public IQueryable<SYS_CODE_AREA> LoadCountry(string cityId)
        {
            return this.LoadAll((SYS_CODE_AREA p) => (int)p.LEVELS == 3 && p.PID == cityId);
        }

        public IQueryable<SYS_CODE_AREA> LoadCommunity(string countryId)
        {
            return this.LoadAll((SYS_CODE_AREA p) => (int)p.LEVELS == 4 && p.PID == countryId);
        }
    }
}
