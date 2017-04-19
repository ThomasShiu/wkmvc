using System.Text.RegularExpressions;

namespace Common
{
    /// <summary>
    /// 操作規則運算式的公共類
    /// </summary>    
    public class RegexHelper
    {
        #region 驗證輸入字串是否與模式字串匹配
        /// <summary>
        /// 驗證輸入字串是否與模式字串匹配，匹配返回true
        /// </summary>
        /// <param name="input">輸入字串</param>
        /// <param name="pattern">模式字串</param>        
        public static bool IsMatch(string input, string pattern)
        {
            return IsMatch(input, pattern, RegexOptions.IgnoreCase);
        }

        /// <summary>
        /// 驗證輸入字串是否與模式字串匹配，匹配返回true
        /// </summary>
        /// <param name="input">輸入的字串</param>
        /// <param name="pattern">模式字串</param>
        /// <param name="options">篩選條件</param>
        public static bool IsMatch(string input, string pattern, RegexOptions options)
        {
            return Regex.IsMatch(input, pattern, options);
        }
        #endregion
    }
}