using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Service.ServiceImp
{
    /// <summary>
    /// Service層部門管理
    /// add yuangang by 2016-05-19
    /// </summary>
    public class DepartmentManage : RepositoryBase<Domain.SYS_DEPARTMENT>, IService.IDepartmentManage
    {
        /// <summary>
        /// 自動創建部門編號
        /// add yuangang by 2016-05-19
        /// <param name="parentId">上級部門ID 注：ID不是Code，資料表已改</param>
        /// </summary>
        public string CreateCode(string parentId)
        {
            string _strCode = string.Empty;

            #region 驗證上級部門code是否為空，為空返回，第一級部門的Code
            if (string.IsNullOrEmpty(parentId))
            {
                //注意：Oracle存儲值為空=null MsSql 空=空 null=null
                var query = this.LoadAll(p => p.PARENTID == null || p.PARENTID == "").OrderBy(p => p.CODE).ToList();
                if (!query.Any())
                {
                    return "001";
                }
                //按照之前的邏輯，查漏補缺
                for (int i = 1; i <= query.Count; i++)
                {
                    string code = query[i - 1].CODE;
                    if (string.IsNullOrEmpty(code))
                    {
                        return FormatCode(i);
                    }
                    if (i != int.Parse(code))
                    {
                        return FormatCode(i);
                    }
                }
                return FormatCode(query.Count + 1);
            }
            #endregion

            #region 上級部門不為空,返回當前上級部門下的部門Code

            /* *根據部門編號獲取下級部門 查詢準則為：
             * 1.下級部門編號長度=當前部門編號+3 
             * 2.下級部門上級部門ID=當前部門ID
             * */
            var parentDpt = this.Get(p => p.ID == parentId);
            if (parentDpt != null)//上級部門存在
            {
                //查詢同等級部門下的所有資料
                var queryable = this.LoadAll(p => p.CODE.Length == parentDpt.CODE.Length + 3 && p.PARENTID == parentId).OrderBy(p => p.CODE).ToList();
                if (queryable.Any())
                {
                    //需要驗證是否存在編號缺失的情況 方法:遍歷下級部門列表，
                    //用部門編號去掉上級部門編號，然後轉化成數位和for迴圈的索引進行對比,遇到第一個不相等時，返回此編號，並跳出迴圈
                    for (int i = 1; i <= queryable.Count; i++)
                    {
                        string _code = queryable[i - 1].CODE;
                        _code = _code.Substring(parentDpt.CODE.Length);
                        int _intCode = 0;
                        Int32.TryParse(_code, out _intCode);
                        //下級部門編號中不存在
                        if (i != _intCode)
                        {
                            //返回此編號,並退出迴圈
                            _strCode = parentDpt.CODE + FormatCode(i);
                            return _strCode;
                        }
                    }
                    //不存在編號缺失情況
                    _strCode = parentDpt.CODE + FormatCode(queryable.Count + 1);
                }
                else
                {
                    _strCode = parentDpt.CODE + FormatCode(1);
                    return _strCode;
                }
            }//上級部門不存在，返回空，這種情況基本不會出現
            #endregion

            return _strCode;
        }
        /// <summary>
        /// 功能描述:根據傳入的數字 返回補數後的3位部門編號
        /// 創建標號:add yuangang by 2016-05-19
        /// </summary>
        public string FormatCode(int dptCode)
        {
            try
            {
                string _strCode = string.Empty;
                //<=9 一位數
                if (dptCode <= 9 && dptCode >= 1)
                {
                    return "00" + dptCode;
                }
                //<=99 兩位數
                else if (dptCode <= 99 && dptCode > 9)
                {
                    return "0" + dptCode;
                }
                //<==999 三位數
                else if (dptCode <= 999 && dptCode > 99)
                {
                    return _strCode;
                }
                return string.Empty;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 驗證當前刪除的部門是否存在下級部門
        /// </summary>
        public bool DepartmentIsExists(string idlist)
        {
            return this.IsExist(p => idlist.Contains(p.PARENTID));
        }

        /// <summary>
        /// 遞迴部門清單，返回排序後的物件集合
        /// add yuangang by 2016-05-19
        /// </summary>
        public List<Domain.SYS_DEPARTMENT> RecursiveDepartment(List<Domain.SYS_DEPARTMENT> list)
        {
            var result = new List<Domain.SYS_DEPARTMENT>();
            if (list.Count > 0)
            {
                ChildDepartment(list, result, null);
            }
            return result;
        }

        /// <summary>
        /// 根據部門ID遞迴部門清單，返回子部門+本部門的物件集合
        /// add yuangang by 2016-05-19
        /// </summary>
        public List<Domain.SYS_DEPARTMENT> RecursiveDepartment(string parentId)
        {
            //原始資料
            var list = this.LoadAll(null);
            //新數據
            var result = new List<Domain.SYS_DEPARTMENT>();
            if (list.Any())
            {
                result.AddRange(list.Where(p => p.ID == parentId).ToList());
                if (!string.IsNullOrEmpty(parentId))
                    ChildDepartment(list.ToList(), result, parentId);
                else
                    ChildDepartment(list.ToList(), result, null);//oracle使用null sql使用空
            }
            return result;
        }

        private void ChildDepartment(List<Domain.SYS_DEPARTMENT> newlist, List<Domain.SYS_DEPARTMENT> list, string id)
        {
            var result = newlist.Where(p => p.PARENTID == id).OrderBy(p => p.CODE).ThenBy(p => p.SHOWORDER).ToList();
            if (result.Any())
            {
                for (int i = 0; i < result.Count(); i++)
                {
                    list.Add(result[i]);
                    ChildDepartment(newlist, list, result[i].ID);
                }
            }
        }

        /// <summary>
        /// 根據部門ID獲取部門名稱，不存在返回空
        /// </summary>
        public string GetDepartmentName(string id)
        {
            var query = this.LoadAll(p => p.ID == id);
            if (query == null || !query.Any())
                return "";
            return query.First().NAME;
        }

        /// <summary>
        /// 顯示錯層方法
        /// </summary>
        public object GetDepartmentName(string name, decimal? level)
        {
            if (level > 1)
            {
                string nbsp = "&nbsp;&nbsp;";
                for (int i = 0; i < level; i++)
                {
                    nbsp += "&nbsp;&nbsp;";
                }
                name = nbsp + "|--" + name;
            }
            return name;
        }

        /// <summary>
        /// 獲取父級列表
        /// </summary>
        public IList GetDepartmentByDetail()
        {
            var list = RecursiveDepartment(this.LoadAll(null).ToList())
                .Select(p => new
                {
                    id = p.ID,
                    code = p.CODE,
                    name = GetDepartmentName(p.NAME, p.BUSINESSLEVEL)
                }).ToList();

            return Common.JsonHelper.JsonConverter.JsonClass(list);
        }
    }
}