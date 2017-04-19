
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Common.JsonHelper
{
    /// <summary>
    /// 判斷字串是否為JSON
    /// </summary>
    public class JsonSplit
    {
        private static bool IsJsonStart(ref string json)
        {
            if (!string.IsNullOrEmpty(json))
            {
                json = json.Trim('\r', '\n', ' ');
                if (json.Length > 1)
                {
                    char s = json[0];
                    char e = json[json.Length - 1];
                    return (s == '{' && e == '}') || (s == '[' && e == ']');
                }
            }
            return false;
        }
        public static bool IsJson(string json)
        {
            int errIndex;
            return IsJson(json, out errIndex);
        }
        public static bool IsJson(string json, out int errIndex)
        {
            errIndex = 0;
            if (IsJsonStart(ref json))
            {
                CharState cs = new CharState();
                char c;
                for (int i = 0; i < json.Length; i++)
                {
                    c = json[i];
                    if (SetCharState(c, ref cs) && cs.childrenStart)//設置關鍵符號狀態。
                    {
                        string item = json.Substring(i);
                        int err;
                        int length = GetValueLength(item, true, out err);
                        cs.childrenStart = false;
                        if (err > 0)
                        {
                            errIndex = i + err;
                            return false;
                        }
                        i = i + length - 1;
                    }
                    if (cs.isError)
                    {
                        errIndex = i;
                        return false;
                    }
                }

                return !cs.arrayStart && !cs.jsonStart;
            }
            return false;
        }

        /// <summary>
        /// 獲取值的長度（當Json值嵌套以"{"或"["開頭時）
        /// </summary>
        private static int GetValueLength(string json, bool breakOnErr, out int errIndex)
        {
            errIndex = 0;
            int len = 0;
            if (!string.IsNullOrEmpty(json))
            {
                CharState cs = new CharState();
                char c;
                for (int i = 0; i < json.Length; i++)
                {
                    c = json[i];
                    if (!SetCharState(c, ref cs))//設置關鍵符號狀態。
                    {
                        if (!cs.jsonStart && !cs.arrayStart)//json結束，又不是陣列，則退出。
                        {
                            break;
                        }
                    }
                    else if (cs.childrenStart)//正常字元，值狀態下。
                    {
                        int length = GetValueLength(json.Substring(i), breakOnErr, out errIndex);//遞迴子值，返回一個長度。。。
                        cs.childrenStart = false;
                        cs.valueStart = 0;
                        //cs.state = 0;
                        i = i + length - 1;
                    }
                    if (breakOnErr && cs.isError)
                    {
                        errIndex = i;
                        return i;
                    }
                    if (!cs.jsonStart && !cs.arrayStart)//記錄當前結束位置。
                    {
                        len = i + 1;//長度比索引+1
                        break;
                    }
                }
            }
            return len;
        }
        /// <summary>
        /// 字元狀態
        /// </summary>
        private class CharState
        {
            internal bool jsonStart = false;//以 "{"開始了...
            internal bool setDicValue = false;// 可以設置字典值了。
            internal bool escapeChar = false;//以"\"轉義符號開始了
            /// <summary>
            /// 陣列開始【僅第一開頭才算】，值嵌套的以【childrenStart】來標識。
            /// </summary>
            internal bool arrayStart = false;//以"[" 符號開始了
            internal bool childrenStart = false;//子級嵌套開始了。
            /// <summary>
            /// 【0 初始狀態，或 遇到“,”逗號】；【1 遇到“：”冒號】
            /// </summary>
            internal int state = 0;

            /// <summary>
            /// 【-1 取值結束】【0 未開始】【1 無引號開始】【2 單引號開始】【3 雙引號開始】
            /// </summary>
            internal int keyStart = 0;
            /// <summary>
            /// 【-1 取值結束】【0 未開始】【1 無引號開始】【2 單引號開始】【3 雙引號開始】
            /// </summary>
            internal int valueStart = 0;
            internal bool isError = false;//是否語法錯誤。

            internal void CheckIsError(char c)//只當成一級處理（因為GetLength會遞迴到每一個子項處理）
            {
                if (keyStart > 1 || valueStart > 1)
                {
                    return;
                }
                //示例 ["aa",{"bbbb":123,"fff","ddd"}] 
                switch (c)
                {
                    case '{'://[{ "[{A}]":[{"[{B}]":3,"m":"C"}]}]
                        isError = jsonStart && state == 0;//重複開始錯誤 同時不是值處理。
                        break;
                    case '}':
                        isError = !jsonStart || (keyStart != 0 && state == 0);//重複結束錯誤 或者 提前結束{"aa"}。正常的有{}
                        break;
                    case '[':
                        isError = arrayStart && state == 0;//重複開始錯誤
                        break;
                    case ']':
                        isError = !arrayStart || jsonStart;//重複開始錯誤 或者 Json 未結束
                        break;
                    case '"':
                    case '\'':
                        isError = !(jsonStart || arrayStart); //json 或陣列開始。
                        if (!isError)
                        {
                            //重複開始 [""",{"" "}]
                            isError = (state == 0 && keyStart == -1) || (state == 1 && valueStart == -1);
                        }
                        if (!isError && arrayStart && !jsonStart && c == '\'')//['aa',{}]
                        {
                            isError = true;
                        }
                        break;
                    case ':':
                        isError = !jsonStart || state == 1;//重複出現。
                        break;
                    case ',':
                        isError = !(jsonStart || arrayStart); //json 或陣列開始。
                        if (!isError)
                        {
                            if (jsonStart)
                            {
                                isError = state == 0 || (state == 1 && valueStart > 1);//重複出現。
                            }
                            else if (arrayStart)//["aa,] [,]  [{},{}]
                            {
                                isError = keyStart == 0 && !setDicValue;
                            }
                        }
                        break;
                    case ' ':
                    case '\r':
                    case '\n'://[ "a",\r\n{} ]
                    case '\0':
                    case '\t':
                        break;
                    default: //值開頭。。
                        isError = (!jsonStart && !arrayStart) || (state == 0 && keyStart == -1) || (valueStart == -1 && state == 1);//
                        break;
                }
                //if (isError)
                //{

                //}
            }
        }
        /// <summary>
        /// 設置字元狀態(返回true則為關鍵字，返回false則當為普通字元處理）
        /// </summary>
        private static bool SetCharState(char c, ref CharState cs)
        {
            cs.CheckIsError(c);
            switch (c)
            {
                case '{'://[{ "[{A}]":[{"[{B}]":3,"m":"C"}]}]
                    #region 大括弧
                    if (cs.keyStart <= 0 && cs.valueStart <= 0)
                    {
                        cs.keyStart = 0;
                        cs.valueStart = 0;
                        if (cs.jsonStart && cs.state == 1)
                        {
                            cs.childrenStart = true;
                        }
                        else
                        {
                            cs.state = 0;
                        }
                        cs.jsonStart = true;//開始。
                        return true;
                    }
                    #endregion
                    break;
                case '}':
                    #region 大括弧結束
                    if (cs.keyStart <= 0 && cs.valueStart < 2 && cs.jsonStart)
                    {
                        cs.jsonStart = false;//正常結束。
                        cs.state = 0;
                        cs.keyStart = 0;
                        cs.valueStart = 0;
                        cs.setDicValue = true;
                        return true;
                    }
                    // cs.isError = !cs.jsonStart && cs.state == 0;
                    #endregion
                    break;
                case '[':
                    #region 中括弧開始
                    if (!cs.jsonStart)
                    {
                        cs.arrayStart = true;
                        return true;
                    }
                    else if (cs.jsonStart && cs.state == 1)
                    {
                        cs.childrenStart = true;
                        return true;
                    }
                    #endregion
                    break;
                case ']':
                    #region 中括弧結束
                    if (cs.arrayStart && !cs.jsonStart && cs.keyStart <= 2 && cs.valueStart <= 0)//[{},333]//這樣結束。
                    {
                        cs.keyStart = 0;
                        cs.valueStart = 0;
                        cs.arrayStart = false;
                        return true;
                    }
                    #endregion
                    break;
                case '"':
                case '\'':
                    #region 引號
                    if (cs.jsonStart || cs.arrayStart)
                    {
                        if (cs.state == 0)//key階段,有可能是陣列["aa",{}]
                        {
                            if (cs.keyStart <= 0)
                            {
                                cs.keyStart = (c == '"' ? 3 : 2);
                                return true;
                            }
                            else if ((cs.keyStart == 2 && c == '\'') || (cs.keyStart == 3 && c == '"'))
                            {
                                if (!cs.escapeChar)
                                {
                                    cs.keyStart = -1;
                                    return true;
                                }
                                else
                                {
                                    cs.escapeChar = false;
                                }
                            }
                        }
                        else if (cs.state == 1 && cs.jsonStart)//值階段必須是Json開始了。
                        {
                            if (cs.valueStart <= 0)
                            {
                                cs.valueStart = (c == '"' ? 3 : 2);
                                return true;
                            }
                            else if ((cs.valueStart == 2 && c == '\'') || (cs.valueStart == 3 && c == '"'))
                            {
                                if (!cs.escapeChar)
                                {
                                    cs.valueStart = -1;
                                    return true;
                                }
                                else
                                {
                                    cs.escapeChar = false;
                                }
                            }

                        }
                    }
                    #endregion
                    break;
                case ':':
                    #region 冒號
                    if (cs.jsonStart && cs.keyStart < 2 && cs.valueStart < 2 && cs.state == 0)
                    {
                        if (cs.keyStart == 1)
                        {
                            cs.keyStart = -1;
                        }
                        cs.state = 1;
                        return true;
                    }
                    // cs.isError = !cs.jsonStart || (cs.keyStart < 2 && cs.valueStart < 2 && cs.state == 1);
                    #endregion
                    break;
                case ',':
                    #region 逗號 //["aa",{aa:12,}]

                    if (cs.jsonStart)
                    {
                        if (cs.keyStart < 2 && cs.valueStart < 2 && cs.state == 1)
                        {
                            cs.state = 0;
                            cs.keyStart = 0;
                            cs.valueStart = 0;
                            //if (cs.valueStart == 1)
                            //{
                            //    cs.valueStart = 0;
                            //}
                            cs.setDicValue = true;
                            return true;
                        }
                    }
                    else if (cs.arrayStart && cs.keyStart <= 2)
                    {
                        cs.keyStart = 0;
                        //if (cs.keyStart == 1)
                        //{
                        //    cs.keyStart = -1;
                        //}
                        return true;
                    }
                    #endregion
                    break;
                case ' ':
                case '\r':
                case '\n'://[ "a",\r\n{} ]
                case '\0':
                case '\t':
                    if (cs.keyStart <= 0 && cs.valueStart <= 0) //cs.jsonStart && 
                    {
                        return true;//跳過空格。
                    }
                    break;
                default: //值開頭。。
                    if (c == '\\') //轉義符號
                    {
                        if (cs.escapeChar)
                        {
                            cs.escapeChar = false;
                        }
                        else
                        {
                            cs.escapeChar = true;
                            return true;
                        }
                    }
                    else
                    {
                        cs.escapeChar = false;
                    }
                    if (cs.jsonStart || cs.arrayStart) // Json 或陣列開始了。
                    {
                        if (cs.keyStart <= 0 && cs.state == 0)
                        {
                            cs.keyStart = 1;//無引號的
                        }
                        else if (cs.valueStart <= 0 && cs.state == 1 && cs.jsonStart)//只有Json開始才有值。
                        {
                            cs.valueStart = 1;//無引號的
                        }
                    }
                    break;
            }
            return false;
        }
    }
}