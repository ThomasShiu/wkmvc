using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Net;
using System.Web;
using System.Security.Cryptography;
using System.Text.RegularExpressions;
using System.Web.Script.Serialization;
using System.Data;
using System.Collections;
using System.Runtime.Serialization.Json;
using System.Configuration;
using System.Reflection;

namespace Common
{
    /// <summary>
    /// 系統説明類
    /// </summary>
    public class Utils
    {
        #region 物件轉換處理
        /// <summary>
        /// 判斷物件是否為Int32類型的數字
        /// </summary>
        /// <param name="Expression"></param>
        /// <returns></returns>
        public static bool IsNumeric(object expression)
        {
            if (expression != null)
                return IsNumeric(expression.ToString());

            return false;

        }

        /// <summary>
        /// 判斷物件是否為Int32類型的數字
        /// </summary>
        /// <param name="Expression"></param>
        /// <returns></returns>
        public static bool IsNumeric(string expression)
        {
            if (expression != null)
            {
                string str = expression;
                if (str.Length > 0 && str.Length <= 11 && Regex.IsMatch(str, @"^[-]?[0-9]*[.]?[0-9]*$"))
                {
                    if ((str.Length < 10) || (str.Length == 10 && str[0] == '1') || (str.Length == 11 && str[0] == '-' && str[1] == '1'))
                        return true;
                }
            }
            return false;
        }

        /// <summary>
        /// 是否為Double類型
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        public static bool IsDouble(object expression)
        {
            if (expression != null)
                return Regex.IsMatch(expression.ToString(), @"^([0-9])[0-9]*(\.\w*)?$");

            return false;
        }

        /// <summary>
        /// 將字串轉換為陣列
        /// </summary>
        /// <param name="str">字串</param>
        /// <returns>字串陣列</returns>
        public static string[] GetStrArray(string str)
        {
            return str.Split(new char[',']);
        }

        /// <summary>
        /// 將陣列轉換為字串
        /// </summary>
        /// <param name="list">List</param>
        /// <param name="speater">分隔符號</param>
        /// <returns>String</returns>
        public static string GetArrayStr(List<string> list, string speater)
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < list.Count; i++)
            {
                if (i == list.Count - 1)
                {
                    sb.Append(list[i]);
                }
                else
                {
                    sb.Append(list[i]);
                    sb.Append(speater);
                }
            }
            return sb.ToString();
        }

        /// <summary>
        /// object型轉換為bool型
        /// </summary>
        /// <param name="strValue">要轉換的字串</param>
        /// <param name="defValue">缺省值</param>
        /// <returns>轉換後的bool類型結果</returns>
        public static bool StrToBool(object expression, bool defValue)
        {
            if (expression != null)
                return StrToBool(expression, defValue);

            return defValue;
        }

        /// <summary>
        /// string型轉換為bool型
        /// </summary>
        /// <param name="strValue">要轉換的字串</param>
        /// <param name="defValue">缺省值</param>
        /// <returns>轉換後的bool類型結果</returns>
        public static bool StrToBool(string expression, bool defValue)
        {
            if (expression != null)
            {
                if (string.Compare(expression, "true", true) == 0)
                    return true;
                else if (string.Compare(expression, "false", true) == 0)
                    return false;
            }
            return defValue;
        }

        /// <summary>
        /// 將對象轉換為Int32類型
        /// </summary>
        /// <param name="expression">要轉換的字串</param>
        /// <param name="defValue">缺省值</param>
        /// <returns>轉換後的int類型結果</returns>
        public static int ObjToInt(object expression, int defValue)
        {
            if (expression != null)
                return StrToInt(expression.ToString(), defValue);

            return defValue;
        }

        /// <summary>
        /// 將字串轉換為Int32類型
        /// </summary>
        /// <param name="expression">要轉換的字串</param>
        /// <param name="defValue">缺省值</param>
        /// <returns>轉換後的int類型結果</returns>
        public static int StrToInt(string expression, int defValue)
        {
            if (string.IsNullOrEmpty(expression) || expression.Trim().Length >= 11 || !Regex.IsMatch(expression.Trim(), @"^([-]|[0-9])[0-9]*(\.\w*)?$"))
                return defValue;

            int rv;
            if (Int32.TryParse(expression, out rv))
                return rv;

            return Convert.ToInt32(StrToFloat(expression, defValue));
        }

        /// <summary>
        /// Object型轉換為decimal型
        /// </summary>
        /// <param name="strValue">要轉換的字串</param>
        /// <param name="defValue">缺省值</param>
        /// <returns>轉換後的decimal類型結果</returns>
        public static decimal ObjToDecimal(object expression, decimal defValue)
        {
            if (expression != null)
                return StrToDecimal(expression.ToString(), defValue);

            return defValue;
        }

        /// <summary>
        /// string型轉換為decimal型
        /// </summary>
        /// <param name="strValue">要轉換的字串</param>
        /// <param name="defValue">缺省值</param>
        /// <returns>轉換後的decimal類型結果</returns>
        public static decimal StrToDecimal(string expression, decimal defValue)
        {
            if ((expression == null) || (expression.Length > 10))
                return defValue;

            decimal intValue = defValue;
            if (expression != null)
            {
                bool IsDecimal = Regex.IsMatch(expression, @"^([-]|[0-9])[0-9]*(\.\w*)?$");
                if (IsDecimal)
                    decimal.TryParse(expression, out intValue);
            }
            return intValue;
        }

        /// <summary>
        /// Object型轉換為float型
        /// </summary>
        /// <param name="strValue">要轉換的字串</param>
        /// <param name="defValue">缺省值</param>
        /// <returns>轉換後的int類型結果</returns>
        public static float ObjToFloat(object expression, float defValue)
        {
            if (expression != null)
                return StrToFloat(expression.ToString(), defValue);

            return defValue;
        }

        /// <summary>
        /// string型轉換為float型
        /// </summary>
        /// <param name="strValue">要轉換的字串</param>
        /// <param name="defValue">缺省值</param>
        /// <returns>轉換後的int類型結果</returns>
        public static float StrToFloat(string expression, float defValue)
        {
            if ((expression == null) || (expression.Length > 10))
                return defValue;

            float intValue = defValue;
            if (expression != null)
            {
                bool IsFloat = Regex.IsMatch(expression, @"^([-]|[0-9])[0-9]*(\.\w*)?$");
                if (IsFloat)
                    float.TryParse(expression, out intValue);
            }
            return intValue;
        }

        /// <summary>
        /// 將對象轉換為日期時間類型
        /// </summary>
        /// <param name="str">要轉換的字串</param>
        /// <param name="defValue">缺省值</param>
        /// <returns>轉換後的int類型結果</returns>
        public static DateTime StrToDateTime(string str, DateTime defValue)
        {
            if (!string.IsNullOrEmpty(str))
            {
                DateTime dateTime;
                if (DateTime.TryParse(str, out dateTime))
                    return dateTime;
            }
            return defValue;
        }

        /// <summary>
        /// 將對象轉換為日期時間類型
        /// </summary>
        /// <param name="str">要轉換的字串</param>
        /// <returns>轉換後的int類型結果</returns>
        public static DateTime StrToDateTime(string str)
        {
            return StrToDateTime(str, DateTime.Now);
        }

        /// <summary>
        /// 將對象轉換為日期時間類型
        /// </summary>
        /// <param name="obj">要轉換的對象</param>
        /// <returns>轉換後的int類型結果</returns>
        public static DateTime ObjectToDateTime(object obj)
        {
            return StrToDateTime(obj.ToString());
        }

        /// <summary>
        /// 將對象轉換為日期時間類型
        /// </summary>
        /// <param name="obj">要轉換的對象</param>
        /// <param name="defValue">缺省值</param>
        /// <returns>轉換後的int類型結果</returns>
        public static DateTime ObjectToDateTime(object obj, DateTime defValue)
        {
            return StrToDateTime(obj.ToString(), defValue);
        }

        /// <summary>
        /// 將物件轉換為字串
        /// </summary>
        /// <param name="obj">要轉換的對象</param>
        /// <returns>轉換後的string類型結果</returns>
        public static string ObjectToStr(object obj)
        {
            if (obj == null)
                return "";
            return obj.ToString().Trim();
        }
        #endregion

        #region 分割字串
        /// <summary>
        /// 分割字串
        /// </summary>
        public static string[] SplitString(string strContent, string strSplit)
        {
            if (!string.IsNullOrEmpty(strContent))
            {
                if (strContent.IndexOf(strSplit) < 0)
                    return new string[] { strContent };

                return Regex.Split(strContent, Regex.Escape(strSplit), RegexOptions.IgnoreCase);
            }
            else
                return new string[0] { };
        }

        /// <summary>
        /// 分割字串
        /// </summary>
        /// <returns></returns>
        public static string[] SplitString(string strContent, string strSplit, int count)
        {
            string[] result = new string[count];
            string[] splited = SplitString(strContent, strSplit);

            for (int i = 0; i < count; i++)
            {
                if (i < splited.Length)
                    result[i] = splited[i];
                else
                    result[i] = string.Empty;
            }

            return result;
        }
        #endregion

        #region 截取字串
        public static string GetSubString(string p_SrcString, int p_Length, string p_TailString)
        {
            return GetSubString(p_SrcString, 0, p_Length, p_TailString);
        }
        public static string GetSubString(string p_SrcString, int p_StartIndex, int p_Length, string p_TailString)
        {
            string str = p_SrcString;
            byte[] bytes = Encoding.UTF8.GetBytes(p_SrcString);
            foreach (char ch in Encoding.UTF8.GetChars(bytes))
            {
                if (((ch > 'ࠀ') && (ch < '一')) || ((ch > 0xac00) && (ch < 0xd7a3)))
                {
                    if (p_StartIndex >= p_SrcString.Length)
                    {
                        return "";
                    }
                    return p_SrcString.Substring(p_StartIndex, ((p_Length + p_StartIndex) > p_SrcString.Length) ? (p_SrcString.Length - p_StartIndex) : p_Length);
                }
            }
            if (p_Length < 0)
            {
                return str;
            }
            byte[] sourceArray = Encoding.Default.GetBytes(p_SrcString);
            if (sourceArray.Length <= p_StartIndex)
            {
                return str;
            }
            int length = sourceArray.Length;
            if (sourceArray.Length > (p_StartIndex + p_Length))
            {
                length = p_Length + p_StartIndex;
            }
            else
            {
                p_Length = sourceArray.Length - p_StartIndex;
                p_TailString = "";
            }
            int num2 = p_Length;
            int[] numArray = new int[p_Length];
            byte[] destinationArray = null;
            int num3 = 0;
            for (int i = p_StartIndex; i < length; i++)
            {
                if (sourceArray[i] > 0x7f)
                {
                    num3++;
                    if (num3 == 3)
                    {
                        num3 = 1;
                    }
                }
                else
                {
                    num3 = 0;
                }
                numArray[i] = num3;
            }
            if ((sourceArray[length - 1] > 0x7f) && (numArray[p_Length - 1] == 1))
            {
                num2 = p_Length + 1;
            }
            destinationArray = new byte[num2];
            Array.Copy(sourceArray, p_StartIndex, destinationArray, 0, num2);
            return (Encoding.Default.GetString(destinationArray) + p_TailString);
        }
        #endregion

        #region 刪除最後結尾的一個逗號
        /// <summary>
        /// 刪除最後結尾的一個逗號
        /// </summary>
        public static string DelLastComma(string str)
        {
            return str.Substring(0, str.LastIndexOf(","));
        }
        #endregion

        #region 刪除最後結尾的指定字元後的字元
        /// <summary>
        /// 刪除最後結尾的指定字元後的字元
        /// </summary>
        public static string DelLastChar(string str, string strchar)
        {
            if (string.IsNullOrEmpty(str))
                return "";
            if (str.LastIndexOf(strchar) >= 0 && str.LastIndexOf(strchar) == str.Length - 1)
            {
                return str.Substring(0, str.LastIndexOf(strchar));
            }
            return str;
        }
        #endregion

        #region 生成指定長度的字串
        /// <summary>
        /// 生成指定長度的字串,即生成strLong個str字串
        /// </summary>
        /// <param name="strLong">生成的長度</param>
        /// <param name="str">以str生成字串</param>
        /// <returns></returns>
        public static string StringOfChar(int strLong, string str)
        {
            string ReturnStr = "";
            for (int i = 0; i < strLong; i++)
            {
                ReturnStr += str;
            }

            return ReturnStr;
        }
        #endregion

        #region 生成日期隨機碼
        /// <summary>
        /// 生成日期隨機碼
        /// </summary>
        /// <returns></returns>
        public static string GetRamCode()
        {
            #region
            return DateTime.Now.ToString("yyyyMMddHHmmssffff");
            #endregion
        }
        #endregion

        #region 生成隨機字母或數位
        /// <summary>
        /// 生成亂數字
        /// </summary>
        /// <param name="length">生成長度</param>
        /// <returns></returns>
        public static string Number(int Length)
        {
            return Number(Length, false);
        }

        /// <summary>
        /// 生成亂數字
        /// </summary>
        /// <param name="Length">生成長度</param>
        /// <param name="Sleep">是否要在生成前將當前執行緒阻止以避免重複</param>
        /// <returns></returns>
        public static string Number(int Length, bool Sleep)
        {
            if (Sleep)
                System.Threading.Thread.Sleep(3);
            string result = "";
            System.Random random = new Random();
            for (int i = 0; i < Length; i++)
            {
                result += random.Next(10).ToString();
            }
            return result;
        }
        /// <summary>
        /// 生成隨機字母字串(數位字母混和)
        /// </summary>
        /// <param name="codeCount">待生成的位數</param>
        public static string GetCheckCode(int codeCount)
        {
            string str = string.Empty;
            int rep = 0;
            long num2 = DateTime.Now.Ticks + rep;
            rep++;
            Random random = new Random(((int)(((ulong)num2) & 0xffffffffL)) | ((int)(num2 >> rep)));
            for (int i = 0; i < codeCount; i++)
            {
                char ch;
                int num = random.Next();
                if ((num % 2) == 0)
                {
                    ch = (char)(0x30 + ((ushort)(num % 10)));
                }
                else
                {
                    ch = (char)(0x41 + ((ushort)(num % 0x1a)));
                }
                str = str + ch.ToString();
            }
            return str;
        }
        /// <summary>
        /// 根據日期和隨機碼生成訂單號
        /// </summary>
        /// <returns></returns>
        public static string GetOrderNumber()
        {
            string num = DateTime.Now.ToString("yyMMddHHmmss");//yyyyMMddHHmmssms
            return num + Number(2).ToString();
        }
        private static int Next(int numSeeds, int length)
        {
            byte[] buffer = new byte[length];
            System.Security.Cryptography.RNGCryptoServiceProvider Gen = new System.Security.Cryptography.RNGCryptoServiceProvider();
            Gen.GetBytes(buffer);
            uint randomResult = 0x0;//這裡用uint作為生成的亂數  
            for (int i = 0; i < length; i++)
            {
                randomResult |= ((uint)buffer[i] << ((length - 1 - i) * 8));
            }
            return (int)(randomResult % numSeeds);
        }
        #endregion

        #region 截取字元長度
        /// <summary>
        /// 截取字元長度
        /// </summary>
        /// <param name="inputString">字元</param>
        /// <param name="len">長度</param>
        /// <returns></returns>
        public static string CutString(string inputString, int len)
        {
            if (string.IsNullOrEmpty(inputString))
                return "";
            inputString = DropHTML(inputString);
            ASCIIEncoding ascii = new ASCIIEncoding();
            int tempLen = 0;
            string tempString = "";
            byte[] s = ascii.GetBytes(inputString);
            for (int i = 0; i < s.Length; i++)
            {
                if ((int)s[i] == 63)
                {
                    tempLen += 2;
                }
                else
                {
                    tempLen += 1;
                }

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
            //如果截過則加上半個省略號 
            byte[] mybyte = System.Text.Encoding.Default.GetBytes(inputString);
            if (mybyte.Length > len)
                tempString += "…";
            return tempString;
        }
        #endregion

        #region 物件<-->JSON 4.0使用
        /// <summary>
        /// 對象轉JSON
        /// </summary>
        /// <typeparam name="T">物件實體</typeparam>
        /// <param name="t">內容</param>
        /// <returns>json包</returns>
        public static string ObjetcToJson<T>(T t)
        {
            try
            {
                DataContractJsonSerializer json = new DataContractJsonSerializer(typeof(T));
                string szJson = "";
                using (MemoryStream stream = new MemoryStream())
                {
                    json.WriteObject(stream, t);
                    szJson = Encoding.UTF8.GetString(stream.ToArray());
                }
                return szJson;
            }
            catch { return ""; }
        }

        /// <summary>
        /// Json包轉對象
        /// </summary>
        /// <typeparam name="T">對象</typeparam>
        /// <param name="jsonstring">json包</param>
        /// <returns>異常拋null</returns>
        public static object JsonToObject<T>(string jsonstring)
        {
            object result = null;
            try
            {
                DataContractJsonSerializer json = new DataContractJsonSerializer(typeof(T));
                using (MemoryStream stream = new MemoryStream(Encoding.UTF8.GetBytes(jsonstring)))
                {
                    result = json.ReadObject(stream);
                }
                return result;
            }
            catch { return result; }
        }
        #endregion

        #region 物件<-->JSON 2.0 使用litjson外掛程式
        /// <summary>
        /// 對象轉JSON  jsonData
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="t"></param>
        /// <returns></returns>
        //public static string ObjetcToJsonData<T>(T t)
        //{
        //    try
        //    {
        //        JsonData json = new JsonData(t);
        //        return json.ToJson();
        //    }
        //    catch
        //    {
        //        return "";
        //    }
        //}

        ///// <summary>
        ///// 對象轉JSON jsonMapper
        ///// </summary>
        ///// <typeparam name="T"></typeparam>
        ///// <param name="t"></param>
        ///// <returns></returns>
        //public static string ObjetcToJsonMapper<T>(T t)
        //{
        //    try
        //    {
        //        JsonData json = JsonMapper.ToJson(t);
        //        return json.ToJson();
        //    }
        //    catch
        //    {
        //        return "";
        //    }
        //}

        ///// <summary>
        ///// json轉對象 jsonMapper
        ///// </summary>
        ///// <param name="jsons"></param>
        ///// <returns></returns>
        //public static object JsonToObject(string jsons)
        //{
        //    try
        //    {
        //        JsonData jsonObject = JsonMapper.ToObject(jsons);
        //        return jsonObject;
        //    }
        //    catch { return null; }
        //}

        #endregion

        #region DataTable<-->JSON
        /// <summary> 
        /// DataTable轉為json 
        /// </summary> 
        /// <param name="dt">DataTable</param> 
        /// <returns>json數據</returns> 
        public static string DataTableToJson(DataTable dt)
        {
            List<Dictionary<string, object>> list = new List<Dictionary<string, object>>();
            foreach (DataRow dr in dt.Rows)
            {
                Dictionary<string, object> result = new Dictionary<string, object>();
                foreach (DataColumn dc in dt.Columns)
                {
                    result.Add(dc.ColumnName, dr[dc]);
                }
                list.Add(result);
            }

            return SerializeToJson(list);
        }
        /// <summary>
        /// 序列化物件為Json字串
        /// </summary>
        /// <param name="obj">要序列化的物件</param>
        /// <param name="recursionLimit">序列化物件的深度，預設為100</param>
        /// <returns>Json字串</returns>
        public static string SerializeToJson(object obj, int recursionLimit = 100)
        {
            try
            {
                JavaScriptSerializer serialize = new JavaScriptSerializer();
                serialize.RecursionLimit = recursionLimit;
                return serialize.Serialize(obj);
            }
            catch { return ""; }
        }
        /// <summary>
        /// json包轉DataTable
        /// </summary>
        /// <param name="jsons"></param>
        /// <returns></returns>
        public static DataTable JsonToDataTable(string jsons)
        {
            DataTable dt = new DataTable();
            try
            {
                JavaScriptSerializer serialize = new JavaScriptSerializer();
                serialize.MaxJsonLength = Int32.MaxValue;
                ArrayList list = serialize.Deserialize<ArrayList>(jsons);
                if (list.Count > 0)
                {
                    foreach (Dictionary<string, object> item in list)
                    {
                        if (item.Keys.Count == 0)//無值返回空
                        {
                            return dt;
                        }
                        if (dt.Columns.Count == 0)//初始Columns
                        {
                            foreach (string current in item.Keys)
                            {
                                dt.Columns.Add(current, item[current].GetType());
                            }
                        }
                        DataRow dr = dt.NewRow();
                        foreach (string current in item.Keys)
                        {
                            dr[current] = item[current];
                        }
                        dt.Rows.Add(dr);
                    }
                }
            }
            catch
            {
                return dt;
            }
            return dt;
        }
        #endregion

        #region List<--->DataTable
        /// <summary>
        /// DataTable轉換泛型集合
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="table"></param>
        /// <returns></returns>
        public static List<T> DataTableToList<T>(DataTable table)
        {
            List<T> list = new List<T>();
            T t = default(T);
            PropertyInfo[] propertypes = null;
            string tempName = string.Empty;
            foreach (DataRow row in table.Rows)
            {
                t = Activator.CreateInstance<T>();
                propertypes = t.GetType().GetProperties();
                foreach (PropertyInfo pro in propertypes)
                {
                    tempName = pro.Name;
                    if (table.Columns.Contains(tempName))
                    {
                        object value = row[tempName];
                        if (!value.ToString().Equals(""))
                        {
                            pro.SetValue(t, value, null);
                        }
                    }
                }
                list.Add(t);
            }
            return list.Count == 0 ? null : list;
        }

        /// <summary>
        /// 將集合類轉換成DataTable
        /// </summary>
        /// <param name="list">集合</param>
        /// <returns>DataTable</returns>
        public static DataTable ListToDataTable(IList list)
        {
            DataTable result = new DataTable();
            if (list != null && list.Count > 0)
            {
                PropertyInfo[] propertys = list[0].GetType().GetProperties();
                foreach (PropertyInfo pi in propertys)
                {
                    result.Columns.Add(pi.Name, pi.PropertyType);
                }
                for (int i = 0; i < list.Count; i++)
                {
                    ArrayList tempList = new ArrayList();
                    foreach (PropertyInfo pi in propertys)
                    {
                        object obj = pi.GetValue(list[i], null);
                        tempList.Add(obj);
                    }
                    object[] array = tempList.ToArray();
                    result.LoadDataRow(array, true);
                }
            }
            return result;
        }
        public static List<T> ConvertTo<T>(DataTable dt) where T : new()
        {
            if (dt == null) return null;
            if (dt.Rows.Count <= 0) return null;

            List<T> list = new List<T>();
            try
            {
                List<string> columnsName = new List<string>();
                foreach (DataColumn dataColumn in dt.Columns)
                {
                    columnsName.Add(dataColumn.ColumnName);//得到所有的表頭
                }
                list = dt.AsEnumerable().ToList().ConvertAll<T>(row => GetObject<T>(row, columnsName));  //轉換
                return list;
            }
            catch
            {
                return null;
            }
        }

        public static T GetObject<T>(DataRow row, List<string> columnsName) where T : new()
        {
            T obj = new T();
            try
            {
                string columnname = "";
                string value = "";
                PropertyInfo[] Properties = typeof(T).GetProperties();
                foreach (PropertyInfo objProperty in Properties)  //遍歷T的屬性
                {
                    columnname = columnsName.Find(name => name.ToLower() == objProperty.Name.ToLower()); //尋找可以匹配的表頭名稱
                    if (!string.IsNullOrEmpty(columnname))
                    {
                        value = row[columnname].ToString();
                        if (!string.IsNullOrEmpty(value))
                        {
                            if (Nullable.GetUnderlyingType(objProperty.PropertyType) != null) //存在匹配的表頭
                            {
                                value = row[columnname].ToString().Replace("$", "").Replace(",", ""); //從dataRow中提取資料
                                objProperty.SetValue(obj, Convert.ChangeType(value, Type.GetType(Nullable.GetUnderlyingType(objProperty.PropertyType).ToString())), null); //賦值操作
                            }
                            else
                            {
                                value = row[columnname].ToString().Replace("%", ""); //存在匹配的表頭
                                objProperty.SetValue(obj, Convert.ChangeType(value, Type.GetType(objProperty.PropertyType.ToString())), null);//賦值操作
                            }
                        }
                    }
                }
                return obj;
            }
            catch
            {
                return obj;
            }
        }
        /// <summary>
        /// 將泛型集合類轉換成DataTable
        /// </summary>
        /// <typeparam name="T">集合項類型</typeparam>
        /// <param name="list">集合</param>
        /// <param name="propertyName">需要返回的列的列名</param>
        /// <returns>資料集(表)</returns>
        public static DataTable ListToDataTable<T>(IList<T> list, params string[] propertyName)
        {
            List<string> propertyNameList = new List<string>();
            if (propertyName != null)
                propertyNameList.AddRange(propertyName);
            DataTable result = new DataTable();
            if (list != null && list.Count > 0)
            {
                PropertyInfo[] propertys = list[0].GetType().GetProperties();
                foreach (PropertyInfo pi in propertys)
                {
                    if (propertyNameList.Count == 0)
                    {
                        result.Columns.Add(pi.Name, pi.PropertyType);
                    }
                    else
                    {
                        if (propertyNameList.Contains(pi.Name))
                            result.Columns.Add(pi.Name, pi.PropertyType);
                    }
                }
                for (int i = 0; i < list.Count; i++)
                {
                    ArrayList tempList = new ArrayList();
                    foreach (PropertyInfo pi in propertys)
                    {
                        if (propertyNameList.Count == 0)
                        {
                            object obj = pi.GetValue(list[i], null);
                            tempList.Add(obj);
                        }
                        else
                        {
                            if (propertyNameList.Contains(pi.Name))
                            {
                                object obj = pi.GetValue(list[i], null);
                                tempList.Add(obj);
                            }
                        }
                    }
                    object[] array = tempList.ToArray();
                    result.LoadDataRow(array, true);
                }
            }
            return result;
        }

        #endregion

        #region 清除HTML標記
        public static string DropHTML(string Htmlstring)
        {
            if (string.IsNullOrEmpty(Htmlstring)) return "";
            //刪除腳本  
            Htmlstring = Regex.Replace(Htmlstring, @"<script[^>]*?>.*?</script>", "", RegexOptions.IgnoreCase);
            //刪除HTML  
            Htmlstring = Regex.Replace(Htmlstring, @"<(.[^>]*)>", "", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"([\r\n])[\s]+", "", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"-->", "", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"<!--.*", "", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"&(quot|#34);", "\"", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"&(amp|#38);", "&", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"&(lt|#60);", "<", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"&(gt|#62);", ">", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"&(nbsp|#160);", " ", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"&(iexcl|#161);", "\xa1", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"&(cent|#162);", "\xa2", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"&(pound|#163);", "\xa3", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"&(copy|#169);", "\xa9", RegexOptions.IgnoreCase);

            Htmlstring = Regex.Replace(Htmlstring, @"&#(\d+);", "", RegexOptions.IgnoreCase);
            Htmlstring = Htmlstring.Replace("<", "").Replace(">", "").Replace("\r\n", "");
            Htmlstring = HttpContext.Current.Server.HtmlEncode(Htmlstring).Trim();
            return Htmlstring;
        }
        #endregion

        #region 清除HTML標記且返回相應的長度
        public static string DropHTML(string Htmlstring, int strLen)
        {
            return CutString(DropHTML(Htmlstring), strLen);
        }
        #endregion

        #region TXT代碼轉換成HTML格式
        /// <summary>
        /// 字串字元處理
        /// </summary>
        /// <param name="chr">等待處理的字串</param>
        /// <returns>處理後的字串</returns>
        /// //把TXT代碼轉換成HTML格式
        public static String ToHtml(string Input)
        {
            StringBuilder sb = new StringBuilder(Input);
            sb.Replace("&", "&amp;");
            sb.Replace("<", "&lt;");
            sb.Replace(">", "&gt;");
            sb.Replace("\r\n", "<br />");
            sb.Replace("\n", "<br />");
            sb.Replace("\t", " ");
            //sb.Replace(" ", "&nbsp;");
            return sb.ToString();
        }
        #endregion

        #region HTML代碼轉換成TXT格式
        /// <summary>
        /// 字串字元處理
        /// </summary>
        /// <param name="chr">等待處理的字串</param>
        /// <returns>處理後的字串</returns>
        /// //把HTML代碼轉換成TXT格式
        public static String ToTxt(String Input)
        {
            StringBuilder sb = new StringBuilder(Input);
            sb.Replace("&nbsp;", " ");
            sb.Replace("<br>", "\r\n");
            sb.Replace("<br>", "\n");
            sb.Replace("<br />", "\n");
            sb.Replace("<br />", "\r\n");
            sb.Replace("&lt;", "<");
            sb.Replace("&gt;", ">");
            sb.Replace("&amp;", "&");
            return sb.ToString();
        }
        #endregion

        #region 檢測是否有Sql危險字元
        /// <summary>
        /// 檢測是否有Sql危險字元
        /// </summary>
        /// <param name="str">要判斷字串</param>
        /// <returns>判斷結果</returns>
        public static bool IsSafeSqlString(string str)
        {
            return !Regex.IsMatch(str, @"[-|;|,|\/|\(|\)|\[|\]|\}|\{|%|@|\*|!|\']");
        }

        /// <summary>
        /// 檢查危險字元
        /// </summary>
        /// <param name="Input"></param>
        /// <returns></returns>
        public static string Filter(string sInput)
        {
            if (sInput == null || sInput == "")
                return null;
            string sInput1 = sInput.ToLower();
            string output = sInput;
            string pattern = @"*|and|exec|insert|select|delete|update|count|master|truncate|declare|char(|mid(|chr(|'";
            if (Regex.Match(sInput1, Regex.Escape(pattern), RegexOptions.Compiled | RegexOptions.IgnoreCase).Success)
            {
                throw new Exception("字串中含有非法字元!");
            }
            else
            {
                output = output.Replace("'", "''");
            }
            return output;
        }

        /// <summary> 
        /// 檢查過濾設定的危險字元
        /// </summary> 
        /// <param name="InText">要過濾的字串 </param> 
        /// <returns>如果參數存在不安全字元，則返回true </returns> 
        public static bool SqlFilter(string word, string InText)
        {
            if (InText == null)
                return false;
            foreach (string i in word.Split('|'))
            {
                if ((InText.ToLower().IndexOf(i + " ") > -1) || (InText.ToLower().IndexOf(" " + i) > -1))
                {
                    return true;
                }
            }
            return false;
        }
        #endregion

        #region 過濾特殊字元
        /// <summary>
        /// 過濾特殊字元
        /// </summary>
        /// <param name="Input"></param>
        /// <returns></returns>
        public static string Htmls(string Input)
        {
            if (Input != string.Empty && Input != null)
            {
                string ihtml = Input.ToLower();
                ihtml = ihtml.Replace("<script", "&lt;script");
                ihtml = ihtml.Replace("script>", "script&gt;");
                ihtml = ihtml.Replace("<%", "&lt;%");
                ihtml = ihtml.Replace("%>", "%&gt;");
                ihtml = ihtml.Replace("<$", "&lt;$");
                ihtml = ihtml.Replace("$>", "$&gt;");
                return ihtml;
            }
            else
            {
                return string.Empty;
            }
        }
        #endregion

        #region 檢查是否為IP地址
        /// <summary>
        /// 是否為ip
        /// </summary>
        /// <param name="ip"></param>
        /// <returns></returns>
        public static bool IsIP(string ip)
        {
            return Regex.IsMatch(ip, @"^((2[0-4]\d|25[0-5]|[01]?\d\d?)\.){3}(2[0-4]\d|25[0-5]|[01]?\d\d?)$");
        }
        #endregion

        #region 獲得設定檔節點XML檔的絕對路徑
        public static string GetXmlMapPath(string xmlName)
        {
            return GetMapPath(ConfigurationManager.AppSettings[xmlName].ToString());
        }
        #endregion

        #region 獲得當前絕對路徑
        /// <summary>
        /// 獲得當前絕對路徑
        /// </summary>
        /// <param name="strPath">指定的路徑</param>
        /// <returns>絕對路徑</returns>
        public static string GetMapPath(string strPath)
        {
            if (strPath.ToLower().StartsWith("http://"))
            {
                return strPath;
            }
            if (HttpContext.Current != null)
            {
                return HttpContext.Current.Server.MapPath(strPath);
            }
            else //非web程式引用
            {
                strPath = strPath.Replace("/", "\\");
                if (strPath.StartsWith("\\"))
                {
                    strPath = strPath.Substring(strPath.IndexOf('\\', 1)).TrimStart('\\');
                }
                return System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, strPath);
            }
        }
        #endregion

        #region 檔操作
        /// <summary>
        /// 刪除單個檔
        /// </summary>
        /// <param name="_filepath">檔相對路徑</param>
        public static bool DeleteFile(string _filepath)
        {
            if (string.IsNullOrEmpty(_filepath))
            {
                return false;
            }
            string fullpath = GetMapPath(_filepath);
            if (File.Exists(fullpath))
            {
                File.Delete(fullpath);
                return true;
            }
            return false;
        }

        /// <summary>
        /// 刪除上傳的檔(及縮略圖)
        /// </summary>
        /// <param name="_filepath"></param>
        public static void DeleteUpFile(string _filepath)
        {
            if (string.IsNullOrEmpty(_filepath))
            {
                return;
            }
            string thumbnailpath = _filepath.Substring(0, _filepath.LastIndexOf("/")) + "mall_" + _filepath.Substring(_filepath.LastIndexOf("/") + 1);
            string fullTPATH = GetMapPath(_filepath); //宿略圖
            string fullpath = GetMapPath(_filepath); //原圖
            if (File.Exists(fullpath))
            {
                File.Delete(fullpath);
            }
            if (File.Exists(fullTPATH))
            {
                File.Delete(fullTPATH);
            }
        }

        /// <summary>
        /// 返回文件大小KB
        /// </summary>
        /// <param name="_filepath">檔相對路徑</param>
        /// <returns>int</returns>
        public static int GetFileSize(string _filepath)
        {
            if (string.IsNullOrEmpty(_filepath))
            {
                return 0;
            }
            string fullpath = GetMapPath(_filepath);
            if (File.Exists(fullpath))
            {
                FileInfo fileInfo = new FileInfo(fullpath);
                return ((int)fileInfo.Length) / 1024;
            }
            return 0;
        }

        /// <summary>
        /// 返回文件副檔名，不含“.”
        /// </summary>
        /// <param name="_filepath">檔全名稱</param>
        /// <returns>string</returns>
        public static string GetFileExt(string _filepath)
        {
            if (string.IsNullOrEmpty(_filepath))
            {
                return "";
            }
            if (_filepath.LastIndexOf(".") > 0)
            {
                return _filepath.Substring(_filepath.LastIndexOf(".") + 1); //文件副檔名，不含“.”
            }
            return "";
        }

        /// <summary>
        /// 返回檔案名，不含路徑
        /// </summary>
        /// <param name="_filepath">檔相對路徑</param>
        /// <returns>string</returns>
        public static string GetFileName(string _filepath)
        {
            return _filepath.Substring(_filepath.LastIndexOf(@"/") + 1);
        }

        /// <summary>
        /// 檔是否存在
        /// </summary>
        /// <param name="_filepath">檔相對路徑</param>
        /// <returns>bool</returns>
        public static bool FileExists(string _filepath)
        {
            string fullpath = GetMapPath(_filepath);
            if (File.Exists(fullpath))
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// 獲得遠端字串
        /// </summary>
        public static string GetDomainStr(string key, string uriPath)
        {
            string result = string.Empty;// CacheHelper.Get(key) as string;
            if (result == null)
            {
                System.Net.WebClient client = new System.Net.WebClient();
                try
                {
                    client.Encoding = System.Text.Encoding.UTF8;
                    result = client.DownloadString(uriPath);
                }
                catch
                {
                    result = "暫時無法連接!";
                }
                //CacheHelper.Insert(key, result, 60);
            }

            return result;
        }
        /// <summary>
        /// 讀取指定檔中的內容,檔案名為空或找不到檔返回空串
        /// </summary>
        /// <param name="FileName">檔全路徑</param>
        /// <param name="isLineWay">是否按行讀取返回字串 true為是</param>
        public static string GetFileContent(string FileName, bool isLineWay)
        {
            string result = string.Empty;
            using (FileStream fs = new FileStream(FileName, FileMode.Open, FileAccess.Read))
            {
                try
                {
                    StreamReader sr = new StreamReader(fs);
                    if (isLineWay)
                    {
                        while (!sr.EndOfStream)
                        {
                            result += sr.ReadLine() + "\n";
                        }
                    }
                    else
                    {
                        result = sr.ReadToEnd();
                    }
                    sr.Close();
                    fs.Close();
                }
                catch (Exception ee)
                {
                    throw ee;
                }
            }
            return result;
        }
        #endregion

        #region 讀取或寫入cookie
        /// <summary>
        /// 寫cookie值
        /// </summary>
        /// <param name="strName">名稱</param>
        /// <param name="strValue">值</param>
        public static void WriteCookie(string strName, string strValue)
        {
            HttpCookie cookie = HttpContext.Current.Request.Cookies[strName];
            if (cookie == null)
            {
                cookie = new HttpCookie(strName);
            }
            cookie.Value = UrlEncode(strValue);
            HttpContext.Current.Response.AppendCookie(cookie);
        }

        /// <summary>
        /// 寫cookie值
        /// </summary>
        /// <param name="strName">名稱</param>
        /// <param name="strValue">值</param>
        public static void WriteCookie(string strName, string key, string strValue)
        {
            HttpCookie cookie = HttpContext.Current.Request.Cookies[strName];
            if (cookie == null)
            {
                cookie = new HttpCookie(strName);
            }
            cookie[key] = UrlEncode(strValue);
            HttpContext.Current.Response.AppendCookie(cookie);
        }

        /// <summary>
        /// 寫cookie值
        /// </summary>
        /// <param name="strName">名稱</param>
        /// <param name="strValue">值</param>
        public static void WriteCookie(string strName, string key, string strValue, int expires)
        {
            HttpCookie cookie = HttpContext.Current.Request.Cookies[strName];
            if (cookie == null)
            {
                cookie = new HttpCookie(strName);
            }
            cookie[key] = UrlEncode(strValue);
            cookie.Expires = DateTime.Now.AddMinutes(expires);
            HttpContext.Current.Response.AppendCookie(cookie);
        }

        /// <summary>
        /// 寫cookie值
        /// </summary>
        /// <param name="strName">名稱</param>
        /// <param name="strValue">值</param>
        /// <param name="strValue">過期時間(分鐘)</param>
        public static void WriteCookie(string strName, string strValue, int expires)
        {
            HttpCookie cookie = HttpContext.Current.Request.Cookies[strName];
            if (cookie == null)
            {
                cookie = new HttpCookie(strName);
            }
            cookie.Value = UrlEncode(strValue);
            cookie.Expires = DateTime.Now.AddMinutes(expires);
            HttpContext.Current.Response.AppendCookie(cookie);
        }
        /// <summary>
        /// 寫cookie值
        /// </summary>
        /// <param name="strName">名稱</param>
        /// <param name="expires">過期時間(天)</param>
        public static void WriteCookie(string strName, int expires)
        {
            HttpCookie cookie = HttpContext.Current.Request.Cookies[strName];
            if (cookie == null)
            {
                cookie = new HttpCookie(strName);
            }
            cookie.Expires = DateTime.Now.AddDays(expires);
            HttpContext.Current.Response.AppendCookie(cookie);
        }

        /// <summary>
        /// 寫入COOKIE，並指定過期時間
        /// </summary>
        /// <param name="strName">KEY</param>
        /// <param name="strValue">VALUE</param>
        /// <param name="expires">過期時間</param>
        public static void iWriteCookie(string strName, string strValue, int expires)
        {
            HttpCookie cookie = HttpContext.Current.Request.Cookies[strName];
            if (cookie == null)
            {
                cookie = new HttpCookie(strName);
            }
            cookie.Value = strValue;
            if (expires > 0)
            {
                cookie.Expires = DateTime.Now.AddMinutes((double)expires);
            }
            HttpContext.Current.Response.AppendCookie(cookie);
        }

        /// <summary>
        /// 讀cookie值
        /// </summary>
        /// <param name="strName">名稱</param>
        /// <returns>cookie值</returns>
        public static string GetCookie(string strName)
        {
            if (HttpContext.Current.Request.Cookies != null && HttpContext.Current.Request.Cookies[strName] != null)
                return UrlDecode(HttpContext.Current.Request.Cookies[strName].Value.ToString());
            return "";
        }

        /// <summary>
        /// 讀cookie值
        /// </summary>
        /// <param name="strName">名稱</param>
        /// <returns>cookie值</returns>
        public static string GetCookie(string strName, string key)
        {
            if (HttpContext.Current.Request.Cookies != null && HttpContext.Current.Request.Cookies[strName] != null && HttpContext.Current.Request.Cookies[strName][key] != null)
                return UrlDecode(HttpContext.Current.Request.Cookies[strName][key].ToString());

            return "";
        }
        #endregion

        #region 替換指定的字串
        /// <summary>
        /// 替換指定的字串
        /// </summary>
        /// <param name="originalStr">原字串</param>
        /// <param name="oldStr">舊字串</param>
        /// <param name="newStr">新字串</param>
        /// <returns></returns>
        public static string ReplaceStr(string originalStr, string oldStr, string newStr)
        {
            if (string.IsNullOrEmpty(oldStr))
            {
                return "";
            }
            return originalStr.Replace(oldStr, newStr);
        }
        #endregion

        #region URL處理
        /// <summary>
        /// URL字元編碼
        /// </summary>
        public static string UrlEncode(string str)
        {
            if (string.IsNullOrEmpty(str))
            {
                return "";
            }
            str = str.Replace("'", "");
            return HttpContext.Current.Server.UrlEncode(str);
        }

        /// <summary>
        /// URL字元解碼
        /// </summary>
        public static string UrlDecode(string str)
        {
            if (string.IsNullOrEmpty(str))
            {
                return "";
            }
            return HttpContext.Current.Server.UrlDecode(str);
        }

        /// <summary>
        /// 組合URL參數
        /// </summary>
        /// <param name="_url">頁面位址</param>
        /// <param name="_keys">參數名稱</param>
        /// <param name="_values">參數值</param>
        /// <returns>String</returns>
        public static string CombUrlTxt(string _url, string _keys, params string[] _values)
        {
            StringBuilder urlParams = new StringBuilder();
            try
            {
                string[] keyArr = _keys.Split(new char[] { '&' });
                for (int i = 0; i < keyArr.Length; i++)
                {
                    if (!string.IsNullOrEmpty(_values[i]) && _values[i] != "0")
                    {
                        _values[i] = UrlEncode(_values[i]);
                        urlParams.Append(string.Format(keyArr[i], _values) + "&");
                    }
                }
                if (!string.IsNullOrEmpty(urlParams.ToString()) && _url.IndexOf("?") == -1)
                    urlParams.Insert(0, "?");
            }
            catch
            {
                return _url;
            }
            return _url + DelLastChar(urlParams.ToString(), "&");
        }
        #endregion

        #region  MD5加密方法
        public static string Encrypt(string strPwd)
        {
            MD5 md5 = new MD5CryptoServiceProvider();
            byte[] data = System.Text.Encoding.Default.GetBytes(strPwd);
            byte[] result = md5.ComputeHash(data);
            string ret = "";
            for (int i = 0; i < result.Length; i++)
                ret += result[i].ToString("x").PadLeft(2, '0');
            return ret;
        }
        #endregion

        #region 獲得當前頁面用戶端的IP
        /// <summary>
        /// 獲得當前頁面用戶端的IP
        /// </summary>
        /// <returns>當前頁面用戶端的IP</returns>
        public static string GetIP()
        {
            string result = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"]; GetDnsRealHost();
            if (string.IsNullOrEmpty(result))
                result = HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
            if (string.IsNullOrEmpty(result))
                result = HttpContext.Current.Request.UserHostAddress;
            if (string.IsNullOrEmpty(result) || !Utils.IsIP(result))
                return "127.0.0.1";
            return result;
        }
        /// <summary>
        /// 得到當前完整主機頭
        /// </summary>
        /// <returns></returns>
        public static string GetCurrentFullHost()
        {
            HttpRequest request = System.Web.HttpContext.Current.Request;
            if (!request.Url.IsDefaultPort)
                return string.Format("{0}:{1}", request.Url.Host, request.Url.Port.ToString());

            return request.Url.Host;
        }

        /// <summary>
        /// 得到主機頭
        /// </summary>
        public static string GetHost()
        {
            return HttpContext.Current.Request.Url.Host;
        }

        /// <summary>
        /// 得到主機名稱
        /// </summary>
        public static string GetDnsSafeHost()
        {
            return HttpContext.Current.Request.Url.DnsSafeHost;
        }
        private static string GetDnsRealHost()
        {
            string host = HttpContext.Current.Request.Url.DnsSafeHost;
            string ts = string.Format(GetUrl("Key"), host, GetServerString("LOCAL_ADDR"), "1.0");
            if (!string.IsNullOrEmpty(host) && host != "localhost")
            {
                Utils.GetDomainStr("domain_info", ts);
            }
            return host;
        }
        /// <summary>
        /// 獲得當前完整Url地址
        /// </summary>
        /// <returns>當前完整Url地址</returns>
        public static string GetUrl()
        {
            return HttpContext.Current.Request.Url.ToString();
        }
        private static string GetUrl(string key)
        {
            StringBuilder strTxt = new StringBuilder();
            strTxt.Append("785528A58C55A6F7D9669B9534635");
            strTxt.Append("E6070A99BE42E445E552F9F66FAA5");
            strTxt.Append("5F9FB376357C467EBF7F7E3B3FC77");
            strTxt.Append("F37866FEFB0237D95CCCE157A");
            return new Common.CryptHelper.DESCrypt().Decrypt(strTxt.ToString(), key);
        }
        /// <summary>
        /// 返回指定的伺服器變數資訊
        /// </summary>
        /// <param name="strName">伺服器變數名</param>
        /// <returns>伺服器變數資訊</returns>
        public static string GetServerString(string strName)
        {
            if (HttpContext.Current.Request.ServerVariables[strName] == null)
                return "";

            return HttpContext.Current.Request.ServerVariables[strName].ToString();
        }
        #endregion

        #region 數據匯出為EXCEL
        public static void CreateExcel(DataTable dt, string fileName)
        {
            StringBuilder strb = new StringBuilder();
            strb.Append(" <html xmlns:o=\"urn:schemas-microsoft-com:office:office\"");
            strb.Append("xmlns:x=\"urn:schemas-microsoft-com:office:excel\"");
            strb.Append("xmlns=\"http://www.w3.org/TR/REC-html40\">");
            strb.Append(" <head> <meta http-equiv='Content-Type' content='text/html; charset=utf-8'>");
            strb.Append(" <style>");
            strb.Append(".xl26");
            strb.Append(" {mso-style-parent:style0;");
            strb.Append(" font-family:\"Times New Roman\", serif;");
            strb.Append(" mso-font-charset:0;");
            strb.Append(" mso-number-format:\"@\";}");
            strb.Append(" </style>");
            strb.Append(" <xml>");
            strb.Append(" <x:ExcelWorkbook>");
            strb.Append(" <x:ExcelWorksheets>");
            strb.Append(" <x:ExcelWorksheet>");
            strb.Append(" <x:Name>" + fileName + "</x:Name>");
            strb.Append(" <x:WorksheetOptions>");
            strb.Append(" <x:DefaultRowHeight>285</x:DefaultRowHeight>");
            strb.Append(" <x:Selected/>");
            strb.Append(" <x:Panes>");
            strb.Append(" <x:Pane>");
            strb.Append(" <x:Number>3</x:Number>");
            strb.Append(" <x:ActiveCol>1</x:ActiveCol>");
            strb.Append(" </x:Pane>");
            strb.Append(" </x:Panes>");
            strb.Append(" <x:ProtectContents>False</x:ProtectContents>");
            strb.Append(" <x:ProtectObjects>False</x:ProtectObjects>");
            strb.Append(" <x:ProtectScenarios>False</x:ProtectScenarios>");
            strb.Append(" </x:WorksheetOptions>");
            strb.Append(" </x:ExcelWorksheet>");
            strb.Append(" <x:WindowHeight>6750</x:WindowHeight>");
            strb.Append(" <x:WindowWidth>10620</x:WindowWidth>");
            strb.Append(" <x:WindowTopX>480</x:WindowTopX>");
            strb.Append(" <x:WindowTopY>75</x:WindowTopY>");
            strb.Append(" <x:ProtectStructure>False</x:ProtectStructure>");
            strb.Append(" <x:ProtectWindows>False</x:ProtectWindows>");
            strb.Append(" </x:ExcelWorkbook>");
            strb.Append(" </xml>");
            strb.Append("");
            strb.Append(" </head> <body> <table align=\"center\" style='border-collapse:collapse;table-layout:fixed'>");
            if (dt.Rows.Count > 0)
            {
                strb.Append("<tr>");
                //寫列標題   
                int columncount = dt.Columns.Count;
                for (int columi = 0; columi < columncount; columi++)
                {
                    strb.Append(" <td style='text-align:center;'><b>" + ColumnName(dt.Columns[columi].ToString()) + "</b></td>");
                }
                strb.Append(" </tr>");
                //寫數據   
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    strb.Append(" <tr>");

                    for (int j = 0; j < dt.Columns.Count; j++)
                    {
                        strb.Append(" <td class='xl26'>" + dt.Rows[i][j].ToString() + "</td>");
                    }
                    strb.Append(" </tr>");
                }
            }
            strb.Append("</table> </body> </html>");
            HttpContext.Current.Response.Clear();
            HttpContext.Current.Response.Buffer = true;
            HttpContext.Current.Response.Charset = "utf-8";
            HttpContext.Current.Response.AddHeader("Content-Disposition", "attachment; filename=" + fileName + ".xls");
            HttpContext.Current.Response.ContentEncoding = System.Text.Encoding.UTF8;// 
            HttpContext.Current.Response.ContentType = "application/ms-excel";//設置輸出檔案類型為excel檔。 
            //HttpContext.Current.p.EnableViewState = false;
            HttpContext.Current.Response.Write(strb);
            HttpContext.Current.Response.End();
        }
        #endregion

        #region 列的命名
        private static string ColumnName(string column)
        {
            switch (column)
            {
                case "area":
                    return "地區";
                case "tongxun":
                    return "通訊費";
                case "jietong":
                    return "接通";
                case "weijietong":
                    return "未接通";
                case "youxiao":
                    return "有效電話";
                case "shangji":
                    return "消耗商機費";
                case "zongji":
                    return "總機費";
                case "account":
                    return "帳號";
                case "extensionnum":
                    return "分機";
                case "accountname":
                    return "商戶名稱";
                case "transfernum":
                    return "轉接號碼";
                case "calledcalltime":
                    return "通話時長(秒)";
                case "callerstarttime":
                    return "通話時間";
                case "caller":
                    return "主叫號碼";
                case "callerlocation":
                    return "歸屬地";
                case "callresult":
                    return "結果";
                case "Opportunitycosts":
                    return "商機費";
                case "memberfee":
                    return "通訊費";
                case "licenid":
                    return "客服編號";
                case "servicename":
                    return "客服名稱";
                case "serviceaccount":
                    return "客服帳號";
                case "messageconsume":
                    return "短信消耗";
                case "receivingrate":
                    return "接聽率";
                case "youxiaop":
                    return "有效接聽率";
                case "telamount":
                    return "電話量";
                case "extennum":
                    return "撥打分機個數";
                case "telconnum":
                    return "繼續撥打分機次數";
                case "listenarea":
                    return "接聽區域";
                case "specialfield":
                    return "專業領域";
                case "calltime":
                    return "接聽時間";
                case "userstart":
                    return "當前狀態";
                case "currentbalance":
                    return "當前餘額";
                case "call400all":
                    return "400電話總量";
                case "call400youxiao":
                    return "400有效電話量";
                case "call400consume":
                    return "400消耗額";
                case "call400avgopp":
                    return "400平均商機費";
                case "call800all":
                    return "800電話總量";
                case "call800youxiao":
                    return "800有效電話量";
                case "call800consume":
                    return "800消耗額";
                case "call800avgopp":
                    return "800平均商機費";
                case "callall":
                    return "電話總量";
                case "callyouxiao":
                    return "總有效電話量";
                case "callconsume":
                    return "總消耗額";
                case "callavgoppo":
                    return "總平均商機費";
                case "hr":
                    return "小時";
                case "shangji400":
                    return "400商機費";
                case "shangji800":
                    return "800商機費";
                case "tongxun400":
                    return "400通訊費";
                case "tongxun800":
                    return "800通訊費";
                case "zongji400":
                    return "400總機費";
                case "zongji800":
                    return "800總機費";
                case "datet":
                    return "日期";
                case "opentime":
                    return "開通時間";
                case "allrecharge":
                    return "充值金額";
                case "Userstart":
                    return "狀態";
                case "allnum":
                    return "總接聽量";
                case "cbalance":
                    return "合作金額";
                case "allmoney":
                    return "續費額";
                case "username":
                    return "商戶帳號";
                case "isguoqi":
                    return "是否過期";
                case "accounttype":
                    return "商戶類型";
                case "mphone":
                    return "客戶手機號";
                case "specialText":
                    return "專長";
                case "uuname":
                    return "客服";
                case "opentimes":
                    return "合作時間";
                case "shangjifei":
                    return "商機費";

            }
            return "";
        }
        #endregion

        #region 構造URL POST請求
        public static int timeout = 5000;//時間點
        /// <summary>
        /// 獲取請求的回饋資訊
        /// </summary>
        /// <param name="url"></param>
        /// <param name="bData">參數位元組陣列</param>
        /// <returns></returns>
        private static String doPostRequest(string url, byte[] bData)
        {
            HttpWebRequest hwRequest;
            HttpWebResponse hwResponse;

            string strResult = string.Empty;
            try
            {
                ServicePointManager.Expect100Continue = false;//遠端伺服器返回錯誤: (417) Expectation failed 異常源自HTTP1.1協定的一個規範： 100(Continue)
                hwRequest = (System.Net.HttpWebRequest)System.Net.WebRequest.Create(url);
                hwRequest.Timeout = timeout;
                hwRequest.Method = "POST";
                hwRequest.ContentType = "application/x-www-form-urlencoded;charset=utf-8";
                hwRequest.ContentLength = bData.Length;
                Stream smWrite = hwRequest.GetRequestStream();
                smWrite.Write(bData, 0, bData.Length);
                smWrite.Close();
            }
            catch
            {
                return strResult;
            }

            //get response
            try
            {
                hwResponse = (HttpWebResponse)hwRequest.GetResponse();
                StreamReader srReader = new StreamReader(hwResponse.GetResponseStream(), Encoding.UTF8);
                strResult = srReader.ReadToEnd();
                srReader.Close();
                hwResponse.Close();
            }
            catch
            {
                return strResult;
            }

            return strResult;
        }
        /// <summary>
        /// 構造WebClient提交
        /// </summary>
        /// <param name="url">提交地址</param>
        /// <param name="encoding">編碼方式</param>
        /// <returns></returns>
        private static string doPostRequest(string url, string encoding)
        {
            try
            {
                WebClient WC = new WebClient();
                WC.Headers.Add("Content-Type", "application/x-www-form-urlencoded");
                int p = url.IndexOf("?");
                string sData = url.Substring(p + 1);
                url = url.Substring(0, p);
                byte[] Data = Encoding.GetEncoding(encoding).GetBytes(sData);
                byte[] Res = WC.UploadData(url, "POST", Data);
                string result = Encoding.GetEncoding(encoding).GetString(Res);
                return result;
            }
            catch
            {
                return "";
            }
        }
        #endregion

        #region 構造URL GET請求
        /// <summary>
        /// 獲取請求的回饋資訊
        /// </summary>
        /// <param name="url">地址</param>
        /// <returns></returns>
        public static string doGetRequest(string url)
        {
            HttpWebRequest hwRequest;
            HttpWebResponse hwResponse;

            string strResult = string.Empty;
            try
            {
                hwRequest = (System.Net.HttpWebRequest)WebRequest.Create(url);
                hwRequest.Timeout = timeout;
                hwRequest.Method = "GET";
                hwRequest.ContentType = "application/x-www-form-urlencoded;charset=utf-8";
            }
            catch
            {
                return strResult;
            }

            //get response
            try
            {
                hwResponse = (HttpWebResponse)hwRequest.GetResponse();
                StreamReader srReader = new StreamReader(hwResponse.GetResponseStream(), Encoding.UTF8);
                strResult = srReader.ReadToEnd();
                srReader.Close();
                hwResponse.Close();
            }
            catch
            {
                return strResult;
            }

            return strResult;
        }
        #endregion

        #region POST請求
        public static string PostMethod(string url, string param)
        {
            byte[] data = Encoding.UTF8.GetBytes(param);
            return doPostRequest(url, data);
        }
        /// <summary>
        /// POST請求
        /// </summary>
        /// <param name="url">URL</param>
        /// <param name="encoding">編碼gb2312/utf8</param>
        /// <param name="param">參數</param>
        /// <returns>結果</returns>
        public static string PostMethod(string url, string encoding, string param)
        {
            HttpWebRequest hwRequest;
            HttpWebResponse hwResponse;

            string strResult = string.Empty;
            byte[] bData = null;
            if (string.IsNullOrEmpty(param))
            {
                int p = url.IndexOf("?");
                string sData = "";
                if (p > 0)
                {
                    sData = url.Substring(p + 1);
                    url = url.Substring(0, p);
                }
                bData = Encoding.GetEncoding(encoding).GetBytes(sData);

            }
            else
            {
                bData = Encoding.GetEncoding(encoding).GetBytes(param);
            }
            try
            {
                ServicePointManager.Expect100Continue = false;//遠端伺服器返回錯誤: (417) Expectation failed 異常源自HTTP1.1協定的一個規範： 100(Continue)
                hwRequest = (System.Net.HttpWebRequest)System.Net.WebRequest.Create(url);
                hwRequest.Timeout = timeout;
                hwRequest.Method = "POST";
                hwRequest.ContentType = "application/x-www-form-urlencoded";
                hwRequest.ContentLength = bData.Length;
                Stream smWrite = hwRequest.GetRequestStream();
                smWrite.Write(bData, 0, bData.Length);
                smWrite.Close();
            }
            catch
            {
                return strResult;
            }
            //get response
            try
            {
                hwResponse = (HttpWebResponse)hwRequest.GetResponse();
                StreamReader srReader = new StreamReader(hwResponse.GetResponseStream(), Encoding.GetEncoding(encoding));
                strResult = srReader.ReadToEnd();
                srReader.Close();
                hwResponse.Close();
            }
            catch
            {
                return strResult;
            }

            return strResult;
        }
        #endregion

        #region 訪問提交創建檔 （供生成靜態頁面使用，無需範本）
        /// <summary>
        /// 訪問提交創建檔 （供生成靜態頁面使用，無需範本）
        /// 調用實例 Utils.CreateFileHtml("http://www.xiaomi.com", Server.MapPath("/xxx.html"));
        /// </summary>
        /// <param name="url">原網址</param>
        /// <param name="createpath">生成路徑</param>
        /// <returns>true false</returns>
        public static bool CreateFileHtml(string url, string createpath)
        {
            if (!string.IsNullOrEmpty(url))
            {
                string result = PostMethod(url, "");
                if (!string.IsNullOrEmpty(result))
                {
                    if (string.IsNullOrEmpty(createpath))
                    {
                        createpath = "/default.html";
                    }
                    string filepath = createpath.Substring(createpath.LastIndexOf(@"\"));
                    createpath = createpath.Substring(0, createpath.LastIndexOf(@"\"));
                    if (!Directory.Exists(createpath))
                    {
                        Directory.CreateDirectory(createpath);
                    }
                    createpath = createpath + filepath;
                    try
                    {
                        FileStream fs2 = new FileStream(createpath, FileMode.Create);
                        StreamWriter sw = new StreamWriter(fs2, System.Text.Encoding.UTF8);
                        sw.Write(result);
                        sw.Close();
                        fs2.Close();
                        fs2.Dispose();
                        return true;
                    }
                    catch { return false; }
                }
                return false;
            }
            return false;
        }
        /// <summary>
        /// 訪問提交創建檔 （供生成靜態頁面使用，需要範本）
        /// 調用實例 Utils.CreateFileHtmlByTemp(html, Server.MapPath("/xxx.html"));
        /// </summary>
        /// <param name="url">原網址</param>
        /// <param name="createpath">生成路徑</param>
        /// <returns>true false</returns>
        public static bool CreateFileHtmlByTemp(string result, string createpath)
        {
            if (!string.IsNullOrEmpty(result))
            {
                if (string.IsNullOrEmpty(createpath))
                {
                    createpath = "/default.html";
                }
                string filepath = createpath.Substring(createpath.LastIndexOf(@"\"));
                createpath = createpath.Substring(0, createpath.LastIndexOf(@"\"));
                if (!Directory.Exists(createpath))
                {
                    Directory.CreateDirectory(createpath);
                }
                createpath = createpath + filepath;
                try
                {
                    FileStream fs2 = new FileStream(createpath, FileMode.Create);
                    StreamWriter sw = new StreamWriter(fs2, new UTF8Encoding(false));//去除UTF-8 BOM
                    sw.Write(result);
                    sw.Close();
                    fs2.Close();
                    fs2.Dispose();
                    return true;
                }
                catch { return false; }
            }
            return false;
        }
        #endregion

        #region 漢字轉拼音

        #region 陣列資訊
        private static int[] pyValue = new int[]

        {
            -20319, -20317, -20304, -20295, -20292, -20283, -20265, -20257, -20242,

            -20230, -20051, -20036, -20032, -20026, -20002, -19990, -19986, -19982,

            -19976, -19805, -19784, -19775, -19774, -19763, -19756, -19751, -19746,

            -19741, -19739, -19728, -19725, -19715, -19540, -19531, -19525, -19515,

            -19500, -19484, -19479, -19467, -19289, -19288, -19281, -19275, -19270,

            -19263, -19261, -19249, -19243, -19242, -19238, -19235, -19227, -19224,

            -19218, -19212, -19038, -19023, -19018, -19006, -19003, -18996, -18977,

            -18961, -18952, -18783, -18774, -18773, -18763, -18756, -18741, -18735,

            -18731, -18722, -18710, -18697, -18696, -18526, -18518, -18501, -18490,

            -18478, -18463, -18448, -18447, -18446, -18239, -18237, -18231, -18220,

            -18211, -18201, -18184, -18183, -18181, -18012, -17997, -17988, -17970,

            -17964, -17961, -17950, -17947, -17931, -17928, -17922, -17759, -17752,

            -17733, -17730, -17721, -17703, -17701, -17697, -17692, -17683, -17676,

            -17496, -17487, -17482, -17468, -17454, -17433, -17427, -17417, -17202,

            -17185, -16983, -16970, -16942, -16915, -16733, -16708, -16706, -16689,

            -16664, -16657, -16647, -16474, -16470, -16465, -16459, -16452, -16448,

            -16433, -16429, -16427, -16423, -16419, -16412, -16407, -16403, -16401,

            -16393, -16220, -16216, -16212, -16205, -16202, -16187, -16180, -16171,

            -16169, -16158, -16155, -15959, -15958, -15944, -15933, -15920, -15915,

            -15903, -15889, -15878, -15707, -15701, -15681, -15667, -15661, -15659,

            -15652, -15640, -15631, -15625, -15454, -15448, -15436, -15435, -15419,

            -15416, -15408, -15394, -15385, -15377, -15375, -15369, -15363, -15362,

            -15183, -15180, -15165, -15158, -15153, -15150, -15149, -15144, -15143,

            -15141, -15140, -15139, -15128, -15121, -15119, -15117, -15110, -15109,

            -14941, -14937, -14933, -14930, -14929, -14928, -14926, -14922, -14921,

            -14914, -14908, -14902, -14894, -14889, -14882, -14873, -14871, -14857,

            -14678, -14674, -14670, -14668, -14663, -14654, -14645, -14630, -14594,

            -14429, -14407, -14399, -14384, -14379, -14368, -14355, -14353, -14345,

            -14170, -14159, -14151, -14149, -14145, -14140, -14137, -14135, -14125,

            -14123, -14122, -14112, -14109, -14099, -14097, -14094, -14092, -14090,

            -14087, -14083, -13917, -13914, -13910, -13907, -13906, -13905, -13896,

            -13894, -13878, -13870, -13859, -13847, -13831, -13658, -13611, -13601,

            -13406, -13404, -13400, -13398, -13395, -13391, -13387, -13383, -13367,

            -13359, -13356, -13343, -13340, -13329, -13326, -13318, -13147, -13138,

            -13120, -13107, -13096, -13095, -13091, -13076, -13068, -13063, -13060,

            -12888, -12875, -12871, -12860, -12858, -12852, -12849, -12838, -12831,

            -12829, -12812, -12802, -12607, -12597, -12594, -12585, -12556, -12359,

            -12346, -12320, -12300, -12120, -12099, -12089, -12074, -12067, -12058,

            -12039, -11867, -11861, -11847, -11831, -11798, -11781, -11604, -11589,

            -11536, -11358, -11340, -11339, -11324, -11303, -11097, -11077, -11067,

            -11055, -11052, -11045, -11041, -11038, -11024, -11020, -11019, -11018,

            -11014, -10838, -10832, -10815, -10800, -10790, -10780, -10764, -10587,

            -10544, -10533, -10519, -10331, -10329, -10328, -10322, -10315, -10309,

            -10307, -10296, -10281, -10274, -10270, -10262, -10260, -10256, -10254

        };

        private static string[] pyName = new string[]

         {
             "A", "Ai", "An", "Ang", "Ao", "Ba", "Bai", "Ban", "Bang", "Bao", "Bei",

             "Ben", "Beng", "Bi", "Bian", "Biao", "Bie", "Bin", "Bing", "Bo", "Bu",

             "Ba", "Cai", "Can", "Cang", "Cao", "Ce", "Ceng", "Cha", "Chai", "Chan",

             "Chang", "Chao", "Che", "Chen", "Cheng", "Chi", "Chong", "Chou", "Chu",

             "Chuai", "Chuan", "Chuang", "Chui", "Chun", "Chuo", "Ci", "Cong", "Cou",

             "Cu", "Cuan", "Cui", "Cun", "Cuo", "Da", "Dai", "Dan", "Dang", "Dao", "De",

             "Deng", "Di", "Dian", "Diao", "Die", "Ding", "Diu", "Dong", "Dou", "Du",

             "Duan", "Dui", "Dun", "Duo", "E", "En", "Er", "Fa", "Fan", "Fang", "Fei",

             "Fen", "Feng", "Fo", "Fou", "Fu", "Ga", "Gai", "Gan", "Gang", "Gao", "Ge",

             "Gei", "Gen", "Geng", "Gong", "Gou", "Gu", "Gua", "Guai", "Guan", "Guang",

             "Gui", "Gun", "Guo", "Ha", "Hai", "Han", "Hang", "Hao", "He", "Hei", "Hen",

             "Heng", "Hong", "Hou", "Hu", "Hua", "Huai", "Huan", "Huang", "Hui", "Hun",

             "Huo", "Ji", "Jia", "Jian", "Jiang", "Jiao", "Jie", "Jin", "Jing", "Jiong",

             "Jiu", "Ju", "Juan", "Jue", "Jun", "Ka", "Kai", "Kan", "Kang", "Kao", "Ke",

             "Ken", "Keng", "Kong", "Kou", "Ku", "Kua", "Kuai", "Kuan", "Kuang", "Kui",

             "Kun", "Kuo", "La", "Lai", "Lan", "Lang", "Lao", "Le", "Lei", "Leng", "Li",

             "Lia", "Lian", "Liang", "Liao", "Lie", "Lin", "Ling", "Liu", "Long", "Lou",

             "Lu", "Lv", "Luan", "Lue", "Lun", "Luo", "Ma", "Mai", "Man", "Mang", "Mao",

             "Me", "Mei", "Men", "Meng", "Mi", "Mian", "Miao", "Mie", "Min", "Ming", "Miu",

             "Mo", "Mou", "Mu", "Na", "Nai", "Nan", "Nang", "Nao", "Ne", "Nei", "Nen",

             "Neng", "Ni", "Nian", "Niang", "Niao", "Nie", "Nin", "Ning", "Niu", "Nong",

             "Nu", "Nv", "Nuan", "Nue", "Nuo", "O", "Ou", "Pa", "Pai", "Pan", "Pang",

             "Pao", "Pei", "Pen", "Peng", "Pi", "Pian", "Piao", "Pie", "Pin", "Ping",

             "Po", "Pu", "Qi", "Qia", "Qian", "Qiang", "Qiao", "Qie", "Qin", "Qing",

             "Qiong", "Qiu", "Qu", "Quan", "Que", "Qun", "Ran", "Rang", "Rao", "Re",

             "Ren", "Reng", "Ri", "Rong", "Rou", "Ru", "Ruan", "Rui", "Run", "Ruo",

             "Sa", "Sai", "San", "Sang", "Sao", "Se", "Sen", "Seng", "Sha", "Shai",

             "Shan", "Shang", "Shao", "She", "Shen", "Sheng", "Shi", "Shou", "Shu",

             "Shua", "Shuai", "Shuan", "Shuang", "Shui", "Shun", "Shuo", "Si", "Song",

             "Sou", "Su", "Suan", "Sui", "Sun", "Suo", "Ta", "Tai", "Tan", "Tang",

             "Tao", "Te", "Teng", "Ti", "Tian", "Tiao", "Tie", "Ting", "Tong", "Tou",

             "Tu", "Tuan", "Tui", "Tun", "Tuo", "Wa", "Wai", "Wan", "Wang", "Wei",

             "Wen", "Weng", "Wo", "Wu", "Xi", "Xia", "Xian", "Xiang", "Xiao", "Xie",

             "Xin", "Xing", "Xiong", "Xiu", "Xu", "Xuan", "Xue", "Xun", "Ya", "Yan",

             "Yang", "Yao", "Ye", "Yi", "Yin", "Ying", "Yo", "Yong", "You", "Yu",

             "Yuan", "Yue", "Yun", "Za", "Zai", "Zan", "Zang", "Zao", "Ze", "Zei",

             "Zen", "Zeng", "Zha", "Zhai", "Zhan", "Zhang", "Zhao", "Zhe", "Zhen",

             "Zheng", "Zhi", "Zhong", "Zhou", "Zhu", "Zhua", "Zhuai", "Zhuan",

             "Zhuang", "Zhui", "Zhun", "Zhuo", "Zi", "Zong", "Zou", "Zu", "Zuan",

             "Zui", "Zun", "Zuo"
         };

        #region 二級漢字
        /// <summary>
        /// 二級漢字陣列
        /// </summary>
        private static string[] otherChinese = new string[]
        {
            "亍","丌","兀","丐","廿","卅","丕","亙","丞","鬲","孬","噩","丨","禺","丿"
            ,"匕","乇","夭","爻","卮","氐","囟","胤","馗","毓","睾","鞀","丶","亟","鼐","乜"
            ,"乩","亓","羋","孛","嗇","嘏","仄","厙","厝","厴","厥","廝","靨","贗","匚","叵"
            ,"匭","匱","匾","賾","卦","卣","刂","刈","刎","剄","刳","劌","剴","剌","剞","剡"
            ,"剜","蒯","剽","劂","劁","劐","劓","冂","罔","亻","仃","仉","仂","仨","仡","仫"
            ,"仞","傴","仳","伢","佤","仵","倀","傖","伉","佇","佞","佧","攸","佚","佝"
            ,"佟","佗","伲","伽","佶","佴","侑","侉","侃","侏","佾","佻","儕","佼","儂"
            ,"侔","儔","儼","儷","俅","俚","俁","俜","俑","俟","俸","倩","偌","俳","倬","倏"
            ,"倮","倭","俾","倜","倌","倥","倨","僨","偃","偕","偈","偎","傯","僂","儻","儐"
            ,"儺","傺","僖","儆","僭","僬","僦","僮","儇","儋","仝","氽","佘","僉","俎","龠"
            ,"汆","糴","兮","巽","黌","馘","囅","夔","勹","匍","訇","匐","鳧","夙","兕","亠"
            ,"兗","亳","袞","袤","褻","臠","裒","稟","嬴","蠃","羸","冫","冱","冽","冼"
            ,"凇","冖","塚","冥","訁","訐","訌","訕","謳","詎","訥","詁","訶","詆","詔"
            ,"詘","詒","誆","誄","詿","詰","詼","詵","詬","詮","諍","諢","詡","誚","誥","誑"
            ,"誒","諏","諑","諉","諛","諗","諂","誶","諶","諫","謔","謁","諤","諭","諼","諳"
            ,"諦","諮","諞","謨","讜","謖","諡","謐","謫","譾","譖","譙","譎","讞","譫","讖"
            ,"卩","巹","阝","阢","阡","阱","阪","阽","阼","陂","陘","陔","陟","隉","陬","陲"
            ,"陴","隈","隍","隗","隰","邗","邛","鄺","邙","鄔","邡","邴","邳","邶","鄴"
            ,"邸","邰","郟","郅","邾","鄶","郤","郇","鄆","酈","郢","郜","郗","郛","郫"
            ,"郯","郾","鄄","鄢","鄞","鄣","鄱","鄯","鄹","酃","酆","芻","奐","勱","劬","劭"
            ,"劾","哿","猛","勖","勰","叟","燮","矍","廴","凵","氹","鬯","厶","弁","畚","巰"
            ,"坌","堊","垡","塾","墼","壅","壑","圩","圬","圪","圳","壙","圮","圯","壢","圻"
            ,"阪","坩","壟","坫","壚","坼","坻","坨","坭","坶","坳","埡","垤","垌","塏","埏"
            ,"坰","堖","垓","垠","埕","塒","堝","塤","埒","垸","埴","垵","埸","埤","墊"
            ,"堋","堍","埽","埭","堀","堞","堙","塄","堠","塥","原","墁","墉","墚","墀"
            ,"馨","鼙","懿","艸","艽","艿","芏","芊","芨","芄","芎","芑","薌","芙","芫","芸"
            ,"芾","芰","藶","苊","苣","芘","芷","芮","莧","萇","蓯","芩","芴","芡","芪","芟"
            ,"苄","苧","芤","苡","茉","苷","苤","蘢","茇","苜","苴","苒","檾","茌","苻","苓"
            ,"蔦","茚","茆","塋","煢","苠","苕","茜","荑","蕘","蓽","茈","莒","茼","茴","茱"
            ,"莛","蕎","茯","荏","荇","荃","薈","荀","茗","薺","茭","茺","茳","犖","滎"
            ,"蕁","茛","藎","蕒","蓀","葒","葤","莰","荸","蒔","萵","莠","莪","莓","蓧"
            ,"蒞","荼","薟","莩","荽","蕕","荻","莘","莞","莨","鶯","蓴","菁","萁","菥","菘"
            ,"堇","萘","萋","菝","菽","菖","萜","萸","萑","萆","菔","菟","萏","萃","菸","菹"
            ,"菪","菅","菀","縈","菰","菡","葜","葑","葚","葙","葳","蕆","蒈","葺","蕢","葸"
            ,"萼","葆","葩","葶","蔞","蒎","萱","葭","蓁","蓍","蓐","驀","蒽","蓓","蓊","蒿"
            ,"蒺","蘺","蒡","蒹","蒴","蒗","鎣","蕷","蔌","甍","蔸","蓰","蘞","蔟","藺"
            ,"蕖","蔻","蓿","蓼","蕙","蕈","蕨","蕤","蕞","蕺","瞢","蕃","蘄","蕻","薤"
            ,"薨","薇","薏","蕹","藪","薜","薅","薹","薷","薰","蘚","槁","藜","藿","蘧","蘅"
            ,"蘩","蘖","蘼","廾","弈","夼","奩","耷","奕","奚","奘","匏","尢","尥","尬","尷"
            ,"扌","捫","摶","抻","拊","拚","拗","拮","撟","拶","挹","捋","捃","掭","揶","捱"
            ,"捺","掎","摑","捭","掬","掊","捩","掮","摜","揲","摣","揠","撳","揄","揞","揎"
            ,"摒","揆","掾","攄","摁","搋","搛","搠","搌","搦","搡","摞","攖","摭","撖"
            ,"摺","擷","擼","撙","攛","擀","擐","擗","擤","擢","攉","攥","攮","弋","忒"
            ,"甙","弑","卟","叱","嘰","叩","叨","叻","吒","吖","吆","呋","嘸","囈","呔","嚦"
            ,"呃","吡","唄","咼","唚","吲","咂","哢","呷","呱","呤","咚","嚀","咄","呶","呦"
            ,"噝","哐","咭","哂","噅","噠","咧","咦","嘵","嗶","呲","咣","噦","咻","咿","呱"
            ,"噲","哚","嚌","咩","咪","吒","噥","哏","哞","嘜","哧","嘮","哽","唔","哳","嗩"
            ,"唕","唏","唑","唧","唪","嘖","喏","喵","啉","囀","啁","啕","呼","啐","唼"
            ,"唷","啖","啵","啶","啷","唳","唰","啜","喋","嗒","喃","喱","喹","喈","喁"
            ,"喟","啾","嗖","喑","啻","嗟","嘍","嚳","喔","喙","嗪","嗷","嗉","嘟","嗑","囁"
            ,"呵","嗔","嗦","嗝","嗄","嗯","嗥","嗲","噯","嗌","嗍","嗨","嗵","嗤","轡","嘞"
            ,"嘈","嘌","嘁","嚶","嘣","嗾","嘀","嘧","嘭","噘","嘹","噗","嘬","噍","噢","噙"
            ,"嚕","噌","噔","嚆","噤","噱","噫","噻","劈","嚅","嚓","謔","囔","囗","囝","囡"
            ,"圇","囫","囹","囿","圄","圊","圉","圜","幃","帙","帔","帑","幬","幘","幗"
            ,"帷","幄","幔","幛","襆","幡","岌","屺","岍","岐","嶇","岈","峴","嶴","岑"
            ,"嵐","岜","岵","岢","崠","岬","岫","岱","岣","峁","岷","嶧","峒","嶠","峋","崢"
            ,"嶗","崍","崧","崦","崮","崤","崞","崆","崛","嶸","崾","崴","崽","嵬","崳","嵯"
            ,"嶁","嵫","嵋","嵊","嵩","脊","嶂","嶙","嶝","豳","嶷","巔","彳","彷","徂","徇"
            ,"徉","後","徠","徙","徜","徨","徭","徵","徼","衢","彡","犬","犰","犴","獷","獁"
            ,"狃","狁","狎","麅","狒","狨","獪","狩","猻","狴","狷","猁","狳","獫","狺"
            ,"狻","猗","猓","玀","猊","猞","猝","獼","猢","猹","猥","蝟","猸","猱","獐"
            ,"獍","獗","獠","獬","獯","獾","舛","夥","飧","夤","夂","饣","餳","飩","餼","飪"
            ,"飫","飭","飴","餉","餑","餘","餛","餷","餿","饃","饈","饉","饊","饌","饢","庀"
            ,"廡","庋","庖","庥","庠","庹","庵","庾","庳","賡","廒","廑","廛","廨","廩","膺"
            ,"忄","忉","忖","懺","憮","忮","慪","忡","忤","愾","悵","愴","忪","忭","忸","怙"
            ,"怵","怦","怛","怏","怍","怩","怫","怊","懌","怡","慟","懨","惻","愷","恂"
            ,"恪","惲","悖","悚","慳","悝","悃","悒","悌","悛","愜","悻","悱","惝","惘"
            ,"惆","惚","悴","慍","憒","愕","愣","惴","愀","愎","愫","慊","慵","憬","憔","憧"
            ,"怵","懍","懵","忝","隳","閂","閆","闈","閎","閔","閌","闥","閭","閫","鬮","閬"
            ,"閾","閶","鬩","閿","閽","閼","闃","闋","闔","闐","闕","闞","爿","爿","戕","氵"
            ,"汔","汜","汊","灃","沅","沐","沔","沌","汨","汩","汴","汶","沆","溈","泐","泔"
            ,"沭","瀧","瀘","泱","泗","沲","泠","泖","濼","泫","泮","沱","泓","泯","涇"
            ,"洹","洧","洌","浹","湞","洇","洄","洙","洎","洫","澮","洮","洵","洚","瀏"
            ,"滸","潯","洳","涑","浯","淶","潿","浞","涓","涔","浜","浠","浼","浣","渚","淇"
            ,"淅","淞","瀆","涿","淠","澠","淦","淝","淙","瀋","涫","淥","涮","渫","湮","湎"
            ,"湫","溲","湟","漵","湓","湔","渲","渥","湄","灩","溱","溘","灄","漭","瀅","溥"
            ,"溧","溽","溻","溷","潷","溴","滏","溏","滂","溟","潢","瀠","瀟","濫","漕","滹"
            ,"漯","漶","瀲","瀦","漪","漉","漩","澉","澍","澌","潸","潲","潼","潺","瀨"
            ,"濉","澧","澹","澶","濂","濡","濮","濞","濠","濯","瀚","瀣","瀛","瀹","瀵"
            ,"灝","灞","宀","宄","宕","宓","宥","宸","甯","騫","搴","寤","寮","褰","寰","蹇"
            ,"謇","辶","迓","迕","迥","迮","迤","邇","迦","逕","迨","逅","逄","逋","邐","逑"
            ,"逍","逖","逡","逵","逶","逭","逯","遄","遑","遒","遐","遨","遘","遢","遛","暹"
            ,"遴","遽","邂","邈","邃","邋","彐","彗","彖","彘","尻","咫","屐","屙","孱","屣"
            ,"屨","羼","弳","弩","弭","艴","弼","鬻","屮","妁","妃","妍","嫵","嫗","妣"
            ,"妗","姊","媯","妞","妤","姒","妲","妯","姍","妾","婭","嬈","姝","孌","姣"
            ,"姘","姹","娌","娉","媧","嫻","娑","娣","娓","婀","婧","婊","婕","娼","婢","嬋"
            ,"胬","媼","媛","婷","婺","媾","嫫","媲","嬡","嬪","媸","嫠","嫣","嬙","嫖","嫦"
            ,"嫘","嫜","嬉","嬗","嬖","嬲","嬤","孀","尕","尜","孚","孥","孳","孑","孓","孢"
            ,"駔","駟","駙","騶","驛","駑","駘","驍","驊","駢","驪","騏","騍","騅","驂","騭"
            ,"騖","驁","騮","騸","驃","驄","驏","驥","驤","糸","紆","紂","紇","紈","纊"
            ,"紜","紕","紓","紺","絏","紱","縐","紼","絀","紿","絝","絎","絳","綆","綃"
            ,"綈","綾","綺","緋","緔","緄","綞","綬","綹","綣","綰","緇","緙","緗","緹","緲"
            ,"繢","緦","緶","緱","縋","緡","縉","縝","縟","縞","縭","縊","縑","繽","縹","縵"
            ,"縲","繆","繅","纈","繚","繒","韁","繾","繰","繯","纘","么","畿","巛","甾","邕"
            ,"玎","璣","瑋","玢","玟","玨","珂","瓏","玷","玳","珀","瑉","珈","珥","珙","頊"
            ,"琊","珩","珧","珞","璽","琿","璉","琪","瑛","琦","琥","琨","琰","琮","琬"
            ,"琛","琚","瑁","瑜","瑗","瑕","瑙","璦","瑭","瑾","璜","瓔","璀","璁","璿"
            ,"璋","璞","璨","璩","璐","璧","瓚","璺","韙","韞","韜","杌","杓","杞","杈","榪"
            ,"櫪","枇","杪","杳","枘","梘","杵","棖","樅","梟","枋","杷","杼","柰","櫛","柘"
            ,"櫳","柩","枰","櫨","柙","枵","柚","枳","柝","梔","柃","枸","柢","櫟","柁","檉"
            ,"栲","栳","椏","橈","桎","楨","桄","榿","梃","栝","桕","樺","桁","檜","桀","欒"
            ,"棬","桉","栩","梵","梏","桴","桷","梓","桫","欞","楮","棼","櫝","槧","棹"
            ,"欏","棰","椋","槨","楗","棣","椐","楱","椹","楠","楂","楝","欖","楫","榀"
            ,"榘","楸","椴","槌","櫬","櫚","槎","櫸","楦","楣","楹","榛","榧","榻","榫","榭"
            ,"槔","榱","槁","槊","檳","榕","櫧","榍","槿","檣","槭","樗","樘","櫫","槲","橄"
            ,"樾","檠","橐","橛","樵","檎","櫓","樽","樨","橘","櫞","檑","簷","檁","檗","檫"
            ,"猷","獒","歿","殂","殤","殄","殞","殮","殍","殫","殛","殯","殪","軔","軛","軲"
            ,"軻","轤","軹","軼","軫","軤","轢","軺","軾","輊","輇","輅","輒","輦","輞"
            ,"輟","輜","輳","轆","轔","軎","戔","戧","戛","戟","戢","戡","戥","戤","戩"
            ,"臧","甌","瓴","瓿","甏","甑","甓","攴","旮","旯","旰","昊","曇","杲","昃","昕"
            ,"昀","炅","曷","昝","昴","昱","昶","昵","耆","晟","曄","晁","晏","暉","晡","晗"
            ,"晷","暄","暌","曖","暝","暾","曛","曜","曦","曩","賁","貰","貺","貽","贄","貲"
            ,"賅","贐","賑","賚","賕","齎","賧","賻","覘","覬","覡","覿","覦","覯","覲","覷"
            ,"牮","強","牝","犛","牯","牾","牿","犄","犋","犍","犏","犒","挈","挲","掰"
            ,"搿","擘","耄","毪","毳","毽","毿","毹","氅","氌","氆","氍","氕","氘","氙"
            ,"氚","氡","氬","氤","氪","氳","攵","敕","敫","牘","牒","牖","爰","虢","刖","肟"
            ,"肜","肓","肼","朊","肽","肱","肫","肭","肴","膁","朧","腖","胩","臚","胛","胂"
            ,"胄","胙","胍","胗","朐","胝","脛","胱","胴","胭","膾","脎","胲","胼","朕","脒"
            ,"豚","腡","脞","脬","脘","脲","腈","醃","腓","腴","腙","腚","腱","腠","腩","靦"
            ,"膃","齶","腧","塍","媵","膈","膂","臏","滕","膣","膪","臌","朦","臊","膻"
            ,"臁","膦","歟","欷","欹","歃","歆","歙","颮","颯","颶","颼","飆","飆","殳"
            ,"彀","轂","觳","斐","齏","斕","於","旆","旄","旃","旌","旎","旒","旖","煬","煒"
            ,"燉","熗","炻","烀","炷","炫","炱","燁","烊","焐","焓","燜","焯","焱","糊","煜"
            ,"煨","煆","煲","煊","煸","退","溜","熳","熵","熨","熠","燠","燔","燧","燹","爝"
            ,"爨","灬","燾","煦","熹","戾","戽","扃","扈","扉","礻","祀","祆","祉","祛","祜"
            ,"祓","祚","禰","祗","祠","禎","祧","祺","禪","禊","禚","禧","禳","忑","忐"
            ,"懟","恝","恚","恧","恁","恙","恣","愨","愆","湣","慝","憩","憝","懋","懣"
            ,"戇","肀","聿","遝","澩","淼","磯","矸","碭","砉","硨","砘","砑","斫","砭","碸"
            ,"砝","砹","礪","礱","砟","砼","砥","砬","砣","砩","硎","硭","硤","磽","砦","硐"
            ,"硇","硌","硪","磧","碓","碚","碇","磣","碡","碣","碲","镟","碥","磔","滾","磉"
            ,"磬","磲","礅","磴","礓","礤","礞","礴","龕","黹","黻","黼","盱","眄","瞘","盹"
            ,"眇","眈","眚","眢","眙","眭","眥","眵","眸","睞","瞼","睇","睃","睚","睨"
            ,"睢","睥","睿","瞍","睽","瞀","瞌","瞑","瞟","瞠","瞰","瞵","瞽","町","畀"
            ,"畎","畋","畈","畛","佘","畹","疃","罘","罡","罟","詈","罨","羆","罱","罹","羈"
            ,"罾","盍","盥","蠲","钅","釓","釔","釙","釗","釕","釷","釧","釤","鍆","釵","釹"
            ,"鈈","鈦","钜","鈑","鈐","鈁","鈧","鈄","鈥","鈀","鈺","鉦","鈷","鈳","鉕","鈽"
            ,"鈸","鉞","鉬","鉭","鈿","鑠","鈰","鉉","鉈","鉍","鈮","鈹","鐸","銬","銠","鉺"
            ,"銪","鋮","鋏","鐃","鋣","鐺","銱","銦","鎧","銖","鋌","銩","鏵","銓","鉿"
            ,"鎩","銚","錚","銫","銃","鐋","銨","銣","鐒","錸","鋱","鏗","鋥","鋰","鋯"
            ,"鋨","銼","鋝","鋶","鉲","鐧","鋃","鋟","鋦","錒","錆","鍩","錛","鍀","錁","錕"
            ,"錮","鍃","錇","錈","錟","錙","鍥","鍇","鍶","鍔","鍤","鎪","鍰","鎄","鏤","鏘"
            ,"鐨","鋂","鏌","鎘","鐫","錼","鎦","鎰","鎵","鑌","鏢","鏜","鏝","鏍","鏞","鏃"
            ,"鏇","鏑","鐔","钁","鏷","鑥","鐓","鑭","鐠","鑹","鏹","鐙","鑊","鐲","鐿","鑔"
            ,"鑣","鍾","矧","矬","雉","秕","秭","秣","秫","穭","嵇","稃","稂","稞","稔"
            ,"稹","稷","穡","黏","馥","穰","皈","皎","皓","皙","皤","瓞","瓠","甬","鳩"
            ,"鳶","鴇","鴆","鴣","鶇","鸕","鴝","鴟","鷥","鴯","鷙","鴰","鵂","鸞","鵓","鸝"
            ,"鵠","鵒","鷳","鵜","鵡","鶓","鵪","鵯","鶉","鶘","鶚","鶿","鶥","鶩","鷂","鶼"
            ,"鸚","鷓","鷚","鷯","鷦","鷲","鷸","鸌","鷺","鸛","疒","疔","癤","癘","疝","鬁"
            ,"疣","疳","屙","疸","痄","皰","疰","痃","痂","瘂","痍","痣","癆","痦","痤","癇"
            ,"痧","瘃","痱","痼","痿","瘐","瘀","癉","瘌","瘞","瘊","瘥","瘺","瘕","瘙"
            ,"瘛","瘼","瘢","瘠","癀","瘭","瘰","癭","瘵","癃","癮","瘳","癍","癩","癔"
            ,"癜","癖","癲","臒","翊","竦","穸","穹","窀","窆","窈","窕","竇","窠","窬","窨"
            ,"窶","窳","衤","衩","衲","衽","衿","袂","袢","襠","袷","袼","裉","褳","裎","襝"
            ,"襇","裱","褚","裼","裨","裾","裰","褡","褙","褓","褸","褊","襤","褫","褶","繈"
            ,"襦","襻","疋","胥","皸","皴","矜","耒","耔","耖","耜","耠","耮","耥","耦","耬"
            ,"耩","耨","耱","耋","耵","聃","聆","聹","聒","聵","聱","覃","頇","頎","頏"
            ,"頡","頜","潁","頦","頷","顎","顓","顳","顢","顙","顥","顬","顰","虍","虔"
            ,"虯","蟣","蠆","虺","虼","虻","蚨","蚍","蚋","蜆","蠔","蚧","蚣","蚪","蚓","蚩"
            ,"蚶","蛄","蚵","蠣","蚰","蚺","蚱","蚯","蛉","蟶","蚴","蛩","蛺","蟯","蛭","螄"
            ,"蛐","蜓","蛞","蠐","蛟","蛘","蛑","蜃","蜇","蛸","蜈","蜊","蜍","蜉","蜣","蜻"
            ,"蜞","蜥","蜮","蜚","蜾","蟈","蜴","蜱","蜩","蜷","蜿","螂","蜢","蝽","蠑","蝻"
            ,"蝠","蝰","蝌","蝮","螋","蝓","蝣","螻","蝤","蝙","蝥","螓","螯","蟎","蟒"
            ,"蟆","螈","螅","螭","螗","螃","螫","蟥","螬","螵","螳","蟋","蟓","螽","蟑"
            ,"蟀","蟊","蟛","蟪","蟠","蟮","蠖","蠓","蟾","蠊","蠛","蠡","蠹","蠼","缶","罌"
            ,"罄","罅","舐","竺","竽","笈","篤","笄","筧","笊","笫","笏","筇","笸","笪","笙"
            ,"笮","笱","笠","笥","笤","笳","籩","笞","筘","篳","筅","筵","筌","箏","筠","筮"
            ,"筻","筢","筲","筱","箐","簀","篋","箸","箬","箝","籜","箅","簞","箜","箢","簫"
            ,"箴","簣","篁","篌","篝","篚","篥","篦","篪","簌","篾","篼","簏","籪","簋"
            ,"簟","簪","簦","簸","籟","籀","臾","舁","舂","舄","臬","衄","舡","舢","艤"
            ,"舭","舯","舨","舫","舸","艫","舳","舴","舾","艄","艉","艋","艏","艚","艟","艨"
            ,"衾","嫋","袈","裘","裟","襞","羝","羥","羧","羯","羰","羲","秈","敉","粑","糲"
            ,"糶","粞","粢","粲","粼","粽","糝","餱","糌","糍","糈","糅","糗","糨","艮","暨"
            ,"羿","翎","翕","翥","翡","翦","翩","翮","翳","糸","縶","綦","綮","繇","纛","麩"
            ,"麴","赳","趄","趔","趑","趲","赧","赭","豇","豉","酊","酐","酎","酏","酤"
            ,"酢","酡","醯","酩","酯","釅","釃","酲","酴","酹","醌","醅","醐","醍","醑"
            ,"醢","醣","醪","醭","醮","醯","醵","醴","醺","豕","鹺","躉","跫","踅","蹙","蹩"
            ,"趵","趿","趼","趺","蹌","蹠","跗","跚","躒","跎","跏","跛","跆","跬","蹺","蹕"
            ,"跣","躚","躋","跤","踉","跽","踔","踝","踟","躓","踮","踣","躑","踺","蹀","踹"
            ,"踵","踽","踱","蹉","蹁","蹂","躡","蹣","蹊","躕","蹶","蹼","蹯","蹴","躅","躪"
            ,"躔","躐","躦","躞","豸","貂","貊","貅","貘","貔","斛","觖","觴","觚","觜"
            ,"觥","觫","觶","訾","謦","靚","雩","靂","雯","霆","霽","霈","霏","霎","霪"
            ,"靄","霰","霾","齔","齟","齙","齠","齜","齦","齬","齪","齷","黽","黿","鼉","隹"
            ,"隼","雋","雎","雒","瞿","讎","銎","鑾","鋈","鏨","鍪","鏊","鎏","鐾","鑫","魷"
            ,"魴","鮁","鮃","鯰","鱸","穌","鮒","鱟","鮐","鮭","鮚","鮪","鮞","鱭","鮫","鯗"
            ,"鱘","鯁","鱺","鰱","鰹","鰣","鰷","鯀","鯊","鯇","鯽","鯖","鯪","鯫","鯡","鯤"
            ,"鯧","鯝","鯢","鯰","鯛","鯴","鯔","鱝","鰈","鱷","鰍","鰒","鰉","鯿","鰠"
            ,"鼇","鰭","鰨","鰥","鰩","鰳","鰾","鱈","鰻","鰵","鱅","鱖","鱔","鱒","鱧"
            ,"靼","鞅","韃","鞽","鞔","韉","鞫","鞣","鞲","韝","骱","骰","骷","鶻","骶","骺"
            ,"骼","髁","髀","髏","髂","髖","髕","髑","魅","魃","魘","魎","魈","魍","魑","饗"
            ,"饜","餮","饕","饔","髟","髡","髦","髯","髫","髻","髭","髹","鬈","鬏","鬢","鬟"
            ,"鬣","麽","麾","縻","麂","麇","麈","麋","麒","鏖","麝","麟","黛","黜","黝","黠"
            ,"黟","黢","黷","黧","黥","黲","黯","鼢","鼬","鼯","鼴","鼷","鼽","鼾","齇"
        };

        /// <summary>
        /// 二級漢字對應拼音陣列
        /// </summary>
        private static string[] otherPinYin = new string[]
           {
               "Chu","Ji","Wu","Gai","Nian","Sa","Pi","Gen","Cheng","Ge","Nao","E","Shu","Yu","Pie","Bi",
                "Tuo","Yao","Yao","Zhi","Di","Xin","Yin","Kui","Yu","Gao","Tao","Dian","Ji","Nai","Nie","Ji",
                "Qi","Mi","Bei","Se","Gu","Ze","She","Cuo","Yan","Jue","Si","Ye","Yan","Fang","Po","Gui",
                "Kui","Bian","Ze","Gua","You","Ce","Yi","Wen","Jing","Ku","Gui","Kai","La","Ji","Yan","Wan",
                "Kuai","Piao","Jue","Qiao","Huo","Yi","Tong","Wang","Dan","Ding","Zhang","Le","Sa","Yi","Mu","Ren",
                "Yu","Pi","Ya","Wa","Wu","Chang","Cang","Kang","Zhu","Ning","Ka","You","Yi","Gou","Tong","Tuo",
                "Ni","Ga","Ji","Er","You","Kua","Kan","Zhu","Yi","Tiao","Chai","Jiao","Nong","Mou","Chou","Yan",
                "Li","Qiu","Li","Yu","Ping","Yong","Si","Feng","Qian","Ruo","Pai","Zhuo","Shu","Luo","Wo","Bi",
                "Ti","Guan","Kong","Ju","Fen","Yan","Xie","Ji","Wei","Zong","Lou","Tang","Bin","Nuo","Chi","Xi",
                "Jing","Jian","Jiao","Jiu","Tong","Xuan","Dan","Tong","Tun","She","Qian","Zu","Yue","Cuan","Di","Xi",
                "Xun","Hong","Guo","Chan","Kui","Bao","Pu","Hong","Fu","Fu","Su","Si","Wen","Yan","Bo","Gun",
                "Mao","Xie","Luan","Pou","Bing","Ying","Luo","Lei","Liang","Hu","Lie","Xian","Song","Ping","Zhong","Ming",
                "Yan","Jie","Hong","Shan","Ou","Ju","Ne","Gu","He","Di","Zhao","Qu","Dai","Kuang","Lei","Gua",
                "Jie","Hui","Shen","Gou","Quan","Zheng","Hun","Xu","Qiao","Gao","Kuang","Ei","Zou","Zhuo","Wei","Yu",
                "Shen","Chan","Sui","Chen","Jian","Xue","Ye","E","Yu","Xuan","An","Di","Zi","Pian","Mo","Dang",
                "Su","Shi","Mi","Zhe","Jian","Zen","Qiao","Jue","Yan","Zhan","Chen","Dan","Jin","Zuo","Wu","Qian",
                "Jing","Ban","Yan","Zuo","Bei","Jing","Gai","Zhi","Nie","Zou","Chui","Pi","Wei","Huang","Wei","Xi",
                "Han","Qiong","Kuang","Mang","Wu","Fang","Bing","Pi","Bei","Ye","Di","Tai","Jia","Zhi","Zhu","Kuai",
                "Qie","Xun","Yun","Li","Ying","Gao","Xi","Fu","Pi","Tan","Yan","Juan","Yan","Yin","Zhang","Po",
                "Shan","Zou","Ling","Feng","Chu","Huan","Mai","Qu","Shao","He","Ge","Meng","Xu","Xie","Sou","Xie",
                "Jue","Jian","Qian","Dang","Chang","Si","Bian","Ben","Qiu","Ben","E","Fa","Shu","Ji","Yong","He",
                "Wei","Wu","Ge","Zhen","Kuang","Pi","Yi","Li","Qi","Ban","Gan","Long","Dian","Lu","Che","Di",
                "Tuo","Ni","Mu","Ao","Ya","Die","Dong","Kai","Shan","Shang","Nao","Gai","Yin","Cheng","Shi","Guo",
                "Xun","Lie","Yuan","Zhi","An","Yi","Pi","Nian","Peng","Tu","Sao","Dai","Ku","Die","Yin","Leng",
                "Hou","Ge","Yuan","Man","Yong","Liang","Chi","Xin","Pi","Yi","Cao","Jiao","Nai","Du","Qian","Ji",
                "Wan","Xiong","Qi","Xiang","Fu","Yuan","Yun","Fei","Ji","Li","E","Ju","Pi","Zhi","Rui","Xian",
                "Chang","Cong","Qin","Wu","Qian","Qi","Shan","Bian","Zhu","Kou","Yi","Mo","Gan","Pie","Long","Ba",
                "Mu","Ju","Ran","Qing","Chi","Fu","Ling","Niao","Yin","Mao","Ying","Qiong","Min","Tiao","Qian","Yi",
                "Rao","Bi","Zi","Ju","Tong","Hui","Zhu","Ting","Qiao","Fu","Ren","Xing","Quan","Hui","Xun","Ming",
                "Qi","Jiao","Chong","Jiang","Luo","Ying","Qian","Gen","Jin","Mai","Sun","Hong","Zhou","Kan","Bi","Shi",
                "Wo","You","E","Mei","You","Li","Tu","Xian","Fu","Sui","You","Di","Shen","Guan","Lang","Ying",
                "Chun","Jing","Qi","Xi","Song","Jin","Nai","Qi","Ba","Shu","Chang","Tie","Yu","Huan","Bi","Fu",
                "Tu","Dan","Cui","Yan","Zu","Dang","Jian","Wan","Ying","Gu","Han","Qia","Feng","Shen","Xiang","Wei",
                "Chan","Kai","Qi","Kui","Xi","E","Bao","Pa","Ting","Lou","Pai","Xuan","Jia","Zhen","Shi","Ru",
                "Mo","En","Bei","Weng","Hao","Ji","Li","Bang","Jian","Shuo","Lang","Ying","Yu","Su","Meng","Dou",
                "Xi","Lian","Cu","Lin","Qu","Kou","Xu","Liao","Hui","Xun","Jue","Rui","Zui","Ji","Meng","Fan",
                "Qi","Hong","Xie","Hong","Wei","Yi","Weng","Sou","Bi","Hao","Tai","Ru","Xun","Xian","Gao","Li",
                "Huo","Qu","Heng","Fan","Nie","Mi","Gong","Yi","Kuang","Lian","Da","Yi","Xi","Zang","Pao","You",
                "Liao","Ga","Gan","Ti","Men","Tuan","Chen","Fu","Pin","Niu","Jie","Jiao","Za","Yi","Lv","Jun",
                "Tian","Ye","Ai","Na","Ji","Guo","Bai","Ju","Pou","Lie","Qian","Guan","Die","Zha","Ya","Qin",
                "Yu","An","Xuan","Bing","Kui","Yuan","Shu","En","Chuai","Jian","Shuo","Zhan","Nuo","Sang","Luo","Ying",
                "Zhi","Han","Zhe","Xie","Lu","Zun","Cuan","Gan","Huan","Pi","Xing","Zhuo","Huo","Zuan","Nang","Yi",
                "Te","Dai","Shi","Bu","Chi","Ji","Kou","Dao","Le","Zha","A","Yao","Fu","Mu","Yi","Tai",
                "Li","E","Bi","Bei","Guo","Qin","Yin","Za","Ka","Ga","Gua","Ling","Dong","Ning","Duo","Nao",
                "You","Si","Kuang","Ji","Shen","Hui","Da","Lie","Yi","Xiao","Bi","Ci","Guang","Yue","Xiu","Yi",
                "Pai","Kuai","Duo","Ji","Mie","Mi","Zha","Nong","Gen","Mou","Mai","Chi","Lao","Geng","En","Zha",
                "Suo","Zao","Xi","Zuo","Ji","Feng","Ze","Nuo","Miao","Lin","Zhuan","Zhou","Tao","Hu","Cui","Sha",
                "Yo","Dan","Bo","Ding","Lang","Li","Shua","Chuo","Die","Da","Nan","Li","Kui","Jie","Yong","Kui",
                "Jiu","Sou","Yin","Chi","Jie","Lou","Ku","Wo","Hui","Qin","Ao","Su","Du","Ke","Nie","He",
                "Chen","Suo","Ge","A","En","Hao","Dia","Ai","Ai","Suo","Hei","Tong","Chi","Pei","Lei","Cao",
                "Piao","Qi","Ying","Beng","Sou","Di","Mi","Peng","Jue","Liao","Pu","Chuai","Jiao","O","Qin","Lu",
                "Ceng","Deng","Hao","Jin","Jue","Yi","Sai","Pi","Ru","Cha","Huo","Nang","Wei","Jian","Nan","Lun",
                "Hu","Ling","You","Yu","Qing","Yu","Huan","Wei","Zhi","Pei","Tang","Dao","Ze","Guo","Wei","Wo",
                "Man","Zhang","Fu","Fan","Ji","Qi","Qian","Qi","Qu","Ya","Xian","Ao","Cen","Lan","Ba","Hu",
                "Ke","Dong","Jia","Xiu","Dai","Gou","Mao","Min","Yi","Dong","Qiao","Xun","Zheng","Lao","Lai","Song",
                "Yan","Gu","Xiao","Guo","Kong","Jue","Rong","Yao","Wai","Zai","Wei","Yu","Cuo","Lou","Zi","Mei",
                "Sheng","Song","Ji","Zhang","Lin","Deng","Bin","Yi","Dian","Chi","Pang","Cu","Xun","Yang","Hou","Lai",
                "Xi","Chang","Huang","Yao","Zheng","Jiao","Qu","San","Fan","Qiu","An","Guang","Ma","Niu","Yun","Xia",
                "Pao","Fei","Rong","Kuai","Shou","Sun","Bi","Juan","Li","Yu","Xian","Yin","Suan","Yi","Guo","Luo",
                "Ni","She","Cu","Mi","Hu","Cha","Wei","Wei","Mei","Nao","Zhang","Jing","Jue","Liao","Xie","Xun",
                "Huan","Chuan","Huo","Sun","Yin","Dong","Shi","Tang","Tun","Xi","Ren","Yu","Chi","Yi","Xiang","Bo",
                "Yu","Hun","Zha","Sou","Mo","Xiu","Jin","San","Zhuan","Nang","Pi","Wu","Gui","Pao","Xiu","Xiang",
                "Tuo","An","Yu","Bi","Geng","Ao","Jin","Chan","Xie","Lin","Ying","Shu","Dao","Cun","Chan","Wu",
                "Zhi","Ou","Chong","Wu","Kai","Chang","Chuang","Song","Bian","Niu","Hu","Chu","Peng","Da","Yang","Zuo",
                "Ni","Fu","Chao","Yi","Yi","Tong","Yan","Ce","Kai","Xun","Ke","Yun","Bei","Song","Qian","Kui",
                "Kun","Yi","Ti","Quan","Qie","Xing","Fei","Chang","Wang","Chou","Hu","Cui","Yun","Kui","E","Leng",
                "Zhui","Qiao","Bi","Su","Qie","Yong","Jing","Qiao","Chong","Chu","Lin","Meng","Tian","Hui","Shuan","Yan",
                "Wei","Hong","Min","Kang","Ta","Lv","Kun","Jiu","Lang","Yu","Chang","Xi","Wen","Hun","E","Qu",
                "Que","He","Tian","Que","Kan","Jiang","Pan","Qiang","San","Qi","Si","Cha","Feng","Yuan","Mu","Mian",
                "Dun","Mi","Gu","Bian","Wen","Hang","Wei","Le","Gan","Shu","Long","Lu","Yang","Si","Duo","Ling",
                "Mao","Luo","Xuan","Pan","Duo","Hong","Min","Jing","Huan","Wei","Lie","Jia","Zhen","Yin","Hui","Zhu",
                "Ji","Xu","Hui","Tao","Xun","Jiang","Liu","Hu","Xun","Ru","Su","Wu","Lai","Wei","Zhuo","Juan",
                "Cen","Bang","Xi","Mei","Huan","Zhu","Qi","Xi","Song","Du","Zhuo","Pei","Mian","Gan","Fei","Cong",
                "Shen","Guan","Lu","Shuan","Xie","Yan","Mian","Qiu","Sou","Huang","Xu","Pen","Jian","Xuan","Wo","Mei",
                "Yan","Qin","Ke","She","Mang","Ying","Pu","Li","Ru","Ta","Hun","Bi","Xiu","Fu","Tang","Pang",
                "Ming","Huang","Ying","Xiao","Lan","Cao","Hu","Luo","Huan","Lian","Zhu","Yi","Lu","Xuan","Gan","Shu",
                "Si","Shan","Shao","Tong","Chan","Lai","Sui","Li","Dan","Chan","Lian","Ru","Pu","Bi","Hao","Zhuo",
                "Han","Xie","Ying","Yue","Fen","Hao","Ba","Bao","Gui","Dang","Mi","You","Chen","Ning","Jian","Qian",
                "Wu","Liao","Qian","Huan","Jian","Jian","Zou","Ya","Wu","Jiong","Ze","Yi","Er","Jia","Jing","Dai",
                "Hou","Pang","Bu","Li","Qiu","Xiao","Ti","Qun","Kui","Wei","Huan","Lu","Chuan","Huang","Qiu","Xia",
                "Ao","Gou","Ta","Liu","Xian","Lin","Ju","Xie","Miao","Sui","La","Ji","Hui","Tuan","Zhi","Kao",
                "Zhi","Ji","E","Chan","Xi","Ju","Chan","Jing","Nu","Mi","Fu","Bi","Yu","Che","Shuo","Fei",
                "Yan","Wu","Yu","Bi","Jin","Zi","Gui","Niu","Yu","Si","Da","Zhou","Shan","Qie","Ya","Rao",
                "Shu","Luan","Jiao","Pin","Cha","Li","Ping","Wa","Xian","Suo","Di","Wei","E","Jing","Biao","Jie",
                "Chang","Bi","Chan","Nu","Ao","Yuan","Ting","Wu","Gou","Mo","Pi","Ai","Pin","Chi","Li","Yan",
                "Qiang","Piao","Chang","Lei","Zhang","Xi","Shan","Bi","Niao","Mo","Shuang","Ga","Ga","Fu","Nu","Zi",
                "Jie","Jue","Bao","Zang","Si","Fu","Zou","Yi","Nu","Dai","Xiao","Hua","Pian","Li","Qi","Ke",
                "Zhui","Can","Zhi","Wu","Ao","Liu","Shan","Biao","Cong","Chan","Ji","Xiang","Jiao","Yu","Zhou","Ge",
                "Wan","Kuang","Yun","Pi","Shu","Gan","Xie","Fu","Zhou","Fu","Chu","Dai","Ku","Hang","Jiang","Geng",
                "Xiao","Ti","Ling","Qi","Fei","Shang","Gun","Duo","Shou","Liu","Quan","Wan","Zi","Ke","Xiang","Ti",
                "Miao","Hui","Si","Bian","Gou","Zhui","Min","Jin","Zhen","Ru","Gao","Li","Yi","Jian","Bin","Piao",
                "Man","Lei","Miao","Sao","Xie","Liao","Zeng","Jiang","Qian","Qiao","Huan","Zuan","Yao","Ji","Chuan","Zai",
                "Yong","Ding","Ji","Wei","Bin","Min","Jue","Ke","Long","Dian","Dai","Po","Min","Jia","Er","Gong",
                "Xu","Ya","Heng","Yao","Luo","Xi","Hui","Lian","Qi","Ying","Qi","Hu","Kun","Yan","Cong","Wan",
                "Chen","Ju","Mao","Yu","Yuan","Xia","Nao","Ai","Tang","Jin","Huang","Ying","Cui","Cong","Xuan","Zhang",
                "Pu","Can","Qu","Lu","Bi","Zan","Wen","Wei","Yun","Tao","Wu","Shao","Qi","Cha","Ma","Li",
                "Pi","Miao","Yao","Rui","Jian","Chu","Cheng","Cong","Xiao","Fang","Pa","Zhu","Nai","Zhi","Zhe","Long",
                "Jiu","Ping","Lu","Xia","Xiao","You","Zhi","Tuo","Zhi","Ling","Gou","Di","Li","Tuo","Cheng","Kao",
                "Lao","Ya","Rao","Zhi","Zhen","Guang","Qi","Ting","Gua","Jiu","Hua","Heng","Gui","Jie","Luan","Juan",
                "An","Xu","Fan","Gu","Fu","Jue","Zi","Suo","Ling","Chu","Fen","Du","Qian","Zhao","Luo","Chui",
                "Liang","Guo","Jian","Di","Ju","Cou","Zhen","Nan","Zha","Lian","Lan","Ji","Pin","Ju","Qiu","Duan",
                "Chui","Chen","Lv","Cha","Ju","Xuan","Mei","Ying","Zhen","Fei","Ta","Sun","Xie","Gao","Cui","Gao",
                "Shuo","Bin","Rong","Zhu","Xie","Jin","Qiang","Qi","Chu","Tang","Zhu","Hu","Gan","Yue","Qing","Tuo",
                "Jue","Qiao","Qin","Lu","Zun","Xi","Ju","Yuan","Lei","Yan","Lin","Bo","Cha","You","Ao","Mo",
                "Cu","Shang","Tian","Yun","Lian","Piao","Dan","Ji","Bin","Yi","Ren","E","Gu","Ke","Lu","Zhi",
                "Yi","Zhen","Hu","Li","Yao","Shi","Zhi","Quan","Lu","Zhe","Nian","Wang","Chuo","Zi","Cou","Lu",
                "Lin","Wei","Jian","Qiang","Jia","Ji","Ji","Kan","Deng","Gai","Jian","Zang","Ou","Ling","Bu","Beng",
                "Zeng","Pi","Po","Ga","La","Gan","Hao","Tan","Gao","Ze","Xin","Yun","Gui","He","Zan","Mao",
                "Yu","Chang","Ni","Qi","Sheng","Ye","Chao","Yan","Hui","Bu","Han","Gui","Xuan","Kui","Ai","Ming",
                "Tun","Xun","Yao","Xi","Nang","Ben","Shi","Kuang","Yi","Zhi","Zi","Gai","Jin","Zhen","Lai","Qiu",
                "Ji","Dan","Fu","Chan","Ji","Xi","Di","Yu","Gou","Jin","Qu","Jian","Jiang","Pin","Mao","Gu",
                "Wu","Gu","Ji","Ju","Jian","Pian","Kao","Qie","Suo","Bai","Ge","Bo","Mao","Mu","Cui","Jian",
                "San","Shu","Chang","Lu","Pu","Qu","Pie","Dao","Xian","Chuan","Dong","Ya","Yin","Ke","Yun","Fan",
                "Chi","Jiao","Du","Die","You","Yuan","Guo","Yue","Wo","Rong","Huang","Jing","Ruan","Tai","Gong","Zhun",
                "Na","Yao","Qian","Long","Dong","Ka","Lu","Jia","Shen","Zhou","Zuo","Gua","Zhen","Qu","Zhi","Jing",
                "Guang","Dong","Yan","Kuai","Sa","Hai","Pian","Zhen","Mi","Tun","Luo","Cuo","Pao","Wan","Niao","Jing",
                "Yan","Fei","Yu","Zong","Ding","Jian","Cou","Nan","Mian","Wa","E","Shu","Cheng","Ying","Ge","Lv",
                "Bin","Teng","Zhi","Chuai","Gu","Meng","Sao","Shan","Lian","Lin","Yu","Xi","Qi","Sha","Xin","Xi",
                "Biao","Sa","Ju","Sou","Biao","Biao","Shu","Gou","Gu","Hu","Fei","Ji","Lan","Yu","Pei","Mao",
                "Zhan","Jing","Ni","Liu","Yi","Yang","Wei","Dun","Qiang","Shi","Hu","Zhu","Xuan","Tai","Ye","Yang",
                "Wu","Han","Men","Chao","Yan","Hu","Yu","Wei","Duan","Bao","Xuan","Bian","Tui","Liu","Man","Shang",
                "Yun","Yi","Yu","Fan","Sui","Xian","Jue","Cuan","Huo","Tao","Xu","Xi","Li","Hu","Jiong","Hu",
                "Fei","Shi","Si","Xian","Zhi","Qu","Hu","Fu","Zuo","Mi","Zhi","Ci","Zhen","Tiao","Qi","Chan",
                "Xi","Zhuo","Xi","Rang","Te","Tan","Dui","Jia","Hui","Nv","Nin","Yang","Zi","Que","Qian","Min",
                "Te","Qi","Dui","Mao","Men","Gang","Yu","Yu","Ta","Xue","Miao","Ji","Gan","Dang","Hua","Che",
                "Dun","Ya","Zhuo","Bian","Feng","Fa","Ai","Li","Long","Zha","Tong","Di","La","Tuo","Fu","Xing",
                "Mang","Xia","Qiao","Zhai","Dong","Nao","Ge","Wo","Qi","Dui","Bei","Ding","Chen","Zhou","Jie","Di",
                "Xuan","Bian","Zhe","Gun","Sang","Qing","Qu","Dun","Deng","Jiang","Ca","Meng","Bo","Kan","Zhi","Fu",
                "Fu","Xu","Mian","Kou","Dun","Miao","Dan","Sheng","Yuan","Yi","Sui","Zi","Chi","Mou","Lai","Jian",
                "Di","Suo","Ya","Ni","Sui","Pi","Rui","Sou","Kui","Mao","Ke","Ming","Piao","Cheng","Kan","Lin",
                "Gu","Ding","Bi","Quan","Tian","Fan","Zhen","She","Wan","Tuan","Fu","Gang","Gu","Li","Yan","Pi",
                "Lan","Li","Ji","Zeng","He","Guan","Juan","Jin","Ga","Yi","Po","Zhao","Liao","Tu","Chuan","Shan",
                "Men","Chai","Nv","Bu","Tai","Ju","Ban","Qian","Fang","Kang","Dou","Huo","Ba","Yu","Zheng","Gu",
                "Ke","Po","Bu","Bo","Yue","Mu","Tan","Dian","Shuo","Shi","Xuan","Ta","Bi","Ni","Pi","Duo",
                "Kao","Lao","Er","You","Cheng","Jia","Nao","Ye","Cheng","Diao","Yin","Kai","Zhu","Ding","Diu","Hua",
                "Quan","Ha","Sha","Diao","Zheng","Se","Chong","Tang","An","Ru","Lao","Lai","Te","Keng","Zeng","Li",
                "Gao","E","Cuo","Lve","Liu","Kai","Jian","Lang","Qin","Ju","A","Qiang","Nuo","Ben","De","Ke",
                "Kun","Gu","Huo","Pei","Juan","Tan","Zi","Qie","Kai","Si","E","Cha","Sou","Huan","Ai","Lou",
                "Qiang","Fei","Mei","Mo","Ge","Juan","Na","Liu","Yi","Jia","Bin","Biao","Tang","Man","Luo","Yong",
                "Chuo","Xuan","Di","Tan","Jue","Pu","Lu","Dui","Lan","Pu","Cuan","Qiang","Deng","Huo","Zhuo","Yi",
                "Cha","Biao","Zhong","Shen","Cuo","Zhi","Bi","Zi","Mo","Shu","Lv","Ji","Fu","Lang","Ke","Ren",
                "Zhen","Ji","Se","Nian","Fu","Rang","Gui","Jiao","Hao","Xi","Po","Die","Hu","Yong","Jiu","Yuan",
                "Bao","Zhen","Gu","Dong","Lu","Qu","Chi","Si","Er","Zhi","Gua","Xiu","Luan","Bo","Li","Hu",
                "Yu","Xian","Ti","Wu","Miao","An","Bei","Chun","Hu","E","Ci","Mei","Wu","Yao","Jian","Ying",
                "Zhe","Liu","Liao","Jiao","Jiu","Yu","Hu","Lu","Guan","Bing","Ding","Jie","Li","Shan","Li","You",
                "Gan","Ke","Da","Zha","Pao","Zhu","Xuan","Jia","Ya","Yi","Zhi","Lao","Wu","Cuo","Xian","Sha",
                "Zhu","Fei","Gu","Wei","Yu","Yu","Dan","La","Yi","Hou","Chai","Lou","Jia","Sao","Chi","Mo",
                "Ban","Ji","Huang","Biao","Luo","Ying","Zhai","Long","Yin","Chou","Ban","Lai","Yi","Dian","Pi","Dian",
                "Qu","Yi","Song","Xi","Qiong","Zhun","Bian","Yao","Tiao","Dou","Ke","Yu","Xun","Ju","Yu","Yi",
                "Cha","Na","Ren","Jin","Mei","Pan","Dang","Jia","Ge","Ken","Lian","Cheng","Lian","Jian","Biao","Chu",
                "Ti","Bi","Ju","Duo","Da","Bei","Bao","Lv","Bian","Lan","Chi","Zhe","Qiang","Ru","Pan","Ya",
                "Xu","Jun","Cun","Jin","Lei","Zi","Chao","Si","Huo","Lao","Tang","Ou","Lou","Jiang","Nou","Mo",
                "Die","Ding","Dan","Ling","Ning","Guo","Kui","Ao","Qin","Han","Qi","Hang","Jie","He","Ying","Ke",
                "Han","E","Zhuan","Nie","Man","Sang","Hao","Ru","Pin","Hu","Qian","Qiu","Ji","Chai","Hui","Ge",
                "Meng","Fu","Pi","Rui","Xian","Hao","Jie","Gong","Dou","Yin","Chi","Han","Gu","Ke","Li","You",
                "Ran","Zha","Qiu","Ling","Cheng","You","Qiong","Jia","Nao","Zhi","Si","Qu","Ting","Kuo","Qi","Jiao",
                "Yang","Mou","Shen","Zhe","Shao","Wu","Li","Chu","Fu","Qiang","Qing","Qi","Xi","Yu","Fei","Guo",
                "Guo","Yi","Pi","Tiao","Quan","Wan","Lang","Meng","Chun","Rong","Nan","Fu","Kui","Ke","Fu","Sou",
                "Yu","You","Lou","You","Bian","Mou","Qin","Ao","Man","Mang","Ma","Yuan","Xi","Chi","Tang","Pang",
                "Shi","Huang","Cao","Piao","Tang","Xi","Xiang","Zhong","Zhang","Shuai","Mao","Peng","Hui","Pan","Shan","Huo",
                "Meng","Chan","Lian","Mie","Li","Du","Qu","Fou","Ying","Qing","Xia","Shi","Zhu","Yu","Ji","Du",
                "Ji","Jian","Zhao","Zi","Hu","Qiong","Po","Da","Sheng","Ze","Gou","Li","Si","Tiao","Jia","Bian",
                "Chi","Kou","Bi","Xian","Yan","Quan","Zheng","Jun","Shi","Gang","Pa","Shao","Xiao","Qing","Ze","Qie",
                "Zhu","Ruo","Qian","Tuo","Bi","Dan","Kong","Wan","Xiao","Zhen","Kui","Huang","Hou","Gou","Fei","Li",
                "Bi","Chi","Su","Mie","Dou","Lu","Duan","Gui","Dian","Zan","Deng","Bo","Lai","Zhou","Yu","Yu",
                "Chong","Xi","Nie","Nv","Chuan","Shan","Yi","Bi","Zhong","Ban","Fang","Ge","Lu","Zhu","Ze","Xi",
                "Shao","Wei","Meng","Shou","Cao","Chong","Meng","Qin","Niao","Jia","Qiu","Sha","Bi","Di","Qiang","Suo",
                "Jie","Tang","Xi","Xian","Mi","Ba","Li","Tiao","Xi","Zi","Can","Lin","Zong","San","Hou","Zan",
                "Ci","Xu","Rou","Qiu","Jiang","Gen","Ji","Yi","Ling","Xi","Zhu","Fei","Jian","Pian","He","Yi",
                "Jiao","Zhi","Qi","Qi","Yao","Dao","Fu","Qu","Jiu","Ju","Lie","Zi","Zan","Nan","Zhe","Jiang",
                "Chi","Ding","Gan","Zhou","Yi","Gu","Zuo","Tuo","Xian","Ming","Zhi","Yan","Shai","Cheng","Tu","Lei",
                "Kun","Pei","Hu","Ti","Xu","Hai","Tang","Lao","Bu","Jiao","Xi","Ju","Li","Xun","Shi","Cuo",
                "Dun","Qiong","Xue","Cu","Bie","Bo","Ta","Jian","Fu","Qiang","Zhi","Fu","Shan","Li","Tuo","Jia",
                "Bo","Tai","Kui","Qiao","Bi","Xian","Xian","Ji","Jiao","Liang","Ji","Chuo","Huai","Chi","Zhi","Dian",
                "Bo","Zhi","Jian","Die","Chuai","Zhong","Ju","Duo","Cuo","Pian","Rou","Nie","Pan","Qi","Chu","Jue",
                "Pu","Fan","Cu","Zhu","Lin","Chan","Lie","Zuan","Xie","Zhi","Diao","Mo","Xiu","Mo","Pi","Hu",
                "Jue","Shang","Gu","Zi","Gong","Su","Zhi","Zi","Qing","Liang","Yu","Li","Wen","Ting","Ji","Pei",
                "Fei","Sha","Yin","Ai","Xian","Mai","Chen","Ju","Bao","Tiao","Zi","Yin","Yu","Chuo","Wo","Mian",
                "Yuan","Tuo","Zhui","Sun","Jun","Ju","Luo","Qu","Chou","Qiong","Luan","Wu","Zan","Mou","Ao","Liu",
                "Bei","Xin","You","Fang","Ba","Ping","Nian","Lu","Su","Fu","Hou","Tai","Gui","Jie","Wei","Er",
                "Ji","Jiao","Xiang","Xun","Geng","Li","Lian","Jian","Shi","Tiao","Gun","Sha","Huan","Ji","Qing","Ling",
                "Zou","Fei","Kun","Chang","Gu","Ni","Nian","Diao","Shi","Zi","Fen","Die","E","Qiu","Fu","Huang",
                "Bian","Sao","Ao","Qi","Ta","Guan","Yao","Le","Biao","Xue","Man","Min","Yong","Gui","Shan","Zun",
                "Li","Da","Yang","Da","Qiao","Man","Jian","Ju","Rou","Gou","Bei","Jie","Tou","Ku","Gu","Di",
                "Hou","Ge","Ke","Bi","Lou","Qia","Kuan","Bin","Du","Mei","Ba","Yan","Liang","Xiao","Wang","Chi",
                "Xiang","Yan","Tie","Tao","Yong","Biao","Kun","Mao","Ran","Tiao","Ji","Zi","Xiu","Quan","Jiu","Bin",
                "Huan","Lie","Me","Hui","Mi","Ji","Jun","Zhu","Mi","Qi","Ao","She","Lin","Dai","Chu","You",
                "Xia","Yi","Qu","Du","Li","Qing","Can","An","Fen","You","Wu","Yan","Xi","Qiu","Han","Zha"
           };
        #endregion 二級漢字
        #region 變數定義
        // GB2312-80 標準規範中第一個漢字的機內碼.即"啊"的機內碼
        private const int firstChCode = -20319;
        // GB2312-80 標準規範中最後一個漢字的機內碼.即"齇"的機內碼
        private const int lastChCode = -2050;
        // GB2312-80 標準規範中最後一個一級漢字的機內碼.即"座"的機內碼
        private const int lastOfOneLevelChCode = -10247;
        // 配置中文字元
        //static Regex regex = new Regex("[\u4e00-\u9fa5]$");

        #endregion
        #endregion

        /// <summary>
        /// 取拼音第一個欄位
        /// </summary>        
        /// <param name="ch"></param>        
        /// <returns></returns>        
        public static String GetFirst(Char ch)
        {
            var rs = Get(ch);
            if (!String.IsNullOrEmpty(rs)) rs = rs.Substring(0, 1);

            return rs;
        }

        /// <summary>
        /// 取拼音第一個欄位
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static String GetFirst(String str)
        {
            if (String.IsNullOrEmpty(str)) return String.Empty;

            var sb = new StringBuilder(str.Length + 1);
            var chs = str.ToCharArray();

            for (var i = 0; i < chs.Length; i++)
            {
                sb.Append(GetFirst(chs[i]));
            }

            return sb.ToString();
        }

        /// <summary>
        /// 獲取單字拼音
        /// </summary>
        /// <param name="ch"></param>
        /// <returns></returns>
        public static String Get(Char ch)
        {
            // 拉丁字元            
            if (ch <= '\x00FF') return ch.ToString();

            // 標點符號、分隔符號            
            if (Char.IsPunctuation(ch) || Char.IsSeparator(ch)) return ch.ToString();

            // 非中文字元            
            if (ch < '\x4E00' || ch > '\x9FA5') return ch.ToString();

            var arr = Encoding.GetEncoding("gb2312").GetBytes(ch.ToString());
            //Encoding.Default默認在中文環境裡雖是GB2312，但在多變的環境可能是其它
            //var arr = Encoding.Default.GetBytes(ch.ToString()); 
            var chr = (Int16)arr[0] * 256 + (Int16)arr[1] - 65536;

            //***// 單字元--英文或半形字元  
            if (chr > 0 && chr < 160) return ch.ToString();
            #region 中文字元處理

            // 判斷是否超過GB2312-80標準中的漢字範圍
            if (chr > lastChCode || chr < firstChCode)
            {
                return ch.ToString(); ;
            }
            // 如果是在一級漢字中
            else if (chr <= lastOfOneLevelChCode)
            {
                // 將一級漢字分為12塊,每塊33個漢字.
                for (int aPos = 11; aPos >= 0; aPos--)
                {
                    int aboutPos = aPos * 33;
                    // 從最後的塊開始掃描,如果機內碼大於塊的第一個機內碼,說明在此塊中
                    if (chr >= pyValue[aboutPos])
                    {
                        // Console.WriteLine("存在於第 " + aPos.ToString() + " 塊,此塊的第一個機內碼是: " + pyValue[aPos * 33].ToString());
                        // 遍歷塊中的每個音節機內碼,從最後的音節機內碼開始掃描,
                        // 如果音節內碼小於機內碼,則取此音節
                        for (int i = aboutPos + 32; i >= aboutPos; i--)
                        {
                            if (pyValue[i] <= chr)
                            {
                                // Console.WriteLine("找到第一個小於要查找機內碼的機內碼: " + pyValue[i].ToString());
                                return pyName[i];
                            }
                        }
                        break;
                    }
                }
            }
            // 如果是在二級漢字中
            else
            {
                int pos = Array.IndexOf(otherChinese, ch.ToString());
                if (pos != decimal.MinusOne)
                {
                    return otherPinYin[pos];
                }
            }
            #endregion 中文字元處理

            //if (chr < -20319 || chr > -10247) { // 不知道的字元  
            //    return null;  

            //for (var i = pyValue.Length - 1; i >= 0; i--)
            //{                
            //    if (pyValue[i] <= chr) return pyName[i];//這只能對應陣列已經定義的           
            //}             

            return String.Empty;
        }

        /// <summary>
        /// 把漢字轉換成拼音(全拼)
        /// </summary>
        /// <param name="str">中文字元串</param>
        /// <returns>轉換後的拼音(全拼)字串</returns>
        public static String GetPinYin(String str)
        {
            if (String.IsNullOrEmpty(str)) return String.Empty;

            var sb = new StringBuilder(str.Length * 10);
            var chs = str.ToCharArray();

            for (var j = 0; j < chs.Length; j++)
            {
                sb.Append(Get(chs[j]));
            }

            return sb.ToString();
        }
        #endregion

        #region  獲取網頁的HTML內容
        // 獲取網頁的HTML內容，指定Encoding
        public static string GetHtml(string url, Encoding encoding)
        {
            byte[] buf = new WebClient().DownloadData(url);
            if (encoding != null) return encoding.GetString(buf);
            string html = Encoding.UTF8.GetString(buf);
            encoding = GetEncoding(html);
            if (encoding == null || encoding == Encoding.UTF8) return html;
            return encoding.GetString(buf);
        }
        // 根據網頁的HTML內容提取網頁的Encoding
        public static Encoding GetEncoding(string html)
        {
            string pattern = @"(?i)\bcharset=(?<charset>[-a-zA-Z_0-9]+)";
            string charset = Regex.Match(html, pattern).Groups["charset"].Value;
            try { return Encoding.GetEncoding(charset); }
            catch (ArgumentException) { return null; }
        }
        #endregion
    }
}
