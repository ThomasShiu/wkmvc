using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace Common.Enums
{
    /// <summary>
    /// 枚舉獨特類
    /// add yuangang by 2016-05-10
    /// </summary>
    public class EnumsClass
    {
        /// <summary>
        /// 枚舉value
        /// </summary>
        public int Value { get; set; }
        /// <summary>
        /// 枚舉顯示值
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 枚舉說明
        /// </summary>
        public string Text { get; set; }
    }

    #region 系統管理相關
    /// <summary>
    /// 系統操作枚舉
    /// </summary>
    public enum enumOperator
    {
        /// <summary>
        /// 無
        /// </summary>
        [Description("無")]
        None,
        /// <summary>
        /// 查詢
        /// </summary>
        [Description("查詢")]
        Select,
        /// <summary>
        /// 添加
        /// </summary>
        [Description("添加")]
        Add,
        /// <summary>
        /// 修改
        /// </summary>
        [Description("修改")]
        Edit,
        /// <summary>
        /// 移除
        /// </summary>
        [Description("移除")]
        Remove,
        /// <summary>
        /// 登錄
        /// </summary>
        [Description("登錄")]
        Login,
        /// <summary>
        /// 登出
        /// </summary>
        [Description("登出")]
        LogOut,
        /// <summary>
        /// 匯出
        /// </summary>
        [Description("匯出")]
        Export,
        /// <summary>
        /// 導入
        /// </summary>
        [Description("導入")]
        Import,
        /// <summary>
        /// 審核
        /// </summary>
        [Description("審核")]
        Audit,
        /// <summary>
        /// 回復
        /// </summary>
        [Description("回復")]
        Reply,
        /// <summary>
        /// 下載
        /// </summary>
        [Description("下載")]
        Download,
        /// <summary>
        /// 上傳
        /// </summary>
        [Description("上傳")]
        Upload,
        /// <summary>
        /// 分配
        /// </summary>
        [Description("分配")]
        Allocation,
        /// <summary>
        /// 文件
        /// </summary>
        [Description("文件")]
        Files,
        /// <summary>
        /// 流程
        /// </summary>
        [Description("流程")]
        Flow
    }
    /// <summary>
    /// log4net枚舉
    /// </summary>
    public enum enumLog4net
    {
        [Description("普通")]
        INFO,
        [Description("警告")]
        WARN,
        [Description("錯誤")]
        ERROR,
        [Description("異常")]
        FATAL

    }
    /// <summary>
    /// 模組類別枚舉,對應TBSYS_Module表的ModuleType欄位
    /// </summary>
    public enum enumModuleType
    {
        無頁面 = 1,
        列表頁 = 2,
        彈出頁 = 3
    }
    /// <summary>
    /// 部門類型
    /// </summary>
    public enum enumDepartmentType
    {
        勝利石油管理局 = 1,
        施工隊 = 2,
        工程部 = 3,
        計畫科 = 4,
        其他單位 = 5

    }

    #endregion

    #region 流程枚舉
    /// <summary>
    /// 流程枚舉
    /// </summary>
    public enum FLowEnums
    {
        /// <summary>
        /// 空白
        /// </summary>
        [Description("空白")]
        Blank = 0,
        /// <summary>
        /// 草稿
        /// </summary>
        [Description("草稿")]
        Draft = 1,
        /// <summary>
        /// 運行中
        /// </summary>
        [Description("運行中")]
        Runing = 2,
        /// <summary>
        /// 已完成
        /// </summary>
        [Description("已完成")]
        Complete = 3,
        /// <summary>
        /// 掛起
        /// </summary>
        [Description("掛起")]
        HungUp = 4,
        /// <summary>
        /// 退回
        /// </summary>
        [Description("退回")]
        ReturnSta = 5,
        /// <summary>
        /// 轉發(移交)
        /// </summary>
        [Description("移交")]
        Shift = 6,
        /// <summary>
        /// 刪除(邏輯刪除狀態)
        /// </summary>
        [Description("刪除")]
        Delete = 7,
        /// <summary>
        /// 加簽
        /// </summary>
        [Description("加簽")]
        Askfor = 8,
        /// <summary>
        /// 凍結
        /// </summary>
        [Description("凍結")]
        Fix = 9,
        /// <summary>
        /// 批次處理
        /// </summary>
        [Description("批次處理")]
        Batch = 10,
        /// <summary>
        /// 加簽回復狀態
        /// </summary>
        [Description("加簽回復")]
        AskForReplay = 11
    }
    #endregion

    #region 系統字典

    /// <summary>
    /// 類描述:系統字典
    /// 創建標識:add yuangang by 2016-05-10
    /// </summary>
    public class ClsDic
    {
        /// <summary>
        /// 聊天資訊類型
        /// </summary>
        public static Dictionary<string, int> DicMessageType
        {
            get
            {
                return new Dictionary<string, int>
                {
                    {"廣播",0},
                    {"群組",1},
                    {"私聊",2}
                };
            }
        }


        /// <summary>
        /// 根據DicKey值獲取value
        /// </summary>
        public static string GetDicValueByKey(string key, Dictionary<string, string> p)
        {
            if (p == null || p.Count == 0) return "";
            var dic = p.GetEnumerator();
            while (dic.MoveNext())
            {
                var obj = dic.Current;
                if (key == obj.Key)
                    return obj.Value;
            }
            return "";
        }
        /// <summary>
        /// 根據DICValue獲取Key
        /// </summary>
        public static string GetDicKeyByValue(string value, Dictionary<string, string> p)
        {
            if (p == null || p.Count == 0) return "";
            var dic = p.GetEnumerator();
            while (dic.MoveNext())
            {
                var obj = dic.Current;
                if (obj.Value == value)
                    return obj.Key;
            }
            return "";
        }
        /// <summary>
        /// 描述:實體與編碼對應字典,在驗證資料許可權時,通過此處字典來枚舉實體編號
        /// <author>創建標識: add yuangang by 2016-05-10</author>
        /// </summary>
        public static Dictionary<string, string> DicEntity
        {
            get
            {
                Dictionary<string, string> _dic = new Dictionary<string, string>();
                _dic.Add("日誌", "");
                _dic.Add("用戶", "18da4207-3bfc-49ea-90f7-16867721805c");
                return _dic;
            }
        }
        /// <summary>
        /// 描述:存放特別的角色編號字典,在驗證操作許可權時用到
        /// 創建標識:add by liuj 2013-8-9 9:56
        /// </summary>
        public static Dictionary<string, int> DicRole
        {
            get
            {
                Dictionary<string, int> _dic = new Dictionary<string, int>();
                _dic.Add("超級管理員", 1);
                return _dic;
            }
        }
        /// <summary>
        /// 字典類型
        /// </summary>
        public static Dictionary<string, string> DicCodeType
        {
            get
            {
                Dictionary<string, string> _dic = new Dictionary<string, string>();

                string dicStr = "職務-ZW,在崗狀態-ZGZT,婚姻狀況-HYZK,學歷-XL,政治面貌-ZZMM,民族-MZ,職稱-ZC,業務授權-YWSQ,職位類型-POSTTYPE,許可權值-ROLEVALUE,員工級別-YGJB,合同類型-HTLB,合同狀態-HTZT,客戶分類-KHFL,IP類型-IPLX,專案類型-XMLX,專案狀態-XMZT,成本類別-CBLB,任務狀態-RWZT,任務類別-RWLB,任務階段-RWJD,日誌類別-RZLB,文章類別-WZLB,流程狀態-WZZT,優先順序-YXJ,連絡人類型-LXRLX,消息類型-XXLX";
                var diclist = dicStr.TrimEnd(',').TrimStart(',').Split(',').ToList();
                if (diclist.Count > 0)
                {
                    foreach (var item in diclist)
                    {
                        _dic.Add(item.Split('-')[0], item.Split('-')[1]);
                    }
                }

                return _dic;
            }
        }
        /// <summary>
        /// 附件上傳路徑
        /// 創建標識:add yuangang by 2016-05-10
        /// </summary>
        public static Dictionary<string, string> DicAttachmentPath
        {
            get
            {
                Dictionary<string, string> _dic = new Dictionary<string, string>();
                _dic.Add("上傳路徑", System.Configuration.ConfigurationManager.AppSettings["upfile"]);
                _dic.Add("檔案簡歷", System.Configuration.ConfigurationManager.AppSettings["upfile"]);
                _dic.Add("手機文件", System.Configuration.ConfigurationManager.AppSettings["upphone"]);
                _dic.Add("手機照片", System.Configuration.ConfigurationManager.AppSettings["photofile"]);
                _dic.Add("技術檔", System.Configuration.ConfigurationManager.AppSettings["upTsfile"]);
                _dic.Add("工程圖", System.Configuration.ConfigurationManager.AppSettings["UploadFiles"]);
                _dic.Add("檔案頭像", System.Configuration.ConfigurationManager.AppSettings["upfile"]);
                return _dic;
            }
        }
        /// <summary>
        /// 業務辦理圖片寬高
        /// 創建標識:add yuangang by 2016-05-10
        /// </summary>
        public static Dictionary<string, string> DicImageWH
        {
            get
            {
                Dictionary<string, string> _dic = new Dictionary<string, string>();
                _dic.Add("圖片寬度", System.Configuration.ConfigurationManager.AppSettings["imgWidth"]);
                _dic.Add("圖片高度", System.Configuration.ConfigurationManager.AppSettings["imgHeight"]);
                _dic.Add("手機用戶頭像高", System.Configuration.ConfigurationManager.AppSettings["UserPhotoHeight"]);
                _dic.Add("手機用戶頭像寬", System.Configuration.ConfigurationManager.AppSettings["UserPhotoWidth"]);
                _dic.Add("用戶頭像高", System.Configuration.ConfigurationManager.AppSettings["PolicePhotoHeight"]);
                _dic.Add("用戶頭像寬", System.Configuration.ConfigurationManager.AppSettings["PolicePhotoWidth"]);
                return _dic;
            }
        }
        /// <summary>
        /// 警務室圖片寬高
        /// 創建標識:add yuangang by 2016-05-10
        /// </summary>
        public static Dictionary<string, string> DicPoliceHouseImageWH
        {
            get
            {
                Dictionary<string, string> _dic = new Dictionary<string, string>();
                _dic.Add("圖片寬度", System.Configuration.ConfigurationManager.AppSettings["imgPoliceWidth"]);
                _dic.Add("圖片高度", System.Configuration.ConfigurationManager.AppSettings["imgPoliceHeight"]);
                return _dic;
            }
        }
        /// <summary>
        /// OracleReportData
        /// 創建標識:add yuangang by 2016-05-10
        /// </summary>
        public static Dictionary<string, string> OracleReportData
        {
            get
            {
                Dictionary<string, string> _dic = new Dictionary<string, string>();
                _dic.Add("OrcalReport", System.Configuration.ConfigurationManager.AppSettings["connectionString"]);
                return _dic;
            }
        }
        /// <summary>
        /// 手機用戶端命名
        /// 創建標識:add yuangang by 2016-05-10
        /// </summary>
        public static Dictionary<string, string> DicPhone
        {
            get
            {
                Dictionary<string, string> _dic = new Dictionary<string, string>();
                _dic.Add("安卓程式", System.Configuration.ConfigurationManager.AppSettings["AndroidName"]);
                _dic.Add("蘋果程式", System.Configuration.ConfigurationManager.AppSettings["IOSName"]);
                return _dic;
            }
        }
        /// <summary>
        /// 功能描述：記錄Cookie的Key值 
        /// 創建標識：徐戈
        /// </summary>
        public static Dictionary<string, string> DicCookie
        {
            get
            {
                Dictionary<string, string> _dic = new Dictionary<string, string>();
                _dic.Add("Session中存儲的帳號和CookieID", "AccountCookieID_Session");
                _dic.Add("Cookie中存儲的帳號和CookieID", "AccountCookieIDNew");
                return _dic;
            }
        }
        /// <summary>
        /// 功能描述：記錄Cookie的Key值 
        /// 創建標識：徐戈
        /// </summary>
        public static Dictionary<string, string> DicCookieTimeout
        {
            get
            {
                Dictionary<string, string> _dic = new Dictionary<string, string>();
                _dic.Add("帳號過期時間", "30");
                return _dic;
            }
        }


        public static Dictionary<string, int> DicProject
        {
            get
            {
                return new Dictionary<string, int>
                {
                    {
                        "準備中",
                        0
                    },
                    {
                        "進行中",
                        1
                    },
                    {
                        "延期",
                        2
                    },
                    {
                        "已超時",
                        3
                    },
                    {
                        "已終止",
                        4
                    },
                    {
                        "已驗收",
                        5
                    },
                    {
                        "已完成",
                        6
                    },
                    {
                        "已失敗",
                        7
                    },
                    {
                        "已違約",
                        8
                    },
                    {
                        "對方違約",
                        9
                    }
                };
            }
        }

        public static System.Collections.Generic.Dictionary<string, int> DicStatus
        {
            get
            {
                return new System.Collections.Generic.Dictionary<string, int>
                {
                    {
                        "駁回",
                        0
                    },
                    {
                        "通過",
                        1
                    },
                    {
                        "等待審核",
                        2
                    }
                };
            }
        }
    }
    #endregion

    #region 業務相關
    /// <summary>
    /// 計畫流轉狀態
    /// </summary>
    public enum enumHCA_RecognitionProgramProcessType
    {
        上報 = 1,
        同意 = 2,
        不同意 = 3
    }
    /// <summary>
    /// 上傳檔案類型
    /// </summary>
    public enum enumFileType
    {
        其他 = 0,
        Word = 1,
        Excel = 2,
        圖片 = 3,
        PPT = 4,
        PDF = 5,
        RAR = 6
    }
    /// <summary>
    ///路單狀態
    /// </summary>
    public enum enumWAYBILLSTATE
    {
        分派 = 1,
        列印 = 2,
        資料錄入 = 3,
        數據填報 = 4,
        車隊審核回收 = 5,
        刪除 = 6,
        作廢 = 7,
        交接 = 8,
        納入結算 = 9,
        完成結算 = 10


    }
    /// <summary>
    /// 來源
    /// </summary>
    public enum enumORIGIN
    {
        自建 = 1,
        任務 = 2,
        外委申請 = 3
    }

    /// <summary>
    /// 應急物資規格型號
    /// </summary>
    public enum enumReliefGoodsModel
    {
        規格型號1 = 1,
        規格型號2 = 2,
        規格型號3 = 3
    }
    /// <summary>
    /// 應急搶險救援物資類別
    /// </summary>
    public enum enumReliefGoodsType
    {
        溢油 = 1,
        防汛 = 2
    }
    /// <summary>
    /// 業務諮詢枚舉,對應業務諮詢表的bptype欄位
    /// </summary>
    public enum enumBptType
    {
        線上諮詢 = 401002,
        身份證 = 501001,
        戶籍 = 501002,
        治安管理 = 501003,
        出入境 = 501004,
        消防 = 501005,
        其他業務 = 501006,
        交警 = 501007,
        網安 = 501008,
        法制 = 501009
    }

    public enum enumNewsType
    {
        警務信息 = 301001,
        警方公告 = 301002,
        防範提示 = 101501
    }

    /// <summary>
    /// 上傳檔案類型
    /// </summary>
    public enum enumBusType
    {

        車輛圖片上傳 = 100001,
        套管圖片上傳 = 103002,
        三通圖片上傳 = 103003,
        閥門圖片上傳 = 103004,
        占壓圖片上傳 = 103005,


    }


    /// <summary>
    /// 管道維修應急預案級別
    /// </summary>
    public enum enumEmergencyPlanLevel
    {
        中石化 = 1,
        油田 = 2,
        總廠 = 3,
        分廠 = 4
    }

    /// <summary>
    /// 陽極材料
    /// </summary>
    public enum enumAnodeMaterial
    {
        未知 = 0,
        鍍鉑陽極 = 1,
        磁性氧化鐵 = 2,
        混合金屬氧化物 = 3,
        鎂 = 4,
        鋅 = 5,
        鉑 = 6,
        高矽鑄鐵 = 7,
        石墨 = 8,
        廢鋼鐵 = 9,
        碳 = 10,
        鋁合金 = 11,
        其它 = 99
    }


    /// <summary>
    /// 業務諮詢處理狀態枚舉,對應業務諮詢表的requesStatus欄位
    /// </summary>
    public enum enumBussinessType
    {
        後臺辦理本部門業務 = 1,
        手機辦理本部門業務 = 2,
        手機業務 = 3,
        社區民警 = 4
    }

    /// <summary>
    /// 業務諮詢處理狀態枚舉,對應業務諮詢表的requesStatus欄位
    /// </summary>
    public enum enumRequesStatus
    {
        用戶提交 = 0,
        指定處理 = 1,
        處理完成 = 2
    }

    public enum enumWorkType
    {
        未指定 = -1,
        手機方式 = 0,
        電腦Web = 1
    }
    public enum enumIsBool
    {
        是 = 1,
        否 = 2
    }

    public enum enumPhoneUserType
    {
        註冊用戶 = 1,
        匿名使用者 = 2
    }

    public enum enumReplyType
    {
        未處理 = 0,
        審核通過 = 1,
        審核不通過 = 2
    }

    public enum enumBlogType
    {
        新浪微博 = 0,
        騰訊微博 = 1,
        東營公安局的騰訊微博 = 2
    }


    #endregion

}
