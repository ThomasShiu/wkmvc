using System.Web;

namespace Common
{
    /// <summary>
    /// Session 操作類
    /// 1、GetSession(string name)根據session名獲取session物件
    /// 2、SetSession(string name, object val)設置session
    /// </summary>
    public class SessionHelper
    {
        /// <summary>
        /// 根據session名獲取session物件
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static object GetSession(string name)
        {
            return HttpContext.Current.Session[name];
        }
        /// <summary>
        /// 設置session
        /// </summary>
        /// <param name="name">session 名</param>
        /// <param name="val">session 值</param>
        public static void SetSession(string name, object val)
        {
            HttpContext.Current.Session.Remove(name);
            HttpContext.Current.Session.Add(name, val);
        }
        /// <summary>
        /// 添加Session，調動有效期為20分鐘
        /// </summary>
        /// <param name="strSessionName">Session對象名稱</param>
        /// <param name="strValue">Session值</param>
        public static void Add(string strSessionName, string strValue)
        {
            HttpContext.Current.Session[strSessionName] = strValue;
            HttpContext.Current.Session.Timeout = 20;
        }

        /// <summary>
        /// 添加Session，調動有效期為20分鐘
        /// </summary>
        /// <param name="strSessionName">Session對象名稱</param>
        /// <param name="strValues">Session值陣列</param>
        public static void Adds(string strSessionName, string[] strValues)
        {
            HttpContext.Current.Session[strSessionName] = strValues;
            HttpContext.Current.Session.Timeout = 20;
        }

        /// <summary>
        /// 添加Session
        /// </summary>
        /// <param name="strSessionName">Session對象名稱</param>
        /// <param name="strValue">Session值</param>
        /// <param name="iExpires">調動有效期（分鐘）</param>
        public static void Add(string strSessionName, string strValue, int iExpires)
        {
            HttpContext.Current.Session[strSessionName] = strValue;
            HttpContext.Current.Session.Timeout = iExpires;
        }

        /// <summary>
        /// 添加Session
        /// </summary>
        /// <param name="strSessionName">Session對象名稱</param>
        /// <param name="strValues">Session值陣列</param>
        /// <param name="iExpires">調動有效期（分鐘）</param>
        public static void Adds(string strSessionName, string[] strValues, int iExpires)
        {
            HttpContext.Current.Session[strSessionName] = strValues;
            HttpContext.Current.Session.Timeout = iExpires;
        }

        /// <summary>
        /// 讀取某個Session對象值
        /// </summary>
        /// <param name="strSessionName">Session對象名稱</param>
        /// <returns>Session對象值</returns>
        public static string Get(string strSessionName)
        {
            if (HttpContext.Current.Session[strSessionName] == null)
            {
                return null;
            }
            else
            {
                return HttpContext.Current.Session[strSessionName].ToString();
            }
        }

        /// <summary>
        /// 讀取某個Session物件值陣列
        /// </summary>
        /// <param name="strSessionName">Session對象名稱</param>
        /// <returns>Session物件值陣列</returns>
        public static string[] Gets(string strSessionName)
        {
            if (HttpContext.Current.Session[strSessionName] == null)
            {
                return null;
            }
            else
            {
                return (string[])HttpContext.Current.Session[strSessionName];
            }
        }

        /// <summary>
        /// 刪除某個Session對象
        /// </summary>
        /// <param name="strSessionName">Session對象名稱</param>
        public static void Del(string strSessionName)
        {
            HttpContext.Current.Session[strSessionName] = null;
        }
        /// <summary>
        /// 移除Session
        /// </summary>
        public static void Remove(string sessionname)
        {
            if (HttpContext.Current.Session[sessionname] != null)
            {
                HttpContext.Current.Session.Remove(sessionname);
                HttpContext.Current.Session[sessionname] = null;
            }
        }
    }
}
