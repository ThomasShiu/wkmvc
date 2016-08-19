using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain;

namespace Service.IService
{
    public interface ICodeAreaManage : IRepository<SYS_CODE_AREA>
    {
        IQueryable<SYS_CODE_AREA> LoadProvince();

        IQueryable<SYS_CODE_AREA> LoadCity(string provinceId);

        IQueryable<SYS_CODE_AREA> LoadCountry(string cityId);

        IQueryable<SYS_CODE_AREA> LoadCommunity(string countryId);
    }
}
