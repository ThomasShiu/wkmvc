using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Service.ServiceImp
{
    /// <summary>
    /// Service模型處理類
    /// add yuangang by 2015-05-22
    /// </summary>
    public class ModuleManage : RepositoryBase<Domain.SYS_MODULE>, IService.IModuleManage
    {
        /// <summary>
        /// 獲取使用者許可權模組集合
        /// add yuangang by 2015-05-30
        /// </summary>
        /// <param name="userId">用戶ID</param>
        /// <param name="permission">使用者授權集合</param>
        /// <param name="siteId">網站ID</param>
        /// <returns></returns>
        public List<Domain.SYS_MODULE> GetModule(int userId, List<Domain.SYS_PERMISSION> permission, List<string> systemid)
        {
            //返回模組
            var retmodule = new List<Domain.SYS_MODULE>();
            var permodule = new List<Domain.SYS_MODULE>();
            //許可權轉模組
            if (permission != null)
            {
                permodule.AddRange(permission.Select(p => p.SYS_MODULE));
                //去重
                permodule = permodule.Distinct(new ModuleDistinct()).ToList();
            }
            //檢索顯示與系統
            permodule = permodule.Where(p => p.ISSHOW && systemid.Any(e => e == p.FK_BELONGSYSTEM)).ToList();
            //構造上級導航模組
            var prevModule = this.LoadListAll(p => systemid.Any(e => e == p.FK_BELONGSYSTEM));
            //反向遞迴演算法構造模組帶上級上上級模組
            if (permodule.Count > 0)
            {
                foreach (var item in permodule)
                {
                    RecursiveModule(prevModule, retmodule, item.PARENTID);
                    retmodule.Add(item);
                }
            }
            //去重
            retmodule = retmodule.Distinct(new ModuleDistinct()).ToList();
            //返回模組集合
            return retmodule.OrderBy(p => p.LEVELS).ThenBy(p => p.SHOWORDER).ToList();
        }

        /// <summary>
        /// 反向遞迴模組集合，可重複模組資料，最後去重
        /// </summary>
        /// <param name="PrevModule">總模組</param>
        /// <param name="retmodule">返回模組</param>
        /// <param name="parentId">上級ID</param>
        private void RecursiveModule(List<Domain.SYS_MODULE> PrevModule, List<Domain.SYS_MODULE> retmodule, int? parentId)
        {
            var result = PrevModule.Where(p => p.ID == parentId);
            if (result != null)
            {
                foreach (var item in result)
                {
                    retmodule.Add(item);
                    RecursiveModule(PrevModule, retmodule, item.PARENTID);
                }
            }
        }

        /// <summary>
        /// 遞迴模組清單，返回按級別排序
        /// add yuangang by 2015-06-03
        /// </summary>
        public List<Domain.SYS_MODULE> RecursiveModule(List<Domain.SYS_MODULE> list)
        {
            List<Domain.SYS_MODULE> result = new List<Domain.SYS_MODULE>();
            if (list != null && list.Count > 0)
            {
                ChildModule(list, result, 0);
            }
            return result;
        }
        /// <summary>
        /// 遞迴模組清單
        /// add yuangang by 2015-06-03
        /// </summary>
        private void ChildModule(List<Domain.SYS_MODULE> list, List<Domain.SYS_MODULE> newlist, int parentId)
        {
            var result = list.Where(p => p.PARENTID == parentId).OrderBy(p => p.LEVELS).OrderBy(p => p.SHOWORDER).ToList();
            if (result.Count() > 0)
            {
                for (int i = 0; i < result.Count(); i++)
                {
                    newlist.Add(result[i]);
                    ChildModule(list, newlist, result[i].ID);
                }
            }
        }

        /// <summary>
        /// 批量變更下級模組的級別
        /// </summary>
        public bool MoreModifyModule(int moduleId, int levels)
        {
            //根據當前模組ID獲取下級模組的集合
            var ChildModule = this.LoadAll(p => p.PARENTID == moduleId).ToList();
            if (ChildModule.Any())
            {
                foreach (var item in ChildModule)
                {
                    item.LEVELS = levels + 1;
                    this.Update(item);
                    MoreModifyModule(item.ID, item.LEVELS);
                }
            }
            return true;
        }

        /// <summary>
        /// 獲取範本清單
        /// </summary>
        public dynamic LoadModuleInfo(int id)
        {
            return Common.JsonHelper.JsonConverter.JsonClass(this.LoadAll(p => p.PARENTID == id).OrderBy(p => p.ID).Select(p => new { p.ID, p.NAME }).ToList());
        }
    }
    /// <summary>
    /// 模型去重，非常重要
    /// add yuangang by 2015-08-03
    /// </summary>
    public class ModuleDistinct : IEqualityComparer<Domain.SYS_MODULE>
    {
        public bool Equals(Domain.SYS_MODULE x, Domain.SYS_MODULE y)
        {
            return x.ID == y.ID;
        }

        public int GetHashCode(Domain.SYS_MODULE obj)
        {
            return obj.ToString().GetHashCode();
        }
    }
}
