using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;
using System.Runtime.Serialization.Json;
using System.IO;

namespace Common.JsonHelper
{

    /// <summary>
    /// 提供了一個關於json的輔助類
    /// </summary>
    public class JsonHelper
    {
        #region Method
        /// <summary>
        /// 類對像轉換成json格式
        /// </summary> 
        /// <returns></returns>
        public static string ToJson(object t)
        {
            return JsonConvert.SerializeObject(t, Newtonsoft.Json.Formatting.Indented, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Include });
        }
        /// <summary>
        /// 類對像轉換成json格式
        /// </summary>
        /// <param name="t"></param>
        /// <param name="HasNullIgnore">是否忽略NULL值</param>
        /// <returns></returns>
        public static string ToJson(object t, bool HasNullIgnore)
        {
            if (HasNullIgnore)
                return JsonConvert.SerializeObject(t, Newtonsoft.Json.Formatting.Indented, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
            else
                return ToJson(t);
        }
        /// <summary>
        /// json格式轉換
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="strJson"></param>
        /// <returns></returns>
        public static T FromJson<T>(string strJson) where T : class
        {
            if (!strJson.IsNullOrEmpty())
                return JsonConvert.DeserializeObject<T>(strJson);
            return null;
        }
        /// <summary>
        /// 功能描述：將List轉換為Json
        /// </summary>
        /// <param name="a"></param>
        /// <returns></returns>
        public static string ListToJson(IList<object> a)
        {

            DataContractJsonSerializer json = new DataContractJsonSerializer(a.GetType());
            string szJson = "";
            //序列化
            using (MemoryStream stream = new MemoryStream())
            {
                json.WriteObject(stream, a);
                szJson = Encoding.UTF8.GetString(stream.ToArray());
            }
            return szJson;
        }
        #endregion

        #region Property
        /// <summary>
        /// 資料狀態
        /// </summary>
        public string Status { get; set; }
        /// <summary>
        /// 提示資訊
        /// </summary>
        public string Msg { get; set; }
        /// <summary>
        /// 回傳URL
        /// </summary>
        public string ReUrl { get; set; }
        /// <summary>
        /// 數據包
        /// </summary>
        public object Data { get; set; }
        #endregion

    }
}
