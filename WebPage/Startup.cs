using System;
using System.Threading.Tasks;
using Microsoft.Owin;
using Owin;

[assembly: OwinStartup(typeof(WebPage.Startup))]

namespace WebPage
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            // 有關如何配置應用程式的詳細資訊，請訪問 http://go.microsoft.com/fwlink/?LinkID=316888
            app.MapSignalR();
        }
    }
}
