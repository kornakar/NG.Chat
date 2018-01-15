using ChatTest234789234234789Service.Hubs;
using Microsoft.AspNet.SignalR;
using Microsoft.Owin;
using Owin;
using Unity;

[assembly: OwinStartup(typeof(ChatTest234789234234789Service.Startup))]

namespace ChatTest234789234234789Service
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureMobileApp(app);

            var hubConfiguration = new HubConfiguration
                                   {
                                           EnableDetailedErrors = true
                                   };

            app.MapSignalR(hubConfiguration);
        }
    }
}