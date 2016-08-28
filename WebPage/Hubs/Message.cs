using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebPage.Hubs
{
    public class Message
    {
        public int UserId
        {
            get;
            set;
        }

        public string ConnectId
        {
            get;
            set;
        }

        public string UserName
        {
            get;
            set;
        }

        public string UserFace
        {
            get;
            set;
        }

        public string MessageDate
        {
            get;
            set;
        }

        public string MessageType
        {
            get;
            set;
        }

        public string MessageContent
        {
            get;
            set;
        }
    }
}