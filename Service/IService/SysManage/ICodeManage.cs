using System.Collections.Generic;
using System.Linq;

namespace Service.IService
{
    /// <summary>
    /// Service層代碼配置介面
    /// add yuangang by 2015-05-22
    /// </summary>
    public interface ICodeManage : IRepository<Domain.SYS_CODE>
    {
        /// <summary>
        /// 根據編碼類型獲取編碼集合
        /// </summary>
        /// <param name="codetype">編碼類型</param>
        /// <param name="codevalue">編碼值</param>
        List<Domain.SYS_CODE> GetCode(string codetype, params string[] codevalue);
        /// <summary>
        /// 通過字典查詢字典指向的編碼集合
        /// </summary>
        IQueryable<Domain.SYS_CODE> GetDicType();
        /// <summary>
        /// 根據字典ID與類型獲取一條資料
        /// </summary>
        string GetCodeByID(int id, string codetype);
        /// <summary>
        /// 根據字典編碼值與類型獲取一條資料
        /// </summary>
        string GetCodeNameByCodeValue(string codeType, string codevalue);
    }
}