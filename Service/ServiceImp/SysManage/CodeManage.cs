using Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Entity;

namespace Service.ServiceImp
{
    /// <summary>
    /// Service層代碼配置
    /// add yuangang by 2015-05-22
    /// </summary>
    public class CodeManage : RepositoryBase<Domain.SYS_CODE>, IService.ICodeManage
    {
        /// <summary>
        /// 根據編碼類型獲取編碼集合
        /// </summary>
        public List<Domain.SYS_CODE> GetCode(string codetype, params string[] codevalue)
        {
            var predicate = PredicateBuilder.True<Domain.SYS_CODE>();
            predicate = predicate.And(p => p.CODETYPE == codetype);
            if (codevalue != null && codevalue.Length > 0)
            {
                var str = codevalue.ToList();
                predicate = predicate.And(p => str.Contains(p.CODEVALUE));
            }
            return this.LoadAll(predicate).OrderBy(p => p.SHOWORDER).ToList();
        }

        /// <summary>
        /// 通過系統字典獲取編碼值
        /// </summary>
        public IQueryable<Domain.SYS_CODE> GetDicType()
        {
            Dictionary<string, string> code = Common.Enums.ClsDic.DicCodeType;
            string dic = code.Aggregate(string.Empty, (current, item) => current + (item.Value + ",")).TrimEnd(',');
            return this.LoadAll(p => dic.Contains(p.CODETYPE)).OrderBy(p => p.SHOWORDER);
        }

        /// <summary>
        /// 根據字典ID與類型獲取一條資料
        /// </summary>
        public string GetCodeByID(int id, string codetype)
        {
            return (this.Get(p => p.ID == id) ?? new Domain.SYS_CODE()).NAMETEXT;
        }

        /// <summary>
        /// 根據字典編碼值與類型獲取一條資料
        /// </summary>
        public string GetCodeNameByCodeValue(string codeType, string codevalue)
        {
            if (string.IsNullOrEmpty(codevalue))
                return "";
            var entity = this.Get(p => p.CODETYPE == codeType && p.CODEVALUE == codevalue);
            if (entity == null) return "";
            return entity.NAMETEXT;
        }
    }
}