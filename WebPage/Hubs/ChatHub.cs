using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Common;
using Common.Enums;
using Common.JsonHelper;
using Domain;
using Microsoft.AspNet.SignalR;
using Service.IService;
using Spring.Context.Support;

namespace WebPage.Hubs
{
    public class ChatHub : Hub
    {
        IUserManage UserManage = ContextRegistry.GetContext().GetObject("Service.User") as IUserManage;

        IDepartmentManage DepartmentManage = ContextRegistry.GetContext().GetObject("Service.Department") as IDepartmentManage;

        ICodeManage CodeManage = ContextRegistry.GetContext().GetObject("Service.Code") as ICodeManage;

        IUserOnlineManage UserOnlineManage = ContextRegistry.GetContext().GetObject("Service.UserOnline") as IUserOnlineManage;

        IChatMessageManage ChatMessageManage = ContextRegistry.GetContext().GetObject("Service.ChatMessage") as IChatMessageManage;
        /// <summary>
        /// 使用者登錄註冊資訊
        /// </summary>
        /// <param name="id"></param>        
        public void Register(string account, string password)
        {
            try
            {
                //獲取使用者資訊
                var User = UserManage.Get(p => p.ACCOUNT == account);
                if (User != null && User.PASSWORD == password)
                {
                    //更新線上狀態
                    var UserOnline = UserOnlineManage.LoadListAll(p => p.FK_UserId == User.ID).FirstOrDefault();
                    bool isEdit = UserOnline != null;
                    if (!isEdit)    //若沒有線上資訊，則新增。
                    {
                        UserOnline = new SYS_USER_ONLINE { FK_UserId = User.ID };
                    }
                    UserOnline.ConnectId = Context.ConnectionId;
                    UserOnline.OnlineDate = DateTime.Now;
                    UserOnline.IsOnline = true;
                    UserOnline.UserIP = Utils.GetIP();
                    UserOnlineManage.SaveOrUpdate(UserOnline, isEdit);

                    //獲取歷史消息
                    int days = Int32.Parse(System.Configuration.ConfigurationManager.AppSettings["HistoryDays"]);
                    DateTime dtHistory = DateTime.Now.AddDays(-days);
                    var ChatMessageList = ChatMessageManage.LoadAll(p => p.MessageDate > dtHistory);

                    //超級管理員
                    if (User.ID == ClsDic.DicRole["超級管理員"])
                    {
                        //通知用戶上線
                        Clients.All.UserLoginNotice("超級管理員：" + User.NAME + " 上線了!");

                        var HistoryMessage = ChatMessageList.OrderBy(p => p.MessageDate).ToList().Select(p => new
                        {
                            UserName = UserManage.Get(m => m.ID == p.FromUser).NAME,
                            UserFace = string.IsNullOrEmpty(UserManage.Get(m => m.ID == p.FromUser).FACE_IMG) ? "/Pro/Project/User_Default_Avatat?name=" + UserManage.Get(m => m.ID == p.FromUser).NAME.Substring(0, 1) : UserManage.Get(m => m.ID == p.FromUser).FACE_IMG,
                            MessageType = GetMessageType(p.MessageType),
                            p.FromUser,
                            p.MessageContent,
                            MessageDate = p.MessageDate.GetDateTimeFormats('D')[1].ToString() + " - " + p.MessageDate.ToString("HH:mm:ss"),
                            ConnectId = UserOnlineManage.LoadListAll(m => m.SYS_USER.ID == p.FromUser).FirstOrDefault().ConnectId
                        }).ToList();

                        //推送歷史消息
                        Clients.Client(Context.ConnectionId).addHistoryMessageToPage(JsonConverter.Serialize(HistoryMessage));
                    }
                    else
                    {
                        //獲取使用者一級部門資訊
                        var Depart = GetUserDepart(User.DPTID);
                        if (Depart != null && !string.IsNullOrEmpty(Depart.ID))
                        {
                            //添加用戶到部門群組 Groups.Add（用戶連接ID，群組）
                            Groups.Add(Context.ConnectionId, Depart.ID);
                            //通知用戶上線
                            Clients.All.UserLoginNotice(Depart.NAME + " - " + CodeManage.Get(m => m.CODEVALUE == User.LEVELS && m.CODETYPE == "ZW").NAMETEXT + "：" + User.NAME + " 上線了!");
                            //使用者歷史消息
                            int typeOfpublic = Common.Enums.ClsDic.DicMessageType["廣播"];
                            int typeOfgroup = Common.Enums.ClsDic.DicMessageType["群組"];
                            int typeOfprivate = Common.Enums.ClsDic.DicMessageType["私聊"];
                            var HistoryMessage = ChatMessageList.Where(p => p.MessageType == typeOfpublic || (p.MessageType == typeOfgroup && p.ToGroup == Depart.ID) || (p.MessageType == typeOfprivate && p.ToGroup == User.ID.ToString())).OrderBy(p => p.MessageDate).ToList().Select(p => new
                            {
                                UserName = UserManage.Get(m => m.ID == p.FromUser).NAME,
                                UserFace = string.IsNullOrEmpty(UserManage.Get(m => m.ID == p.FromUser).FACE_IMG) ? "/Pro/Project/User_Default_Avatat?name=" + UserManage.Get(m => m.ID == p.FromUser).NAME.Substring(0, 1) : UserManage.Get(m => m.ID == p.FromUser).FACE_IMG,
                                MessageType = GetMessageType(p.MessageType),
                                p.FromUser,
                                p.MessageContent,
                                MessageDate = p.MessageDate.GetDateTimeFormats('D')[1].ToString() + " - " + p.MessageDate.ToString("HH:mm:ss"),
                                ConnectId = UserOnlineManage.LoadListAll(m => m.SYS_USER.ID == p.FromUser).FirstOrDefault().ConnectId
                            }).ToList();

                            //推送歷史消息
                            Clients.Client(Context.ConnectionId).addHistoryMessageToPage(JsonConverter.Serialize(HistoryMessage));

                        }
                    }
                    //刷新用戶通訊錄
                    Clients.All.ContactsNotice(getUserOnlineJson(UserOnline));
                }
            }
            catch (Exception ex)
            {
                Clients.Client(Context.ConnectionId).UserLoginNotice("出錯了：" + ex.Message);
                throw ex.InnerException;
            }

        }


        /// <summary>
        /// 發送消息（廣播、組播）
        /// </summary>
        /// <param name="message">發送的消息</param>
        /// <param name="message">發送的群組</param>
        public void Send(string message, string groupId)
        {
            try
            {
                //消息使用者主體
                var UserOnline = UserOnlineManage.LoadListAll(p => p.ConnectId == Context.ConnectionId).FirstOrDefault();

                //廣播
                if (string.IsNullOrEmpty(groupId))
                {
                    //保存消息
                    ChatMessageManage.Save(new Domain.SYS_CHATMESSAGE() { FromUser = UserOnline.FK_UserId, MessageType = Common.Enums.ClsDic.DicMessageType["廣播"], MessageContent = message, MessageDate = DateTime.Now, MessageIP = Utils.GetIP() });
                    //返回消息實體
                    var Message = new Message() { ConnectId = UserOnline.ConnectId, UserName = UserOnline.SYS_USER.NAME, UserFace = string.IsNullOrEmpty(UserOnline.SYS_USER.FACE_IMG) ? "/Pro/Project/User_Default_Avatat?name=" + UserOnline.SYS_USER.NAME.Substring(0, 1) : UserOnline.SYS_USER.FACE_IMG, MessageDate = DateTime.Now.GetDateTimeFormats('D')[1].ToString() + " - " + DateTime.Now.ToString("HH:mm:ss"), MessageContent = message, MessageType = "public", UserId = UserOnline.SYS_USER.ID };

                    //推送消息
                    Clients.All.addNewMessageToPage(JsonConverter.Serialize(Message));
                }
                //組播
                else
                {
                    //保存消息
                    ChatMessageManage.Save(new Domain.SYS_CHATMESSAGE() { FromUser = UserOnline.FK_UserId, MessageType = Common.Enums.ClsDic.DicMessageType["群組"], MessageContent = message, MessageDate = DateTime.Now, MessageIP = Utils.GetIP(), ToGroup = groupId });
                    //返回消息實體
                    var Message = new Message() { ConnectId = UserOnline.ConnectId, UserName = UserOnline.SYS_USER.NAME, UserFace = string.IsNullOrEmpty(UserOnline.SYS_USER.FACE_IMG) ? "/Pro/Project/User_Default_Avatat?name=" + UserOnline.SYS_USER.NAME.Substring(0, 1) : UserOnline.SYS_USER.FACE_IMG, MessageDate = DateTime.Now.GetDateTimeFormats('D')[1].ToString() + " - " + DateTime.Now.ToString("HH:mm:ss"), MessageContent = message, MessageType = "group", UserId = UserOnline.SYS_USER.ID };

                    //推送消息
                    Clients.Group(groupId).addNewMessageToPage(JsonConverter.Serialize(Message));
                    //如果用戶不在群組中則單獨推送消息給使用者
                    var Depart = GetUserDepart(UserOnline.SYS_USER.DPTID);
                    if (Depart == null)
                    {
                        //推送給用戶
                        Clients.Client(Context.ConnectionId).addNewMessageToPage(JsonConverter.Serialize(Message));
                    }
                    else if (Depart.ID != groupId)
                    {
                        //推送給用戶
                        Clients.Client(Context.ConnectionId).addNewMessageToPage(JsonConverter.Serialize(Message));
                    }
                }
            }
            catch (Exception ex)
            {
                //推送系統消息
                Clients.Client(Context.ConnectionId).addSysMessageToPage("系統消息：消息發送失敗，請稍後再試！");
                throw ex.InnerException;
            }
        }


        /// <summary>
        /// 發送給指定用戶（單播）
        /// </summary>
        /// <param name="clientId">接收用戶的連接ID</param>
        /// <param name="message">發送的消息</param>
        public void SendSingle(string clientId, string message)
        {
            try
            {
                //接收用戶連接為空
                if (string.IsNullOrEmpty(clientId))
                {
                    //推送系統消息
                    Clients.Client(Context.ConnectionId).addSysMessageToPage("系統消息：用戶不存在！");
                }
                else
                {
                    //消息使用者主體
                    var UserOnline = UserOnlineManage.LoadListAll(p => p.ConnectId == Context.ConnectionId).FirstOrDefault();
                    //接收消息使用者主體
                    var ReceiveUser = UserOnlineManage.LoadListAll(p => p.ConnectId == clientId).FirstOrDefault();
                    if (ReceiveUser == null)
                    {
                        //推送系統消息
                        Clients.Client(Context.ConnectionId).addSysMessageToPage("系統消息：用戶不存在！");
                    }
                    else
                    {
                        //保存消息
                        ChatMessageManage.Save(new Domain.SYS_CHATMESSAGE() { FromUser = UserOnline.FK_UserId, MessageType = Common.Enums.ClsDic.DicMessageType["私聊"], MessageContent = message, MessageDate = DateTime.Now, MessageIP = Utils.GetIP(), ToGroup = UserOnline.SYS_USER.ID.ToString() });
                        //返回消息實體
                        var Message = new Message() { ConnectId = UserOnline.ConnectId, UserName = UserOnline.SYS_USER.NAME, UserFace = string.IsNullOrEmpty(UserOnline.SYS_USER.FACE_IMG) ? "/Pro/Project/User_Default_Avatat?name=" + UserOnline.SYS_USER.NAME.Substring(0, 1) : UserOnline.SYS_USER.FACE_IMG, MessageDate = DateTime.Now.GetDateTimeFormats('D')[1].ToString() + " - " + DateTime.Now.ToString("HH:mm:ss"), MessageContent = message, MessageType = "private", UserId = UserOnline.SYS_USER.ID };
                        if (ReceiveUser.IsOnline)
                        {
                            //推送給指定用戶
                            Clients.Client(clientId).addNewMessageToPage(JsonConverter.Serialize(Message));
                        }
                        //推送給用戶
                        Clients.Client(Context.ConnectionId).addNewMessageToPage(JsonConverter.Serialize(Message));

                    }
                }
            }
            catch (Exception ex)
            {
                //推送系統消息
                Clients.Client(Context.ConnectionId).addSysMessageToPage("系統消息：消息發送失敗，請稍後再試！");
                throw ex.InnerException;
            }
        }


        //使用者離線
        public override Task OnDisconnected(bool stopCalled)
        {
            //更新線上狀態
            var UserOnline = UserOnlineManage.LoadListAll(p => p.ConnectId == Context.ConnectionId).FirstOrDefault();
            UserOnline.ConnectId = Context.ConnectionId;
            UserOnline.OfflineDate = DateTime.Now;
            UserOnline.IsOnline = false;
            UserOnlineManage.Update(UserOnline);

            //獲取使用者資訊
            var User = UserManage.Get(p => p.ID == UserOnline.FK_UserId);

            Clients.All.UserLogOutNotice(User.NAME + "：離線了!");
            //刷新用戶通訊錄
            Clients.All.ContactsNotice(getUserOnlineJson(UserOnline));

            return base.OnDisconnected(true);
        }

        private string getUserOnlineJson(SYS_USER_ONLINE user)
        {
            var ret = new
            {
                IsOnline = user.IsOnline,
                FK_UserId = user.FK_UserId,
                ConnectId = user.ConnectId,
            };
            return JsonConverter.Serialize(ret);
        }


        private string GetMessageType(int type)
        {
            if (type == ClsDic.DicMessageType["廣播"])
            {
                return "public";
            }
            if (type == ClsDic.DicMessageType["群組"])
            {
                return "group";
            }
            return "private";
        }

        private SYS_DEPARTMENT GetUserDepart(string departId)
        {
            SYS_DEPARTMENT depart = this.DepartmentManage.Get(p => p.ID == departId);
            if (depart != null)
            {
                string ParentId = depart.PARENTID;
                var depart2 = new SYS_DEPARTMENT();
                for (int? num = depart.BUSINESSLEVEL; num >= 1; num--) //迴圈查找子部門
                {
                    depart2 = this.DepartmentManage.Get(p => p.ID == ParentId);
                    if (string.IsNullOrEmpty(depart2?.PARENTID))
                    {
                        break;
                    }
                    ParentId = depart2.PARENTID;
                }
                return depart2;
            }
            return null;
        }
    }
}
