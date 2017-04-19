using Common;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Data.Entity.Infrastructure;
using System.Data.Entity;
using System.Linq.Expressions;
using System.Threading.Tasks;
using System.Collections;

namespace Service
{
    /// <summary>
    /// 所有的資料操作基類介面
    /// add yuangang by 2015-05-10
    /// </summary>
    public interface IRepository<T> where T : class
    {
        #region 資料物件操作
        /// <summary>
        /// 數據上下文
        /// </summary>
        DbContext _Context { get; }
        /// <summary>
        /// 數據上下文
        /// </summary>
        Domain.MyConfig Config { get; }
        #endregion

        #region 單模型 CRUD 操作
        /// <summary>
        /// 增加一條記錄
        /// </summary>
        /// <param name="entity">實體模型</param>
        /// <param name="IsCommit">是否提交（默認提交）</param>
        /// <returns></returns>
        bool Save(T entity, bool IsCommit = true);
        /// <summary>
        /// 增加一條記錄（非同步方式）
        /// </summary>
        /// <param name="entity">實體模型</param>
        /// <param name="IsCommit">是否提交（默認提交）</param>
        /// <returns></returns>
        Task<bool> SaveAsync(T entity, bool IsCommit = true);

        /// <summary>
        /// 更新一條記錄
        /// </summary>
        /// <param name="entity">實體模型</param>
        /// <param name="IsCommit">是否提交（默認提交）</param>
        /// <returns></returns>
        bool Update(T entity, bool IsCommit = true);
        /// <summary>
        /// 更新一條記錄（非同步方式）
        /// </summary>
        /// <param name="entity">實體模型</param>
        /// <param name="IsCommit">是否提交（默認提交）</param>
        /// <returns></returns>
        Task<bool> UpdateAsync(T entity, bool IsCommit = true);

        /// <summary>
        /// 增加或更新一條記錄
        /// </summary>
        /// <param name="entity">實體模型</param>
        /// <param name="isEdit">是否增加</param>
        /// <param name="IsCommit">是否提交（默認提交）</param>
        /// <returns></returns>
        bool SaveOrUpdate(T entity, bool isEdit, bool IsCommit = true);
        /// <summary>
        /// 增加或更新一條記錄（非同步方式）
        /// </summary>
        /// <param name="entity">實體模型</param>
        /// <param name="isEdit">是否增加</param>
        /// <param name="IsCommit">是否提交（默認提交）</param>
        /// <returns></returns>
        Task<bool> SaveOrUpdateAsync(T entity, bool isEdit, bool IsCommit = true);

        /// <summary>
        /// 通過Lamda運算式獲取實體
        /// </summary>
        /// <param name="predicate">Lamda運算式（p=>p.Id==Id）</param>
        /// <returns></returns>
        T Get(Expression<Func<T, bool>> predicate);
        /// <summary>
        /// 通過Lamda運算式獲取實體（非同步方式）
        /// </summary>
        /// <param name="predicate">Lamda運算式（p=>p.Id==Id）</param>
        /// <returns></returns>
        Task<T> GetAsync(Expression<Func<T, bool>> predicate);

        /// <summary>
        /// 刪除一條記錄
        /// </summary>
        /// <param name="entity">實體模型</param>
        /// <param name="IsCommit">是否提交（默認提交）</param>
        /// <returns></returns>
        bool Delete(T entity, bool IsCommit = true);
        /// <summary>
        /// 刪除一條記錄（非同步方式）
        /// </summary>
        /// <param name="entity">實體模型</param>
        /// <param name="IsCommit">是否提交（默認提交）</param>
        /// <returns></returns>
        Task<bool> DeleteAsync(T entity, bool IsCommit = true);

        #endregion

        #region 多模型操作
        /// <summary>
        /// 增加多條記錄，同一模型
        /// </summary>
        /// <param name="T1">實體模型集合</param>
        /// <param name="IsCommit">是否提交（默認提交）</param>
        /// <returns></returns>
        bool SaveList(List<T> T1, bool IsCommit = true);
        /// <summary>
        /// 增加多條記錄，同一模型（非同步方式）
        /// </summary>
        /// <param name="T1">實體模型集合</param>
        /// <param name="IsCommit">是否提交（默認提交）</param>
        /// <returns></returns>
        Task<bool> SaveListAsync(List<T> T1, bool IsCommit = true);

        /// <summary>
        /// 增加多條記錄，獨立模型
        /// </summary>
        /// <param name="T1">實體模型集合</param>
        /// <param name="IsCommit">是否提交（默認提交）</param>
        /// <returns></returns>
        bool SaveList<T1>(List<T1> T, bool IsCommit = true) where T1 : class;
        /// <summary>
        /// 增加多條記錄，獨立模型（非同步方式）
        /// </summary>
        /// <param name="T1">實體模型集合</param>
        /// <param name="IsCommit">是否提交（默認提交）</param>
        /// <returns></returns>
        Task<bool> SaveListAsync<T1>(List<T1> T, bool IsCommit = true) where T1 : class;

        /// <summary>
        /// 更新多條記錄，同一模型
        /// </summary>
        /// <param name="T1">實體模型集合</param>
        /// <param name="IsCommit">是否提交（默認提交）</param>
        /// <returns></returns>
        bool UpdateList(List<T> T1, bool IsCommit = true);
        /// <summary>
        /// 更新多條記錄，同一模型（非同步方式）
        /// </summary>
        /// <param name="T1">實體模型集合</param>
        /// <param name="IsCommit">是否提交（默認提交）</param>
        /// <returns></returns>
        Task<bool> UpdateListAsync(List<T> T1, bool IsCommit = true);

        /// <summary>
        /// 更新多條記錄，獨立模型
        /// </summary>
        /// <param name="T1">實體模型集合</param>
        /// <param name="IsCommit">是否提交（默認提交）</param>
        /// <returns></returns>
        bool UpdateList<T1>(List<T1> T, bool IsCommit = true) where T1 : class;
        /// <summary>
        /// 更新多條記錄，獨立模型（非同步方式）
        /// </summary>
        /// <param name="T1">實體模型集合</param>
        /// <param name="IsCommit">是否提交（默認提交）</param>
        /// <returns></returns>
        Task<bool> UpdateListAsync<T1>(List<T1> T, bool IsCommit = true) where T1 : class;

        /// <summary>
        /// 刪除多條記錄，同一模型
        /// </summary>
        /// <param name="T1">實體模型集合</param>
        /// <param name="IsCommit">是否提交（默認提交）</param>
        /// <returns></returns>
        bool DeleteList(List<T> T1, bool IsCommit = true);
        /// <summary>
        /// 刪除多條記錄，同一模型（非同步方式）
        /// </summary>
        /// <param name="T1">實體模型集合</param>
        /// <param name="IsCommit">是否提交（默認提交）</param>
        /// <returns></returns>
        Task<bool> DeleteListAsync(List<T> T1, bool IsCommit = true);

        /// <summary>
        /// 刪除多條記錄，獨立模型
        /// </summary>
        /// <param name="T1">實體模型集合</param>
        /// <param name="IsCommit">是否提交（默認提交）</param>
        /// <returns></returns>
        bool DeleteList<T1>(List<T1> T, bool IsCommit = true) where T1 : class;
        /// <summary>
        /// 刪除多條記錄，獨立模型（非同步方式）
        /// </summary>
        /// <param name="T1">實體模型集合</param>
        /// <param name="IsCommit">是否提交（默認提交）</param>
        /// <returns></returns>
        Task<bool> DeleteListAsync<T1>(List<T1> T, bool IsCommit = true) where T1 : class;

        /// <summary>
        /// 通過Lamda運算式，刪除一條或多條記錄
        /// </summary>
        /// <param name="predicate"></param>
        /// <param name="IsCommit"></param>
        /// <returns></returns>
        bool Delete(Expression<Func<T, bool>> predicate, bool IsCommit = true);
        /// <summary>
        /// 通過Lamda運算式，刪除一條或多條記錄（非同步方式）
        /// </summary>
        /// <param name="predicate"></param>
        /// <param name="IsCommit"></param>
        /// <returns></returns>
        Task<bool> DeleteAsync(Expression<Func<T, bool>> predicate, bool IsCommit = true);

        /// <summary>
        /// 執行SQL刪除
        /// </summary>
        /// <param name="sql">SQL語句</param>
        /// <param name="para">Parameters參數</param>
        int DeleteBySql(string sql, params DbParameter[] para);
        /// <summary>
        /// 執行SQL刪除（非同步方式）
        /// </summary>
        /// <param name="sql">SQL語句</param>
        /// <param name="para">Parameters參數</param>
        Task<int> DeleteBySqlAsync(string sql, params DbParameter[] para);
        #endregion

        #region 存儲過程操作
        /// <summary>
        /// 執行增刪改存儲過程
        /// </summary>
        object ExecuteProc(string procname, params DbParameter[] parameter);
        /// <summary>
        /// 執行查詢的存儲過程
        /// </summary>
        object ExecuteQueryProc(string procname, params DbParameter[] parameter);
        #endregion

        #region 獲取多條資料操作

        /// <summary>
        /// 返回IQueryable集合，延時載入資料
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        IQueryable<T> LoadAll(Expression<Func<T, bool>> predicate);
        /// <summary>
        /// 返回IQueryable集合，延時載入資料（非同步方式）
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        Task<IQueryable<T>> LoadAllAsync(Expression<Func<T, bool>> predicate);

        // <summary>
        /// 返回List<T>集合,不採用延時載入
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        List<T> LoadListAll(Expression<Func<T, bool>> predicate);
        // <summary>
        /// 返回List<T>集合,不採用延時載入（非同步方式）
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        Task<List<T>> LoadListAllAsync(Expression<Func<T, bool>> predicate);

        /// <summary>
        /// 獲取DbQuery的列表
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        DbQuery<T> LoadQueryAll(Expression<Func<T, bool>> predicate);
        /// <summary>
        /// 獲取DbQuery的清單（非同步方式）
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        Task<DbQuery<T>> LoadQueryAllAsync(Expression<Func<T, bool>> predicate);

        /// <summary>
        /// 獲取IEnumerable列表
        /// </summary>
        /// <param name="sql">SQL語句</param>
        /// <param name="para">Parameters參數</param>
        /// <returns></returns>
        IEnumerable<T> LoadEnumerableAll(string sql, params DbParameter[] para);
        /// <summary>
        /// 獲取IEnumerable清單（非同步方式）
        /// </summary>
        /// <param name="sql">SQL語句</param>
        /// <param name="para">Parameters參數</param>
        /// <returns></returns>
        Task<IEnumerable<T>> LoadEnumerableAllAsync(string sql, params DbParameter[] para);

        /// <summary>
        /// 獲取資料動態集合
        /// </summary>
        /// <param name="sql">SQL語句</param>
        /// <param name="para">Parameters參數</param>
        /// <returns></returns>
        IEnumerable LoadEnumerable(string sql, params DbParameter[] para);
        /// <summary>
        /// 獲取資料動態集合（非同步方式）
        /// </summary>
        /// <param name="sql">SQL語句</param>
        /// <param name="para">Parameters參數</param>
        /// <returns></returns>
        Task<IEnumerable> LoadEnumerableAsync(string sql, params DbParameter[] para);

        /// <summary>
        /// 採用SQL進行資料的查詢，返回IList集合
        /// </summary>
        /// <param name="sql">SQL語句</param>
        /// <param name="para">Parameters參數</param>
        /// <returns></returns>
        List<T> SelectBySql(string sql, params DbParameter[] para);
        /// <summary>
        /// 採用SQL進行資料的查詢，返回IList集合（非同步方式）
        /// </summary>
        /// <param name="sql">SQL語句</param>
        /// <param name="para">Parameters參數</param>
        /// <returns></returns>
        Task<List<T>> SelectBySqlAsync(string sql, params DbParameter[] para);

        /// <summary>
        /// 採用SQL進行資料的查詢，指定泛型，返回IList集合
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <param name="sql"></param>
        /// <param name="para"></param>
        /// <returns></returns>
        List<T1> SelectBySql<T1>(string sql, params DbParameter[] para);
        /// <summary>
        /// 採用SQL進行資料的查詢，指定泛型，返回IList集合
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <param name="sql"></param>
        /// <param name="para"></param>
        /// <returns></returns>
        Task<List<T1>> SelectBySqlAsync<T1>(string sql, params DbParameter[] para);

        /// <summary>
        /// 可指定返回結果、排序、查詢準則的通用查詢方法，返回實體物件集合
        /// </summary>
        /// <typeparam name="TEntity">實體物件</typeparam>
        /// <typeparam name="TOrderBy">排序欄位類型</typeparam>
        /// <typeparam name="TResult">資料結果，與TEntity一致</typeparam>
        /// <param name="where">過濾條件，需要用到類型轉換的需要提前處理與資料表一致的</param>
        /// <param name="orderby">排序欄位</param>
        /// <param name="selector">返回結果（必須是模型中存在的欄位）</param>
        /// <param name="IsAsc">排序方向，true為正序false為倒序</param>
        /// <returns>實體集合</returns>
        List<TResult> QueryEntity<TEntity, TOrderBy, TResult>(Expression<Func<TEntity, bool>> where, Expression<Func<TEntity, TOrderBy>> orderby, Expression<Func<TEntity, TResult>> selector, bool IsAsc)
            where TEntity : class
            where TResult : class;
        /// <summary>
        /// 可指定返回結果、排序、查詢準則的通用查詢方法，返回實體物件集合（非同步方式）
        /// </summary>
        /// <typeparam name="TEntity">實體物件</typeparam>
        /// <typeparam name="TOrderBy">排序欄位類型</typeparam>
        /// <typeparam name="TResult">資料結果，與TEntity一致</typeparam>
        /// <param name="where">過濾條件，需要用到類型轉換的需要提前處理與資料表一致的</param>
        /// <param name="orderby">排序欄位</param>
        /// <param name="selector">返回結果（必須是模型中存在的欄位）</param>
        /// <param name="IsAsc">排序方向，true為正序false為倒序</param>
        /// <returns>實體集合</returns>
        Task<List<TResult>> QueryEntityAsync<TEntity, TOrderBy, TResult>(Expression<Func<TEntity, bool>> where, Expression<Func<TEntity, TOrderBy>> orderby, Expression<Func<TEntity, TResult>> selector, bool IsAsc)
            where TEntity : class
            where TResult : class;

        /// <summary>
        /// 可指定返回結果、排序、查詢準則的通用查詢方法，返回Object物件集合
        /// </summary>
        /// <typeparam name="TEntity">實體物件</typeparam>
        /// <typeparam name="TOrderBy">排序欄位類型</typeparam>
        /// <param name="where">過濾條件，需要用到類型轉換的需要提前處理與資料表一致的</param>
        /// <param name="orderby">排序欄位</param>
        /// <param name="selector">返回結果（必須是模型中存在的欄位）</param>
        /// <param name="IsAsc">排序方向，true為正序false為倒序</param>
        /// <returns>自訂實體集合</returns>
        List<object> QueryObject<TEntity, TOrderBy>(Expression<Func<TEntity, bool>> where, Expression<Func<TEntity, TOrderBy>> orderby, Func<IQueryable<TEntity>, List<object>> selector, bool IsAsc)
            where TEntity : class;
        /// <summary>
        /// 可指定返回結果、排序、查詢準則的通用查詢方法，返回Object物件集合（非同步方式）
        /// </summary>
        /// <typeparam name="TEntity">實體物件</typeparam>
        /// <typeparam name="TOrderBy">排序欄位類型</typeparam>
        /// <param name="where">過濾條件，需要用到類型轉換的需要提前處理與資料表一致的</param>
        /// <param name="orderby">排序欄位</param>
        /// <param name="selector">返回結果（必須是模型中存在的欄位）</param>
        /// <param name="IsAsc">排序方向，true為正序false為倒序</param>
        /// <returns>自訂實體集合</returns>
        Task<List<object>> QueryObjectAsync<TEntity, TOrderBy>(Expression<Func<TEntity, bool>> where, Expression<Func<TEntity, TOrderBy>> orderby, Func<IQueryable<TEntity>, List<object>> selector, bool IsAsc)
            where TEntity : class;

        /// <summary>
        /// 可指定返回結果、排序、查詢準則的通用查詢方法，返回動態類物件集合
        /// </summary>
        /// <typeparam name="TEntity">實體物件</typeparam>
        /// <typeparam name="TOrderBy">排序欄位類型</typeparam>
        /// <param name="where">過濾條件，需要用到類型轉換的需要提前處理與資料表一致的</param>
        /// <param name="orderby">排序欄位</param>
        /// <param name="selector">返回結果（必須是模型中存在的欄位）</param>
        /// <param name="IsAsc">排序方向，true為正序false為倒序</param>
        /// <returns>動態類</returns>
        dynamic QueryDynamic<TEntity, TOrderBy>(Expression<Func<TEntity, bool>> where, Expression<Func<TEntity, TOrderBy>> orderby, Func<IQueryable<TEntity>, List<object>> selector, bool IsAsc)
            where TEntity : class;
        /// <summary>
        /// 可指定返回結果、排序、查詢準則的通用查詢方法，返回動態類物件集合（非同步方式）
        /// </summary>
        /// <typeparam name="TEntity">實體物件</typeparam>
        /// <typeparam name="TOrderBy">排序欄位類型</typeparam>
        /// <param name="where">過濾條件，需要用到類型轉換的需要提前處理與資料表一致的</param>
        /// <param name="orderby">排序欄位</param>
        /// <param name="selector">返回結果（必須是模型中存在的欄位）</param>
        /// <param name="IsAsc">排序方向，true為正序false為倒序</param>
        /// <returns>動態類</returns>
        Task<dynamic> QueryDynamicAsync<TEntity, TOrderBy>(Expression<Func<TEntity, bool>> where, Expression<Func<TEntity, TOrderBy>> orderby, Func<IQueryable<TEntity>, List<object>> selector, bool IsAsc)
            where TEntity : class;

        #endregion

        #region 驗證是否存在

        /// <summary>
        /// 驗證當前條件是否存在相同項
        /// </summary>
        bool IsExist(Expression<Func<T, bool>> predicate);
        /// <summary>
        /// 驗證當前條件是否存在相同項（非同步方式）
        /// </summary>
        Task<bool> IsExistAsync(Expression<Func<T, bool>> predicate);

        /// <summary>
        /// 根據SQL驗證實體物件是否存在
        /// </summary>
        bool IsExist(string sql, params DbParameter[] para);
        /// <summary>
        /// 根據SQL驗證實體物件是否存在（非同步方式）
        /// </summary>
        Task<bool> IsExistAsync(string sql, params DbParameter[] para);

        #endregion

        #region 分頁查詢
        /// <summary>
        /// 通用EF分頁，預設顯示20條記錄
        /// </summary>
        /// <typeparam name="TEntity">實體模型</typeparam>
        /// <typeparam name="TOrderBy">排序類型</typeparam>
        /// <param name="index">當前頁</param>
        /// <param name="pageSize">顯示條數</param>
        /// <param name="where">過濾條件</param>
        /// <param name="orderby">排序欄位</param>
        /// <param name="selector">結果集合</param>
        /// <param name="isAsc">排序方向true正序 false倒序</param>
        /// <returns>自訂實體集合</returns>
        PageInfo<object> Query<TEntity, TOrderBy>
            (int index, int pageSize,
            Expression<Func<TEntity, bool>> where,
            Expression<Func<TEntity, TOrderBy>> orderby,
            Func<IQueryable<TEntity>, List<object>> selector,
            bool IsAsc)
            where TEntity : class;
        /// <summary>
        /// 對IQueryable物件進行分頁邏輯處理，過濾、查詢項、排序對IQueryable操作
        /// </summary>
        /// <param name="t">Iqueryable</param>
        /// <param name="index">當前頁</param>
        /// <param name="PageSize">每頁顯示多少條</param>
        /// <returns>當前IQueryable to List的物件</returns>
        Common.PageInfo<T> Query(IQueryable<T> query, int index, int PageSize);
        /// <summary>
        /// 普通SQL查詢分頁方法
        /// </summary>
        /// <param name="index">當前頁</param>
        /// <param name="pageSize">顯示行數</param>
        /// <param name="tableName">表名/視圖</param>
        /// <param name="field">獲取項</param>
        /// <param name="filter">過濾條件</param>
        /// <param name="orderby">排序欄位+排序方向</param>
        /// <param name="group">分組欄位</param>
        /// <returns>結果集</returns>
        Common.PageInfo Query(int index, int pageSize, string tableName, string field, string filter, string orderby, string group, params DbParameter[] para);
        /// <summary>
        /// 簡單的Sql查詢分頁
        /// </summary>
        /// <param name="index"></param>
        /// <param name="pageSize"></param>
        /// <param name="sql"></param>
        /// <returns></returns>
        Common.PageInfo Query(int index, int pageSize, string sql, string orderby, params DbParameter[] para);
        /// <summary>
        /// 多表聯合分頁演算法
        /// </summary>
        PageInfo Query(IQueryable query, int index, int pagesize);
        #endregion

        #region ADO.NET增刪改查方法
        /// <summary>
        /// 執行增刪改方法,含交易處理
        /// </summary>
        object ExecuteSqlCommand(string sql, params DbParameter[] para);
        /// <summary>
        /// 執行多條SQL，增刪改方法,含交易處理
        /// </summary>
        object ExecuteSqlCommand(Dictionary<string, object> sqllist);
        /// <summary>
        /// 執行查詢方法,返回動態類，接收使用var，遍歷時使用dynamic類型
        /// </summary>
        object ExecuteSqlQuery(string sql, params DbParameter[] para);
        #endregion

        #region 更新操作
        /// <summary>
        /// 更新欄位
        /// </summary>
        /// <param name="table">表名</param>
        /// <param name="dic">被解析的欄位</param>
        /// <param name="where">條件</param>
        /// <returns></returns>
        bool Modify(string table, Dictionary<string, object> dic, string where);
        #endregion
    }
}