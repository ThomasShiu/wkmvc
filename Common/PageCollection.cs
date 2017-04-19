using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Common
{
    /// <summary>
    /// 分頁邏輯處理類
    /// </summary>
    public class PageCollection
    {
        /// <summary>
        /// 總頁數
        /// </summary>
        public int TotalPages { get; set; }
        /// <summary>
        /// 當前頁面
        /// </summary>
        public int CurrentPage { get; set; }
        /// <summary>
        /// 每頁的記錄數
        /// </summary>
        public int OnePageSize { get; set; }
        /// <summary>
        /// 總記錄數
        /// </summary>
        public long TotalRows { get; set; }
        /// <summary>
        /// 排序
        /// </summary>
        public string OrderBy { get; set; }

        /// <summary>
        /// 構造無參默認為最大數
        /// </summary>
        public PageCollection()
        {
            this.CurrentPage = 0;
            this.OnePageSize = 20;//默認最大行數20條
        }
    }
    /// <summary>
    /// 分頁邏輯處理類 linq to entites
    /// </summary>
    public class PageInfo<TEntity> where TEntity : class
    {
        public PageInfo(int index, int pageSize, int count, List<TEntity> list, string url = "")
        {
            Index = index;
            PageSize = pageSize;
            Count = count;
            List = list;
            Url = url;
            //計算資料條數從開始到結束的值
            if (count == 0)
            {
                BeginPage = 0;
                EndPage = 0;
            }
            else
            {
                int maxpage = count / pageSize;

                if (count % pageSize > 0)
                {
                    maxpage++;
                }
                if (index >= maxpage)
                {
                    index = maxpage;

                    BeginPage = pageSize * index - pageSize + 1;
                    EndPage = count;
                }
                else
                {
                    BeginPage = pageSize * index - pageSize + 1;
                    EndPage = pageSize * index;
                }
            }
        }

        public int Index { get; private set; }
        public int PageSize { get; private set; }
        public int Count { get; private set; }
        public List<TEntity> List { get; set; }
        public string Url { get; set; }
        public int BeginPage { get; private set; }
        public int EndPage { get; private set; }
    }

    /// <summary>
    /// 分頁邏輯處理類 dynamic
    /// </summary>
    public class PageInfo
    {
        public PageInfo(int index, int pageSize, int count, dynamic list, string url = "")
        {
            Index = index;
            PageSize = pageSize;
            Count = count;
            List = list;
            Url = url;
            //計算資料條數從開始到結束的值
            if (count == 0)
            {
                BeginPage = 0;
                EndPage = 0;
            }
            else
            {
                int maxpage = count / pageSize;

                if (count % pageSize > 0)
                {
                    maxpage++;
                }
                if (index >= maxpage)
                {
                    index = maxpage;

                    BeginPage = pageSize * index - pageSize + 1;
                    EndPage = count;
                }
                else
                {
                    BeginPage = pageSize * index - pageSize + 1;
                    EndPage = pageSize * index;
                }
            }
        }

        public int Index { get; private set; }
        public int PageSize { get; private set; }
        public int Count { get; private set; }
        public dynamic List { get; private set; }
        public string Url { get; set; }
        public int BeginPage { get; private set; }
        public int EndPage { get; private set; }
    }

    /// <summary>
    /// Eazyui分頁處理邏輯類
    /// </summary>
    public class PageEazyUi
    {
        public PageEazyUi(int _page, int _pagesize, int _total, object _rows)
        {
            page = _page;
            pagesize = _pagesize;
            total = _total;
            rows = _rows;
        }

        public int page { get; private set; }
        public int pagesize { get; private set; }
        public int total { get; private set; }
        public object rows { get; private set; }
    }
}

