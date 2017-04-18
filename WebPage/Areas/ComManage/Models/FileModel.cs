using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebPage.Areas.ComManage.Models
{
    public class FileModel
    {
        /// <summary>
        /// 檔案名稱
        /// </summary>
        public string name { get; set; }
        /// <summary>
        /// 文件全稱
        /// </summary>
        public string fullname { get; set; }
        /// <summary>
        /// 檔路徑
        /// </summary>
        public string path { get; set; }
        /// <summary>
        /// 檔案格式
        /// </summary>
        public string ext { get; set; }
        /// <summary>
        /// 文件大小
        /// </summary>
        public string size { get; set; }
        /// <summary>
        /// 檔圖示
        /// </summary>
        public string icon { get; set; }
        /// <summary>
        /// 是否為資料夾
        /// </summary>
        public bool isfolder { get; set; }
        /// <summary>
        /// 是否為圖片
        /// </summary>
        public bool isImage { get; set; }
        /// <summary>
        /// 上傳時間
        /// </summary>
        public DateTime time { get; set; }
    }
}