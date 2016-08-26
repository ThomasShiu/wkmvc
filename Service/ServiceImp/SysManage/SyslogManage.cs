using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain;
using Service.IService;

namespace Service.ServiceImp
{
    public class SyslogManage: RepositoryBase<SYS_LOG>, ISyslogManage
    {
    }
}
