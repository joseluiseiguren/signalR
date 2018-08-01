using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using Microsoft.Owin;
using Microsoft.Owin.Cors;
using Microsoft.Owin.Hosting;
using Owin;
using signalR;
using System;
using System.Threading.Tasks;

[assembly: OwinStartup(typeof(Program.Startup))]
namespace signalR
{
    class Program
    {
        static void Main(string[] args)
        {
            string url = "http://+:43666";

            using (WebApp.Start(url))
            {
                Console.WriteLine("Server running on {0}", url);
                string texto;
                while ((texto = Console.ReadLine()) != "exit")
                {
                    var hub = GlobalHost.ConnectionManager.GetHubContext<MyHub>();
                    hub.Clients.All.newMessage(texto);
                }
            }
        }

        public class Startup
        {
            public void Configuration(IAppBuilder app)
            {
                app.Map("/signalr", map =>
                {
                    map.UseCors(CorsOptions.AllowAll);

                    var hubConfig = new HubConfiguration
                    {
                        EnableDetailedErrors = true,
                        EnableJSONP = true
                    };
                    map.RunSignalR(hubConfig);
                });
            }
        }

        [HubName("MyHub")]
        public class MyHub : Hub
        {
            public override Task OnConnected()
            {
                Console.WriteLine("Nueva Cliente: {0}", this.Context.ConnectionId.ToString());
                return base.OnConnected();
            }
        }
    }
}
