using System;
using System.Text;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.Reflection;
using System.Web;
//using System.Web.Mvc;
using System.ComponentModel;

namespace Common
{
    /// <summary>
    /// 功能描述：共用工具類
    /// </summary>
    public static class Tools
    {

        #region 得到字串長度，一個漢字長度為2
        /// <summary>
        /// 得到字串長度，一個漢字長度為2
        /// </summary>
        /// <param name="inputString">參數字串</param>
        /// <returns></returns>
        public static int StrLength(string inputString)
        {
            System.Text.ASCIIEncoding ascii = new System.Text.ASCIIEncoding();
            int tempLen = 0;
            byte[] s = ascii.GetBytes(inputString);
            for (int i = 0; i < s.Length; i++)
            {
                if ((int)s[i] == 63) //todo:漢字會變為問號，但問號也算成2個了
                    tempLen += 2;
                else
                    tempLen += 1;
            }
            return tempLen;
        }
        #endregion

        #region 截取指定長度字串
        /// <summary>
        /// 截取指定長度字串
        /// </summary>
        /// <param name="inputString">要處理的字串</param>
        /// <param name="len">指定長度</param>
        /// <returns>返回處理後的字串</returns>
        public static string ClipString(string inputString, int len)
        {
            bool isShowFix = false;
            if (len % 2 == 1)
            {
                isShowFix = true;
                len--;
            }
            System.Text.ASCIIEncoding ascii = new System.Text.ASCIIEncoding();
            int tempLen = 0;
            string tempString = "";
            byte[] s = ascii.GetBytes(inputString);
            for (int i = 0; i < s.Length; i++)
            {
                if ((int)s[i] == 63)
                    tempLen += 2;
                else
                    tempLen += 1;

                try
                {
                    tempString += inputString.Substring(i, 1);
                }
                catch
                {
                    break;
                }

                if (tempLen > len)
                    break;
            }

            byte[] mybyte = System.Text.Encoding.Default.GetBytes(inputString);
            if (isShowFix && mybyte.Length > len)
                tempString += "…";
            return tempString;
        }
        #endregion

        #region 獲得兩個日期的間隔
        /// <summary>
        /// 獲得兩個日期的間隔
        /// </summary>
        /// <param name="DateTime1">日期一。</param>
        /// <param name="DateTime2">日期二。</param>
        /// <returns>日期間隔TimeSpan。</returns>
        public static TimeSpan DateDiff(DateTime DateTime1, DateTime DateTime2)
        {
            TimeSpan ts1 = new TimeSpan(DateTime1.Ticks);
            TimeSpan ts2 = new TimeSpan(DateTime2.Ticks);
            TimeSpan ts = ts1.Subtract(ts2).Duration();
            return ts;
        }
        #endregion

        #region 格式化日期時間
        /// <summary>
        /// 格式化日期時間
        /// </summary>
        /// <param name="dateTime1">日期時間</param>
        /// <param name="dateMode">顯示模式</param>
        /// <returns>0-9種模式的日期</returns>
        public static string FormatDate(DateTime dateTime1, string dateMode)
        {
            switch (dateMode)
            {
                case "0":
                    return dateTime1.ToString("yyyy-MM-dd");
                case "1":
                    return dateTime1.ToString("yyyy-MM-dd HH:mm:ss");
                case "2":
                    return dateTime1.ToString("yyyy/MM/dd");
                case "3":
                    return dateTime1.ToString("yyyy年MM月dd日");
                case "4":
                    return dateTime1.ToString("MM-dd");
                case "5":
                    return dateTime1.ToString("MM/dd");
                case "6":
                    return dateTime1.ToString("MM月dd日");
                case "7":
                    return dateTime1.ToString("yyyy-MM");
                case "8":
                    return dateTime1.ToString("yyyy/MM");
                case "9":
                    return dateTime1.ToString("yyyy年MM月");
                default:
                    return dateTime1.ToString();
            }
        }
        #endregion

        #region 得到隨機日期
        /// <summary>
        /// 得到隨機日期
        /// </summary>
        /// <param name="time1">起始日期</param>
        /// <param name="time2">結束日期</param>
        /// <returns>間隔日期之間的 隨機日期</returns>
        public static DateTime GetRandomTime(DateTime time1, DateTime time2)
        {
            Random random = new Random();
            DateTime minTime = new DateTime();
            DateTime maxTime = new DateTime();

            System.TimeSpan ts = new System.TimeSpan(time1.Ticks - time2.Ticks);

            // 獲取兩個時間相隔的秒數
            double dTotalSecontds = ts.TotalSeconds;
            int iTotalSecontds = 0;

            if (dTotalSecontds > System.Int32.MaxValue)
            {
                iTotalSecontds = System.Int32.MaxValue;
            }
            else if (dTotalSecontds < System.Int32.MinValue)
            {
                iTotalSecontds = System.Int32.MinValue;
            }
            else
            {
                iTotalSecontds = (int)dTotalSecontds;
            }


            if (iTotalSecontds > 0)
            {
                minTime = time2;
                maxTime = time1;
            }
            else if (iTotalSecontds < 0)
            {
                minTime = time1;
                maxTime = time2;
            }
            else
            {
                return time1;
            }

            int maxValue = iTotalSecontds;

            if (iTotalSecontds <= System.Int32.MinValue)
                maxValue = System.Int32.MinValue + 1;

            int i = random.Next(System.Math.Abs(maxValue));

            return minTime.AddSeconds(i);
        }
        /// <summary>
        /// 獲取時間戳記
        /// </summary>
        public static string GetRandomTimeSpan()
        {
            TimeSpan ts = DateTime.Now - new DateTime(1970, 1, 1, 0, 0, 0, 0);
            return Convert.ToInt64(ts.TotalSeconds).ToString();
        }
        #endregion

        #region HTML轉行成TEXT
        /// <summary>
        /// HTML轉行成TEXT
        /// </summary>
        /// <param name="strHtml"></param>
        /// <returns></returns>
        public static string HtmlToTxt(string strHtml)
        {
            string[] aryReg ={
            @"<script[^>]*?>.*?</script>",
            @"<(\/\s*)?!?((\w+:)?\w+)(\w+(\s*=?\s*(([""'])(\\[""'tbnr]|[^\7])*?\7|\w+)|.{0})|\s)*?(\/\s*)?>",
            @"([\r\n])[\s]+",
            @"&(quot|#34);",
            @"&(amp|#38);",
            @"&(lt|#60);",
            @"&(gt|#62);",
            @"&(nbsp|#160);",
            @"&(iexcl|#161);",
            @"&(cent|#162);",
            @"&(pound|#163);",
            @"&(copy|#169);",
            @"&#(\d+);",
            @"-->",
            @"<!--.*\n"
            };

            string newReg = aryReg[0];
            string strOutput = strHtml;
            for (int i = 0; i < aryReg.Length; i++)
            {
                Regex regex = new Regex(aryReg[i], RegexOptions.IgnoreCase);
                strOutput = regex.Replace(strOutput, string.Empty);
            }

            strOutput.Replace("<", "");
            strOutput.Replace(">", "");
            strOutput.Replace("\r\n", "");


            return strOutput;
        }
        #endregion

        #region 判斷物件是否為空
        /// <summary>
        /// 判斷物件是否為空，為空返回true
        /// </summary>
        /// <typeparam name="T">要驗證的對象的類型</typeparam>
        /// <param name="data">要驗證的對象</param>        
        public static bool IsNullOrEmpty<T>(this T data)
        {
            //如果為null
            if (data == null)
            {
                return true;
            }

            //如果為""
            if (data.GetType() == typeof(String))
            {
                if (string.IsNullOrEmpty(data.ToString().Trim()) || data.ToString() == "")
                {
                    return true;
                }
            }

            //如果為DBNull
            if (data.GetType() == typeof(DBNull))
            {
                return true;
            }

            //不為空
            return false;
        }

        /// <summary>
        /// 判斷物件是否為空，為空返回true
        /// </summary>
        /// <param name="data">要驗證的對象</param>
        public static bool IsNullOrEmpty(this object data)
        {
            //如果為null
            if (data == null)
            {
                return true;
            }

            //如果為""
            if (data.GetType() == typeof(String))
            {
                if (string.IsNullOrEmpty(data.ToString().Trim()))
                {
                    return true;
                }
            }

            //如果為DBNull
            if (data.GetType() == typeof(DBNull))
            {
                return true;
            }

            //不為空
            return false;
        }
        #endregion      

        #region 驗證是否為浮點數
        /// <summary>
        /// 驗證是否浮點數
        /// </summary>
        /// <param name="floatNum"></param>
        /// <returns></returns>
        public static bool IsFloat(this string floatNum)
        {
            //如果為空，認為驗證不合格
            if (IsNullOrEmpty(floatNum))
            {
                return false;
            }
            //清除要驗證字串中的空格
            floatNum = floatNum.Trim();

            //模式字串
            string pattern = @"^(-?\d+)(\.\d+)?$";

            //驗證
            return RegexHelper.IsMatch(floatNum, pattern);
        }
        #endregion

        #region 驗證是否為整數
        /// <summary>
        /// 驗證是否為整數 如果為空，認為驗證不合格 返回false
        /// </summary>
        /// <param name="number">要驗證的整數</param>        
        public static bool IsInt(this string number)
        {
            //如果為空，認為驗證不合格
            if (IsNullOrEmpty(number))
            {
                return false;
            }

            //清除要驗證字串中的空格
            number = number.Trim();

            //模式字串
            string pattern = @"^[0-9]+[0-9]*$";

            //驗證
            return RegexHelper.IsMatch(number, pattern);
        }
        #endregion

        #region 驗證是否為數字
        /// <summary>
        /// 驗證是否為數字
        /// </summary>
        /// <param name="number">要驗證的數字</param>        
        public static bool IsNumber(this string number)
        {
            //如果為空，認為驗證不合格
            if (IsNullOrEmpty(number))
            {
                return false;
            }

            //清除要驗證字串中的空格
            number = number.Trim();

            //模式字串
            string pattern = @"^[0-9]+[0-9]*[.]?[0-9]*$";

            //驗證
            return RegexHelper.IsMatch(number, pattern);
        }
        #endregion

        #region 驗證日期是否合法
        /// <summary>
        /// 是否是日期
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        public static bool IsDate(this object date)
        {

            //如果為空，認為驗證合格
            if (IsNullOrEmpty(date))
            {
                return false;
            }
            string strdate = date.ToString();
            try
            {
                //用轉換測試是否為規則的日期字元
                date = Convert.ToDateTime(date).ToString("d");
                return true;
            }
            catch
            {
                //如果日期字串中存在非數位，則返回false
                if (!IsInt(strdate))
                {
                    return false;
                }

                #region 對純數位進行解析
                //對8位元純數位進行解析
                if (strdate.Length == 8)
                {
                    //獲取年月日
                    string year = strdate.Substring(0, 4);
                    string month = strdate.Substring(4, 2);
                    string day = strdate.Substring(6, 2);

                    //驗證合法性
                    if (Convert.ToInt32(year) < 1900 || Convert.ToInt32(year) > 2100)
                    {
                        return false;
                    }
                    if (Convert.ToInt32(month) > 12 || Convert.ToInt32(day) > 31)
                    {
                        return false;
                    }

                    //拼接日期
                    date = Convert.ToDateTime(year + "-" + month + "-" + day).ToString("d");
                    return true;
                }

                //對6位元純數位進行解析
                if (strdate.Length == 6)
                {
                    //獲取年月
                    string year = strdate.Substring(0, 4);
                    string month = strdate.Substring(4, 2);

                    //驗證合法性
                    if (Convert.ToInt32(year) < 1900 || Convert.ToInt32(year) > 2100)
                    {
                        return false;
                    }
                    if (Convert.ToInt32(month) > 12)
                    {
                        return false;
                    }

                    //拼接日期
                    date = Convert.ToDateTime(year + "-" + month).ToString("d");
                    return true;
                }

                //對5位元純數位進行解析
                if (strdate.Length == 5)
                {
                    //獲取年月
                    string year = strdate.Substring(0, 4);
                    string month = strdate.Substring(4, 1);

                    //驗證合法性
                    if (Convert.ToInt32(year) < 1900 || Convert.ToInt32(year) > 2100)
                    {
                        return false;
                    }

                    //拼接日期
                    date = year + "-" + month;
                    return true;
                }

                //對4位元純數位進行解析
                if (strdate.Length == 4)
                {
                    //獲取年
                    string year = strdate.Substring(0, 4);

                    //驗證合法性
                    if (Convert.ToInt32(year) < 1900 || Convert.ToInt32(year) > 2100)
                    {
                        return false;
                    }

                    //拼接日期
                    date = Convert.ToDateTime(year).ToString("d");
                    return true;
                }
                #endregion

                return false;
            }

        }
        /// <summary>
        /// 驗證日期是否合法,對不規則的作了簡單處理
        /// </summary>
        /// <param name="date">日期</param>
        public static bool IsDate(ref string date)
        {
            //如果為空，認為驗證合格
            if (IsNullOrEmpty(date))
            {
                return true;
            }

            //清除要驗證字串中的空格
            date = date.Trim();

            //替換\
            date = date.Replace(@"\", "-");
            //替換/
            date = date.Replace(@"/", "-");

            //如果查找到漢字"今",則認為是當前日期
            if (date.IndexOf("今") != -1)
            {
                date = DateTime.Now.ToString();
            }

            try
            {
                //用轉換測試是否為規則的日期字元
                date = Convert.ToDateTime(date).ToString("d");
                return true;
            }
            catch
            {
                //如果日期字串中存在非數位，則返回false
                if (!IsInt(date))
                {
                    return false;
                }

                #region 對純數位進行解析
                //對8位元純數位進行解析
                if (date.Length == 8)
                {
                    //獲取年月日
                    string year = date.Substring(0, 4);
                    string month = date.Substring(4, 2);
                    string day = date.Substring(6, 2);

                    //驗證合法性
                    if (Convert.ToInt32(year) < 1900 || Convert.ToInt32(year) > 2100)
                    {
                        return false;
                    }
                    if (Convert.ToInt32(month) > 12 || Convert.ToInt32(day) > 31)
                    {
                        return false;
                    }

                    //拼接日期
                    date = Convert.ToDateTime(year + "-" + month + "-" + day).ToString("d");
                    return true;
                }

                //對6位元純數位進行解析
                if (date.Length == 6)
                {
                    //獲取年月
                    string year = date.Substring(0, 4);
                    string month = date.Substring(4, 2);

                    //驗證合法性
                    if (Convert.ToInt32(year) < 1900 || Convert.ToInt32(year) > 2100)
                    {
                        return false;
                    }
                    if (Convert.ToInt32(month) > 12)
                    {
                        return false;
                    }

                    //拼接日期
                    date = Convert.ToDateTime(year + "-" + month).ToString("d");
                    return true;
                }

                //對5位元純數位進行解析
                if (date.Length == 5)
                {
                    //獲取年月
                    string year = date.Substring(0, 4);
                    string month = date.Substring(4, 1);

                    //驗證合法性
                    if (Convert.ToInt32(year) < 1900 || Convert.ToInt32(year) > 2100)
                    {
                        return false;
                    }

                    //拼接日期
                    date = year + "-" + month;
                    return true;
                }

                //對4位元純數位進行解析
                if (date.Length == 4)
                {
                    //獲取年
                    string year = date.Substring(0, 4);

                    //驗證合法性
                    if (Convert.ToInt32(year) < 1900 || Convert.ToInt32(year) > 2100)
                    {
                        return false;
                    }

                    //拼接日期
                    date = Convert.ToDateTime(year).ToString("d");
                    return true;
                }
                #endregion

                return false;
            }
        }
        #endregion

        /// <summary>
        /// 前臺顯示郵箱的遮罩替換(由tzh@qq.com等替換成t*****@qq.com)
        /// </summary>
        /// <param name="Email">郵箱</param>
        /// <returns></returns>
        public static string GetEmail(string Email)
        {

            string strArg = "";
            string SendEmail = "";
            Match match = Regex.Match(Email, @"(\w)\w+@");

            if (match.Success)
            {
                strArg = match.Groups[1].Value + "*****@";
                SendEmail = Regex.Replace(Email, @"\w+@", strArg);
            }
            else
                SendEmail = Email;
            return SendEmail;
        }

        /// <summary>
        /// 檢查字串是否存在與一個,組合到一起的字串陣列中
        /// </summary>
        /// <param name="strSplit">未分割的字串</param>
        /// <param name="split">分割符號</param>
        /// <param name="targetValue">目標字串</param>
        /// <returns></returns>
        public static bool CheckStringHasValue(string strSplit, char split, string targetValue)
        {
            string[] strList = strSplit.Split(split);
            foreach (string str in strList)
            {
                if (targetValue == str)
                    return true;
            }
            return false;
        }

        #region 枚舉型相關操作

        /// <summary>
        /// 功能描述；獲取枚舉名稱.傳入枚舉類型和枚舉值
        /// </summary>
        /// <param name="enumType"></param>
        /// <param name="intEnumValue"></param>
        /// <returns></returns>
        public static string GetEnumText<T>(int intEnumValue)
        {
            return Enum.GetName(typeof(T), intEnumValue);
        }

        /// <summary>
        /// 功能描述:獲取枚舉項集合，傳入枚舉類型
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static IList<object> BindEnums<T>()
        {
            IList<object> _list = new List<object>();
            //遍歷枚舉集合
            foreach (int i in Enum.GetValues(typeof(T)))
            {
                var _selItem = new
                {
                    Value = i,
                    Text = Enum.GetName(typeof(T), i)
                };
                _list.Add(_selItem);
            }
            return _list;
        }

        ///<summary>
        /// 返回 Dic 枚舉項，描述
        ///</summary>
        ///<param name="enumType"></param>
        ///<returns>Dic枚舉項，描述</returns>
        public static Dictionary<string, string> BindEnums(Type enumType)
        {
            Dictionary<string, string> dic = new Dictionary<string, string>();
            FieldInfo[] fieldinfos = enumType.GetFields();
            foreach (FieldInfo field in fieldinfos)
            {
                if (field.FieldType.IsEnum)
                {
                    Object[] objs = field.GetCustomAttributes(typeof(DescriptionAttribute), false);

                    dic.Add(field.Name, ((DescriptionAttribute)objs[0]).Description);
                }

            }

            return dic;
        }
        ///<summary>
        /// 返回 List《Enums.EnumsClass》 枚舉值、名稱、描述
        ///</summary>
        public static List<Enums.EnumsClass> BindEnumsList(Type enumType)
        {
            var list = new List<Enums.EnumsClass>();
            FieldInfo[] fieldinfos = enumType.GetFields();
            var enumvalue = Enum.GetValues(enumType);
            foreach (FieldInfo field in fieldinfos)
            {
                if (field.FieldType.IsEnum)
                {
                    int ev = -1;
                    Object[] objs = field.GetCustomAttributes(typeof(DescriptionAttribute), false);
                    foreach (int item in enumvalue)
                    {
                        if (Enum.GetName(enumType, item) == field.Name)
                        {
                            ev = item;
                            break;
                        }
                    }
                    list.Add(new Enums.EnumsClass
                    {
                        Name = field.Name,
                        Value = ev,
                        Text = ((DescriptionAttribute)objs[0]).Description
                    });
                }
            }
            return list;
        }

        #endregion

        #region 獲取集合中某個欄位的拼接，例：獲取姓名拼接

        /// <summary>
        /// 功能描述：獲取集合中某個欄位的拼接，例：獲取姓名拼接
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list">集合</param>
        /// <param name="strFieldName">欄位名</param>
        /// <param name="strSplit">分隔符號</param>
        /// <returns></returns>
        public static string GetFieldValueJoin<T>(IList<T> list, string strFieldName, string strSplit)
        {
            //判斷入口
            if (list == null || list.Count <= 0 || string.IsNullOrEmpty(strFieldName))
                return string.Empty;


            //獲取屬性
            PropertyInfo _pro = typeof(T).GetProperty(strFieldName);
            if (_pro == null)
                return string.Empty;
            //變數，記錄返回值
            string _strReturn = string.Empty;
            foreach (T _entityI in list)
            {
                //獲取屬性值
                object _objValue = _pro.GetValue(_entityI, null);
                if (_objValue == null || string.IsNullOrEmpty(_objValue.ToString()))
                    //沒有屬性值，則跳過
                    continue;

                //有屬性值，則拼接
                _strReturn += _objValue.ToString() + strSplit;
            }

            if (string.IsNullOrEmpty(_strReturn))
                return string.Empty;

            return _strReturn.Substring(0, _strReturn.Length - strSplit.Length);
        }

        #endregion



    }
}