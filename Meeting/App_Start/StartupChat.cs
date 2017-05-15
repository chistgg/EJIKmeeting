using Microsoft.Owin;
using Owin;

[assembly: OwinStartup(typeof(Meeting.SignalRChat.StartupChat))]
namespace Meeting.SignalRChat
{
    public class StartupChat
    {
        public void Configuration(IAppBuilder app)
         {
            app.MapSignalR();
        }
    }
}
