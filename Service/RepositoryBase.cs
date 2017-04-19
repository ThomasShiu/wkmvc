using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.Common;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using Domain;
using System.Linq.Expressions;
using System.Collections;
using System.Threading.Tasks;
using Common.JsonHelper;

namespace Service
{
    /// <summary>
    /// 資料操作基本實現類，公用實現方法
    /// add yuangang by 2015-05-10
    /// </summary>
    /// <typeparam name="T">具體操作的實體模型</typeparam>
    public abstract class RepositoryBase<T> : IRepository<T> where T : class
    {
        #region 固定公用説明，含事務

        private DbContext context = new MyConfig().db;
        /// <summary>
        /// 數據上下文
        /// </summary>
        public DbContext _Context
        {
            get
            {
                context.Configuration.ValidateOnSaveEnabled = false;
                return context;
            }
        }
        /// <summary>
        /// 資料上下文--->拓展屬性
        /// </summary>
        public MyConfig Config
        {
            get
            {
                return new MyConfig();
            }
        }
        #endregion

        #region 單模型 CRUD 操作
        /// <summary>
        /// 增加一條記錄
        /// </summary>
        /// <param name="entity">實體模型</param>
        /// <param name="IsCommit">是否提交（默認提交）</param>
        /// <returns></returns>
        public virtual bool Save(T entity, bool IsCommit = true)
        {
            _Context.Set<T>().Add(entity);
            if (IsCommit)
                return _Context.SaveChanges() > 0;
            else
                return false;
        }
        /// <summary>
        /// 增加一條記錄（非同步方式）
        /// </summary>
        /// <param name="entity">實體模型</param>
        /// <param name="IsCommit">是否提交（默認提交）</param>
        /// <returns></returns>
        public virtual async Task<bool> SaveAsync(T entity, bool IsCommit = true)
        {
            _Context.Set<T>().Add(entity);
            if (IsCommit)
                return await Task.Run(() => _Context.SaveChanges() > 0);
            else
                return await Task.Run(() => false);
        }

        /// <summary>
        /// 更新一條記錄
        /// </summary>
        /// <param name="entity">實體模型</param>
        /// <param name="IsCommit">是否提交（默認提交）</param>
        /// <returns></returns>
        public virtual bool Update(T entity, bool IsCommit = true)
        {
            _Context.Set<T>().Attach(entity);
            _Context.Entry<T>(entity).State = System.Data.Entity.EntityState.Modified;
            if (IsCommit)
                return _Context.SaveChanges() > 0;
            else
                return false;
        }
        /// <summary>
        /// 更新一條記錄（非同步方式）
        /// </summary>
        /// <param name="entity">實體模型</param>
        /// <param name="IsCommit">是否提交（默認提交）</param>
        /// <returns></returns>
        public virtual async Task<bool> UpdateAsync(T entity, bool IsCommit = true)
        {
            _Context.Set<T>().Attach(entity);
            _Context.Entry<T>(entity).State = System.Data.Entity.EntityState.Modified;
            if (IsCommit)
                return await Task.Run(() => _Context.SaveChanges() > 0);
            else
                return await Task.Run(() => false);
        }

        /// <summary>
        /// 增加或更新一條記錄
        /// </summary>
        /// <param name="entity">實體模型</param>
        /// <param name="IsSave">是否增加</param>
        /// <param name="IsCommit">是否提交（默認提交）</param>
        /// <returns></returns>
        public virtual bool SaveOrUpdate(T entity, bool isEdit, bool IsCommit = true)
        {
            return isEdit ? Update(entity, IsCommit) : Save(entity, IsCommit);
        }
        /// <summary>
        /// 增加或更新一條記錄（非同步方式）
        /// </summary>
        /// <param name="entity">實體模型</param>
        /// <param name="IsSave">是否增加</param>
        /// <param name="IsCommit">是否提交（默認提交）</param>
        /// <returns></returns>
        public virtual async Task<bool> SaveOrUpdateAsync(T entity, bool isEdit, bool IsCommit = true)
        {
            return isEdit ? await UpdateAsync(entity, IsCommit) : await SaveAsync(entity, IsCommit);
        }

        /// <summary>
        /// 通過Lamda運算式獲取實體
        /// </summary>
        /// <param name="predicate">Lamda運算式（p=>p.Id==Id）</param>
        /// <returns></returns>
        public virtual T Get(Expression<Func<T, bool>> predicate)
        {
            return _Context.Set<T>().AsNoTracking().SingleOrDefault(predicate);
        }
        /// <summary>
        /// 通過Lamda運算式獲取實體（非同步方式）
        /// </summary>
        /// <param name="predicate">Lamda運算式（p=>p.Id==Id）</param>
        /// <returns></returns>
        public virtual async Task<T> GetAsync(Expression<Func<T, bool>> predicate)
        {
            return await Task.Run(() => _Context.Set<T>().AsNoTracking().SingleOrDefault(predicate));
        }

        /// <summary>
        /// 刪除一條記錄
        /// </summary>
        /// <param name="entity">實體模型</param>
        /// <param name="IsCommit">是否提交（默認提交）</param>
        /// <returns></returns>
        public virtual bool Delete(T entity, bool IsCommit = true)
        {
            if (entity == null) return false;
            _Context.Set<T>().Attach(entity);
            _Context.Set<T>().Remove(entity);

            if (IsCommit)
                return _Context.SaveChanges() > 0;
            else
                return false;
        }
        /// <summary>
        /// 刪除一條記錄（非同步方式）
        /// </summary>
        /// <param name="entity">實體模型</param>
        /// <param name="IsCommit">是否提交（默認提交）</param>
        /// <returns></returns>
        public virtual async Task<bool> DeleteAsync(T entity, bool IsCommit = true)
        {
            if (entity == null) return await Task.Run(() => false);
            _Context.Set<T>().Attach(entity);
            _Context.Set<T>().Remove(entity);
            if (IsCommit)
                return await Task.Run(() => _Context.SaveChanges() > 0);
            else
                return await Task.Run(() => false); ;
        }

        #endregion

        #region 多模型操作
        /// <summary>
        /// 增加多條記錄，同一模型
        /// </summary>
        /// <param name="T1">實體模型集合</param>
        /// <param name="IsCommit">是否提交（默認提交）</param>
        /// <returns></returns>
        public virtual bool SaveList(List<T> T1, bool IsCommit = true)
        {
            if (T1 == null || T1.Count == 0) return false;

            T1.ToList().ForEach(item =>
            {
                _Context.Set<T>().Add(item);
            });

            if (IsCommit)
                return _Context.SaveChanges() > 0;
            else
                return false;
        }
        /// <summary>
        /// 增加多條記錄，同一模型（非同步方式）
        /// </summary>
        /// <param name="T1">實體模型集合</param>
        /// <param name="IsCommit">是否提交（默認提交）</param>
        /// <returns></returns>
        public virtual async Task<bool> SaveListAsync(List<T> T1, bool IsCommit = true)
        {
            if (T1 == null || T1.Count == 0) return await Task.Run(() => false);

            T1.ToList().ForEach(item =>
            {
                _Context.Set<T>().Add(item);
            });

            if (IsCommit)
                return await Task.Run(() => _Context.SaveChanges() > 0);
            else
                return await Task.Run(() => false);
        }

        /// <summary>
        /// 增加多條記錄，獨立模型
        /// </summary>
        /// <param name="T1">實體模型集合</param>
        /// <param name="IsCommit">是否提交（默認提交）</param>
        /// <returns></returns>
        public virtual bool SaveList<T1>(List<T1> T, bool IsCommit = true) where T1 : class
        {
            if (T == null || T.Count == 0) return false;
            _Context.Set<T1>().Local.Clear();
            T.ToList().ForEach(item =>
            {
                _Context.Set<T1>().Add(item);
            });
            if (IsCommit)
                return _Context.SaveChanges() > 0;
            else
                return false;
        }
        /// <summary>
        /// 增加多條記錄，獨立模型（非同步方式）
        /// </summary>
        /// <param name="T1">實體模型集合</param>
        /// <param name="IsCommit">是否提交（默認提交）</param>
        /// <returns></returns>
        public virtual async Task<bool> SaveListAsync<T1>(List<T1> T, bool IsCommit = true) where T1 : class
        {
            if (T == null || T.Count == 0) return await Task.Run(() => false);
            _Context.Set<T1>().Local.Clear();
            T.ToList().ForEach(item =>
            {
                _Context.Set<T1>().Add(item);
            });
            if (IsCommit)
                return await Task.Run(() => _Context.SaveChanges() > 0);
            else
                return await Task.Run(() => false);
        }

        /// <summary>
        /// 更新多條記錄，同一模型
        /// </summary>
        /// <param name="T1">實體模型集合</param>
        /// <param name="IsCommit">是否提交（默認提交）</param>
        /// <returns></returns>
        public virtual bool UpdateList(List<T> T1, bool IsCommit = true)
        {
            if (T1 == null || T1.Count == 0) return false;

            T1.ToList().ForEach(item =>
            {
                _Context.Set<T>().Attach(item);
                _Context.Entry<T>(item).State = System.Data.Entity.EntityState.Modified;
            });

            if (IsCommit)
                return _Context.SaveChanges() > 0;
            else
                return false;
        }
        /// <summary>
        /// 更新多條記錄，同一模型（非同步方式）
        /// </summary>
        /// <param name="T1">實體模型集合</param>
        /// <param name="IsCommit">是否提交（默認提交）</param>
        /// <returns></returns>
        public virtual async Task<bool> UpdateListAsync(List<T> T1, bool IsCommit = true)
        {
            if (T1 == null || T1.Count == 0) return await Task.Run(() => false);

            T1.ToList().ForEach(item =>
            {
                _Context.Set<T>().Attach(item);
                _Context.Entry<T>(item).State = System.Data.Entity.EntityState.Modified;
            });

            if (IsCommit)
                return await Task.Run(() => _Context.SaveChanges() > 0);
            else
                return await Task.Run(() => false);
        }

        /// <summary>
        /// 更新多條記錄，獨立模型
        /// </summary>
        /// <param name="T1">實體模型集合</param>
        /// <param name="IsCommit">是否提交（默認提交）</param>
        /// <returns></returns>
        public virtual bool UpdateList<T1>(List<T1> T, bool IsCommit = true) where T1 : class
        {
            if (T == null || T.Count == 0) return false;

            T.ToList().ForEach(item =>
            {
                _Context.Set<T1>().Attach(item);
                _Context.Entry<T1>(item).State = System.Data.Entity.EntityState.Modified;
            });

            if (IsCommit)
                return _Context.SaveChanges() > 0;
            else
                return false;
        }
        /// <summary>
        /// 更新多條記錄，獨立模型（非同步方式）
        /// </summary>
        /// <param name="T1">實體模型集合</param>
        /// <param name="IsCommit">是否提交（默認提交）</param>
        /// <returns></returns>
        public virtual async Task<bool> UpdateListAsync<T1>(List<T1> T, bool IsCommit = true) where T1 : class
        {
            if (T == null || T.Count == 0) return await Task.Run(() => false);

            T.ToList().ForEach(item =>
            {
                _Context.Set<T1>().Attach(item);
                _Context.Entry<T1>(item).State = System.Data.Entity.EntityState.Modified;
            });

            if (IsCommit)
                return await Task.Run(() => _Context.SaveChanges() > 0);
            else
                return await Task.Run(() => false);
        }

        /// <summary>
        /// 刪除多條記錄，同一模型
        /// </summary>
        /// <param name="T1">實體模型集合</param>
        /// <param name="IsCommit">是否提交（默認提交）</param>
        /// <returns></returns>
        public virtual bool DeleteList(List<T> T1, bool IsCommit = true)
        {
            if (T1 == null || T1.Count == 0) return false;

            T1.ToList().ForEach(item =>
            {
                _Context.Set<T>().Attach(item);
                _Context.Set<T>().Remove(item);
            });

            if (IsCommit)
                return _Context.SaveChanges() > 0;
            else
                return false;
        }
        /// <summary>
        /// 刪除多條記錄，同一模型（非同步方式）
        /// </summary>
        /// <param name="T1">實體模型集合</param>
        /// <param name="IsCommit">是否提交（默認提交）</param>
        /// <returns></returns>
        public virtual async Task<bool> DeleteListAsync(List<T> T1, bool IsCommit = true)
        {
            if (T1 == null || T1.Count == 0) return await Task.Run(() => false);

            T1.ToList().ForEach(item =>
            {
                _Context.Set<T>().Attach(item);
                _Context.Set<T>().Remove(item);
            });

            if (IsCommit)
                return await Task.Run(() => _Context.SaveChanges() > 0);
            else
                return await Task.Run(() => false);
        }

        /// <summary>
        /// 刪除多條記錄，獨立模型
        /// </summary>
        /// <param name="T1">實體模型集合</param>
        /// <param name="IsCommit">是否提交（默認提交）</param>
        /// <returns></returns>
        public virtual bool DeleteList<T1>(List<T1> T, bool IsCommit = true) where T1 : class
        {
            if (T == null || T.Count == 0) return false;

            T.ToList().ForEach(item =>
            {
                _Context.Set<T1>().Attach(item);
                _Context.Set<T1>().Remove(item);
            });

            if (IsCommit)
                return _Context.SaveChanges() > 0;
            else
                return false;
        }
        /// <summary>
        /// 刪除多條記錄，獨立模型（非同步方式）
        /// </summary>
        /// <param name="T1">實體模型集合</param>
        /// <param name="IsCommit">是否提交（默認提交）</param>
        /// <returns></returns>
        public virtual async Task<bool> DeleteListAsync<T1>(List<T1> T, bool IsCommit = true) where T1 : class
        {
            if (T == null || T.Count == 0) return await Task.Run(() => false);

            T.ToList().ForEach(item =>
            {
                _Context.Set<T1>().Attach(item);
                _Context.Set<T1>().Remove(item);
            });

            if (IsCommit)
                return await Task.Run(() => _Context.SaveChanges() > 0);
            else
                return await Task.Run(() => false);
        }

        /// <summary>
        /// 通過Lamda運算式，刪除一條或多條記錄
        /// </summary>
        /// <param name="predicate"></param>
        /// <param name="IsCommit"></param>
        /// <returns></returns>
        public virtual bool Delete(Expression<Func<T, bool>> predicate, bool IsCommit = true)
        {
            IQueryable<T> entry = (predicate == null) ? _Context.Set<T>().AsQueryable() : _Context.Set<T>().Where(predicate);
            List<T> list = entry.ToList();

            if (list != null && list.Count == 0) return false;
            list.ForEach(item =>
            {
                _Context.Set<T>().Attach(item);
                _Context.Set<T>().Remove(item);
            });

            if (IsCommit)
                return _Context.SaveChanges() > 0;
            else
                return false;
        }
        /// <summary>
        /// 通過Lamda運算式，刪除一條或多條記錄（非同步方式）
        /// </summary>
        /// <param name="predicate"></param>
        /// <param name="IsCommit"></param>
        /// <returns></returns>
        public virtual async Task<bool> DeleteAsync(Expression<Func<T, bool>> predicate, bool IsCommit = true)
        {
            IQueryable<T> entry = (predicate == null) ? _Context.Set<T>().AsQueryable() : _Context.Set<T>().Where(predicate);
            List<T> list = entry.ToList();

            if (list != null && list.Count == 0) return await Task.Run(() => false);
            list.ForEach(item =>
            {
                _Context.Set<T>().Attach(item);
                _Context.Set<T>().Remove(item);
            });

            if (IsCommit)
                return await Task.Run(() => _Context.SaveChanges() > 0);
            else
                return await Task.Run(() => false);
        }

        /// <summary>
        /// 執行SQL刪除
        /// </summary>
        /// <param name="sql">SQL語句</param>
        /// <param name="para">Parameters參數</param>
        public virtual int DeleteBySql(string sql, params DbParameter[] para)
        {
            return _Context.Database.ExecuteSqlCommand(sql, para);
        }
        /// <summary>
        /// 執行SQL刪除（非同步方式）
        /// </summary>
        /// <param name="sql">SQL語句</param>
        /// <param name="para">Parameters參數</param>
        public virtual async Task<int> DeleteBySqlAsync(string sql, params DbParameter[] para)
        {
            return await Task.Run(() => _Context.Database.ExecuteSqlCommand(sql, para));
        }
        #endregion

        #region 獲取多條資料操作

        /// <summary>
        /// 返回IQueryable集合，延時載入資料
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public virtual IQueryable<T> LoadAll(Expression<Func<T, bool>> predicate)
        {
            return predicate != null ? _Context.Set<T>().Where(predicate).AsNoTracking<T>() : _Context.Set<T>().AsQueryable<T>().AsNoTracking<T>();
        }
        /// <summary>
        /// 返回IQueryable集合，延時載入資料（非同步方式）
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public virtual async Task<IQueryable<T>> LoadAllAsync(Expression<Func<T, bool>> predicate)
        {
            return predicate != null ? await Task.Run(() => _Context.Set<T>().Where(predicate).AsNoTracking<T>()) : await Task.Run(() => _Context.Set<T>().AsQueryable<T>().AsNoTracking<T>());
        }

        // <summary>
        /// 返回List<T>集合,不採用延時載入
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public virtual List<T> LoadListAll(Expression<Func<T, bool>> predicate)
        {
            return predicate != null ? _Context.Set<T>().Where(predicate).AsNoTracking().ToList() : _Context.Set<T>().AsQueryable<T>().AsNoTracking().ToList();
        }
        // <summary>
        /// 返回List<T>集合,不採用延時載入（非同步方式）
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public virtual async Task<List<T>> LoadListAllAsync(Expression<Func<T, bool>> predicate)
        {
            return predicate != null ? await Task.Run(() => _Context.Set<T>().Where(predicate).AsNoTracking().ToList()) : await Task.Run(() => _Context.Set<T>().AsQueryable<T>().AsNoTracking().ToList());
        }

        /// <summary>
        /// 獲取DbQuery的列表
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public virtual DbQuery<T> LoadQueryAll(Expression<Func<T, bool>> predicate)
        {
            return predicate != null ? _Context.Set<T>().Where(predicate) as DbQuery<T> : _Context.Set<T>();
        }
        /// <summary>
        /// 獲取DbQuery的清單（非同步方式）
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public virtual async Task<DbQuery<T>> LoadQueryAllAsync(Expression<Func<T, bool>> predicate)
        {
            return predicate != null ? await Task.Run(() => _Context.Set<T>().Where(predicate) as DbQuery<T>) : await Task.Run(() => _Context.Set<T>());
        }

        /// <summary>
        /// 獲取IEnumerable列表
        /// </summary>
        /// <param name="sql">SQL語句</param>
        /// <param name="para">Parameters參數</param>
        /// <returns></returns>
        public virtual IEnumerable<T> LoadEnumerableAll(string sql, params DbParameter[] para)
        {
            return _Context.Database.SqlQuery<T>(sql, para);
        }
        /// <summary>
        /// 獲取IEnumerable清單（非同步方式）
        /// </summary>
        /// <param name="sql">SQL語句</param>
        /// <param name="para">Parameters參數</param>
        /// <returns></returns>
        public virtual async Task<IEnumerable<T>> LoadEnumerableAllAsync(string sql, params DbParameter[] para)
        {
            return await Task.Run(() => _Context.Database.SqlQuery<T>(sql, para));
        }

        /// <summary>
        /// 獲取資料動態集合
        /// </summary>
        /// <param name="sql">SQL語句</param>
        /// <param name="para">Parameters參數</param>
        /// <returns></returns>
        public virtual IEnumerable LoadEnumerable(string sql, params DbParameter[] para)
        {
            return _Context.Database.SqlQueryForDynamic(sql, para);
        }
        /// <summary>
        /// 獲取資料動態集合（非同步方式）
        /// </summary>
        /// <param name="sql">SQL語句</param>
        /// <param name="para">Parameters參數</param>
        /// <returns></returns>
        public virtual async Task<IEnumerable> LoadEnumerableAsync(string sql, params DbParameter[] para)
        {
            return await Task.Run(() => _Context.Database.SqlQueryForDynamic(sql, para));
        }

        /// <summary>
        /// 採用SQL進行資料的查詢，返回IList集合
        /// </summary>
        /// <param name="sql">SQL語句</param>
        /// <param name="para">Parameters參數</param>
        /// <returns></returns>
        public virtual List<T> SelectBySql(string sql, params DbParameter[] para)
        {
            return _Context.Database.SqlQuery(typeof(T), sql, para).Cast<T>().ToList();
        }
        /// <summary>
        /// 採用SQL進行資料的查詢，返回IList集合（非同步方式）
        /// </summary>
        /// <param name="sql">SQL語句</param>
        /// <param name="para">Parameters參數</param>
        /// <returns></returns>
        public virtual async Task<List<T>> SelectBySqlAsync(string sql, params DbParameter[] para)
        {
            return await Task.Run(() => _Context.Database.SqlQuery(typeof(T), sql, para).Cast<T>().ToList());
        }

        /// <summary>
        /// 採用SQL進行資料的查詢，指定泛型，返回IList集合
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <param name="sql"></param>
        /// <param name="para"></param>
        /// <returns></returns>
        public virtual List<T1> SelectBySql<T1>(string sql, params DbParameter[] para)
        {
            return _Context.Database.SqlQuery<T1>(sql, para).ToList();
        }
        /// <summary>
        /// 採用SQL進行資料的查詢，指定泛型，返回IList集合
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <param name="sql"></param>
        /// <param name="para"></param>
        /// <returns></returns>
        public virtual async Task<List<T1>> SelectBySqlAsync<T1>(string sql, params DbParameter[] para)
        {
            return await Task.Run(() => _Context.Database.SqlQuery<T1>(sql, para).ToList());
        }

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
        public virtual List<TResult> QueryEntity<TEntity, TOrderBy, TResult>
            (Expression<Func<TEntity, bool>> where,
            Expression<Func<TEntity, TOrderBy>> orderby,
            Expression<Func<TEntity, TResult>> selector,
            bool IsAsc)
            where TEntity : class
            where TResult : class
        {
            IQueryable<TEntity> query = _Context.Set<TEntity>();
            if (where != null)
            {
                query = query.Where(where);
            }

            if (orderby != null)
            {
                query = IsAsc ? query.OrderBy(orderby) : query.OrderByDescending(orderby);
            }
            if (selector == null)
            {
                return query.Cast<TResult>().AsNoTracking().ToList();
            }
            return query.Select(selector).AsNoTracking().ToList();
        }
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
        public virtual async Task<List<TResult>> QueryEntityAsync<TEntity, TOrderBy, TResult>(Expression<Func<TEntity, bool>> where, Expression<Func<TEntity, TOrderBy>> orderby, Expression<Func<TEntity, TResult>> selector, bool IsAsc)
            where TEntity : class
            where TResult : class
        {
            IQueryable<TEntity> query = _Context.Set<TEntity>();
            if (where != null)
            {
                query = query.Where(where);
            }

            if (orderby != null)
            {
                query = IsAsc ? query.OrderBy(orderby) : query.OrderByDescending(orderby);
            }
            if (selector == null)
            {
                return await Task.Run(() => query.Cast<TResult>().AsNoTracking().ToList());
            }
            return await Task.Run(() => query.Select(selector).AsNoTracking().ToList());
        }

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
        public virtual List<object> QueryObject<TEntity, TOrderBy>(Expression<Func<TEntity, bool>> where, Expression<Func<TEntity, TOrderBy>> orderby, Func<IQueryable<TEntity>, List<object>> selector, bool IsAsc)
            where TEntity : class
        {
            IQueryable<TEntity> query = _Context.Set<TEntity>();
            if (where != null)
            {
                query = query.Where(where);
            }

            if (orderby != null)
            {
                query = IsAsc ? query.OrderBy(orderby) : query.OrderByDescending(orderby);
            }
            if (selector == null)
            {
                return query.AsNoTracking().ToList<object>();
            }
            return selector(query);
        }
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
        public virtual async Task<List<object>> QueryObjectAsync<TEntity, TOrderBy>(Expression<Func<TEntity, bool>> where, Expression<Func<TEntity, TOrderBy>> orderby, Func<IQueryable<TEntity>, List<object>> selector, bool IsAsc)
            where TEntity : class
        {
            IQueryable<TEntity> query = _Context.Set<TEntity>();
            if (where != null)
            {
                query = query.Where(where);
            }

            if (orderby != null)
            {
                query = IsAsc ? query.OrderBy(orderby) : query.OrderByDescending(orderby);
            }
            if (selector == null)
            {
                return await Task.Run(() => query.AsNoTracking().ToList<object>());
            }
            return await Task.Run(() => selector(query));
        }

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
        public dynamic QueryDynamic<TEntity, TOrderBy>(Expression<Func<TEntity, bool>> where, Expression<Func<TEntity, TOrderBy>> orderby, Func<IQueryable<TEntity>, List<object>> selector, bool IsAsc)
            where TEntity : class
        {
            List<object> list = QueryObject<TEntity, TOrderBy>
                 (where, orderby, selector, IsAsc);
            return JsonConverter.JsonClass(list);
        }
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
        public virtual async Task<dynamic> QueryDynamicAsync<TEntity, TOrderBy>(Expression<Func<TEntity, bool>> where, Expression<Func<TEntity, TOrderBy>> orderby, Func<IQueryable<TEntity>, List<object>> selector, bool IsAsc)
            where TEntity : class
        {
            List<object> list = QueryObject<TEntity, TOrderBy>
                 (where, orderby, selector, IsAsc);
            return await Task.Run(() => JsonConverter.JsonClass(list));
        }

        #endregion

        #region 驗證是否存在

        /// <summary>
        /// 驗證當前條件是否存在相同項
        /// </summary>
        public virtual bool IsExist(Expression<Func<T, bool>> predicate)
        {
            var entry = _Context.Set<T>().Where(predicate);
            return (entry.Any());
        }
        /// <summary>
        /// 驗證當前條件是否存在相同項（非同步方式）
        /// </summary>
        public virtual async Task<bool> IsExistAsync(Expression<Func<T, bool>> predicate)
        {
            var entry = _Context.Set<T>().Where(predicate);
            return await Task.Run(() => entry.Any());
        }

        /// <summary>
        /// 根據SQL驗證實體物件是否存在
        /// </summary>
        public virtual bool IsExist(string sql, params DbParameter[] para)
        {
            IEnumerable result = _Context.Database.SqlQuery(typeof(int), sql, para);

            if (result.GetEnumerator().Current == null || result.GetEnumerator().Current.ToString() == "0")
                return false;
            return true;
        }
        /// <summary>
        /// 根據SQL驗證實體物件是否存在（非同步方式）
        /// </summary>
        public virtual async Task<bool> IsExistAsync(string sql, params DbParameter[] para)
        {
            IEnumerable result = _Context.Database.SqlQuery(typeof(int), sql, para);

            if (result.GetEnumerator().Current == null || result.GetEnumerator().Current.ToString() == "0")
                return await Task.Run(() => false);
            return await Task.Run(() => true);
        }

        #endregion

        #region 存儲過程操作
        /// <summary>
        /// 執行返回影響行數的存儲過程
        /// </summary>
        /// <param name="procname">過程名稱</param>
        /// <param name="parameter">參數對象</param>
        /// <returns></returns>
        public virtual object ExecuteProc(string procname, params DbParameter[] parameter)
        {
            return ExecuteSqlCommand(procname, parameter);
        }
        /// <summary>
        /// 執行返回結果集的存儲過程
        /// </summary>
        /// <param name="procname">過程名稱</param>
        /// <param name="parameter">參數對象</param>
        /// <returns></returns>
        public virtual object ExecuteQueryProc(string procname, params DbParameter[] parameter)
        {
            return _Context.Database.SqlFunctionForDynamic(procname, parameter);
        }
        #endregion

        #region 分頁操作
        /// <summary>
        /// 對IQueryable物件進行分頁邏輯處理，過濾、查詢項、排序對IQueryable操作
        /// </summary>
        /// <param name="t">Iqueryable</param>
        /// <param name="index">當前頁</param>
        /// <param name="PageSize">每頁顯示多少條</param>
        /// <returns>當前IQueryable to List的物件</returns>
        public virtual Common.PageInfo<T> Query(IQueryable<T> query, int index, int PageSize)
        {
            if (index < 1)
            {
                index = 1;
            }
            if (PageSize <= 0)
            {
                PageSize = 20;
            }
            int count = query.Count();

            int maxpage = count / PageSize;

            if (count % PageSize > 0)
            {
                maxpage++;
            }
            if (index > maxpage)
            {
                index = maxpage;
            }
            if (count > 0)
                query = query.Skip((index - 1) * PageSize).Take(PageSize);
            return new Common.PageInfo<T>(index, PageSize, count, query.ToList());
        }
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
        public virtual Common.PageInfo<object> Query<TEntity, TOrderBy>
            (int index, int pageSize,
            Expression<Func<TEntity, bool>> where,
            Expression<Func<TEntity, TOrderBy>> orderby,
            Func<IQueryable<TEntity>,
            List<object>> selector,
            bool isAsc)
            where TEntity : class
        {
            if (index < 1)
            {
                index = 1;
            }

            if (pageSize <= 0)
            {
                pageSize = 20;
            }

            IQueryable<TEntity> query = _Context.Set<TEntity>();
            if (where != null)
            {
                query = query.Where(where);
            }
            int count = query.Count();

            int maxpage = count / pageSize;

            if (count % pageSize > 0)
            {
                maxpage++;
            }
            if (index > maxpage)
            {
                index = maxpage;
            }

            if (orderby != null)
            {
                query = isAsc ? query.OrderBy(orderby) : query.OrderByDescending(orderby);
            }
            if (count > 0)
                query = query.Skip((index - 1) * pageSize).Take(pageSize);
            //返回結果為null，返回所有欄位
            if (selector == null)
                return new Common.PageInfo<object>(index, pageSize, count, query.ToList<object>());
            return new Common.PageInfo<object>(index, pageSize, count, selector(query).ToList());
        }
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
        public virtual Common.PageInfo Query(int index, int pageSize, string tableName, string field, string filter, string orderby, string group, params DbParameter[] para)
        {
            //執行分頁演算法
            if (index <= 0)
                index = 1;
            int start = (index - 1) * pageSize;
            if (start > 0)
                start -= 1;
            else
                start = 0;
            int end = index * pageSize;

            #region 查詢邏輯
            string logicSql = "SELECT";
            //查詢項
            if (!string.IsNullOrEmpty(field))
            {
                logicSql += " " + field;
            }
            else
            {
                logicSql += " *";
            }
            logicSql += " FROM (" + tableName + " ) where";
            //過濾條件
            if (!string.IsNullOrEmpty(filter))
            {
                logicSql += " " + filter;
            }
            else
            {
                filter = " 1=1";
                logicSql += "  1=1";
            }
            //分組
            if (!string.IsNullOrEmpty(group))
            {
                logicSql += " group by " + group;
            }

            #endregion

            //獲取當前條件下資料總條數
            int count = _Context.Database.SqlQuery(typeof(int), "select count(*) from (" + tableName + ") where " + filter, para).Cast<int>().FirstOrDefault();
            string sql = "SELECT T.* FROM ( SELECT B.* FROM ( SELECT A.*,ROW_NUMBER() OVER(ORDER BY getdate()) as RN" +
                         logicSql + ") A ) B WHERE B.RN<=" + end + ") T WHERE T.RN>" + start;
            //排序
            if (!string.IsNullOrEmpty(orderby))
            {
                sql += " order by " + orderby;
            }
            var list = ExecuteSqlQuery(sql, para) as IEnumerable;
            if (list != null)
                return new Common.PageInfo(index, pageSize, count, list);
            return new Common.PageInfo(index, pageSize, count, new { });
        }

        /// <summary>
        /// 最簡單的SQL分頁
        /// </summary>
        /// <param name="index">頁碼</param>
        /// <param name="pageSize">顯示行數</param>
        /// <param name="sql">純SQL語句</param>
        /// <param name="orderby">排序欄位與方向</param>
        /// <returns></returns>
        public virtual Common.PageInfo Query(int index, int pageSize, string sql, string orderby, params DbParameter[] para)
        {
            return this.Query(index, pageSize, sql, null, null, orderby, null, para);
        }
        /// <summary>
        /// 多表聯合分頁演算法
        /// </summary>
        public virtual Common.PageInfo Query(IQueryable query, int index, int PageSize)
        {
            var enumerable = (query as System.Collections.IEnumerable).Cast<object>();
            if (index < 1)
            {
                index = 1;
            }
            if (PageSize <= 0)
            {
                PageSize = 20;
            }

            int count = enumerable.Count();

            int maxpage = count / PageSize;

            if (count % PageSize > 0)
            {
                maxpage++;
            }
            if (index > maxpage)
            {
                index = maxpage;
            }
            if (count > 0)
                enumerable = enumerable.Skip((index - 1) * PageSize).Take(PageSize);
            return new Common.PageInfo(index, PageSize, count, JsonConverter.JsonClass(enumerable.ToList()));
        }
        #endregion

        #region ADO.NET增刪改查方法
        /// <summary>
        /// 執行增刪改方法,含交易處理
        /// </summary>
        public virtual object ExecuteSqlCommand(string sql, params DbParameter[] para)
        {
            return _Context.Database.ExecuteSqlCommand(sql, para);

        }
        /// <summary>
        /// 執行多條SQL，增刪改方法,含交易處理
        /// </summary>
        public virtual object ExecuteSqlCommand(Dictionary<string, object> sqllist)
        {
            int rows = 0;
            IEnumerator<KeyValuePair<string, object>> enumerator = sqllist.GetEnumerator();

            while (enumerator.MoveNext())
            {
                rows += _Context.Database.ExecuteSqlCommand(enumerator.Current.Key, enumerator.Current.Value);
            }
            return rows;

        }
        /// <summary>
        /// 執行查詢方法,返回動態類，接收使用var，遍歷時使用dynamic類型
        /// </summary>
        public virtual object ExecuteSqlQuery(string sql, params DbParameter[] para)
        {
            return _Context.Database.SqlQueryForDynamic(sql, para);
        }

        #endregion

        #region 更新操作
        /// <summary>
        /// 更新欄位
        /// </summary>
        /// <param name="table">表名</param>
        /// <param name="dic">被解析的欄位</param>
        /// <param name="where">條件</param>
        /// <returns></returns>
        public bool Modify(string table, Dictionary<string, object> dic, string where)
        {
            try
            {
                if (dic == null || dic.Count <= 0)
                    return false;
                List<DbParameter> list = new List<DbParameter>();
                //解析字典
                string col = string.Empty;
                //字典反射
                var kv = dic.GetEnumerator();
                while (kv.MoveNext())
                {
                    var current = kv.Current;
                    if (!string.IsNullOrEmpty(current.Key) && current.Value != null)
                    {
                        col += current.Key.ToLower() + "=@" + current.Key.ToLower() + ",";
                        list.Add(new System.Data.SqlClient.SqlParameter("@" + current.Key.ToLower(), current.Value));
                    }
                }
                col = col.Trim(',');
                //拼接SQL
                string sql = "update " + table + " set " + col + " where 1=1 " + where;
                //執行
                object obj = this.ExecuteSqlCommand(sql, list.ToArray());
                return obj.ToString() != "0";
            }
            catch (Exception e) { throw e; }
        }
        #endregion

    }
}