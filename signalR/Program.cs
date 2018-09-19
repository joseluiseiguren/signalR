using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using Microsoft.Owin;
using Microsoft.Owin.Cors;
using Microsoft.Owin.Hosting;
using Owin;
using signalR;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

[assembly: OwinStartup(typeof(Program.Startup))]
namespace signalR
{
    class Program
    {
        //lista de clientes conectados
        public static List<string> _clientsConnected = new List<string>();

        const string GROUPNAME_PARES = "pares";
        const string GROUPNAME_IMPARES = "impares";

        static void Main(string[] args)
        {            
            var hubContext = GlobalHost.ConnectionManager.GetHubContext<MyHub>();

            string url = "http://+:43666";

            using (WebApp.Start(url))
            {
                Console.WriteLine("Server running on {0}", url);
                string texto;
                while ((texto = Console.ReadLine()) != "exit")
                {
                    //no conected clients
                    if (_clientsConnected.Count == 0)
                    {
                        Console.WriteLine("No hay clientes conectados");
                        continue;
                    }

                    //le quiere enviar el mensaje a todos los clientes conectados
                    if (texto.StartsWith("all:", StringComparison.CurrentCultureIgnoreCase))
                    {
                        hubContext.Clients.All.newMessage(texto);
                    }
                    //le quiere enviar el mensaje a los miembros del grupo "pares"
                    else if (texto.StartsWith("pares:", StringComparison.CurrentCultureIgnoreCase))
                    {
                        hubContext.Clients.Group(GROUPNAME_PARES).newMessage(texto);
                    }
                    //le quiere enviar el mensaje a los miembros del grupo "impares"
                    else if (texto.StartsWith("impares:", StringComparison.CurrentCultureIgnoreCase))
                    {
                        hubContext.Clients.Group(GROUPNAME_IMPARES).newMessage(texto);
                    }
                    //le quiere enviar el mensaje a un cliente en particular, dentro de la lista de clietnes conectados. Es el index de la lista
                    else
                    {
                        try
                        {
                            int index;
                            if (!int.TryParse(texto.Substring(0, texto.IndexOf(':')), out index))
                            {
                                Console.WriteLine("Invalid Command!");
                                continue;
                            }

                            if (_clientsConnected.Count < index)
                            {
                                Console.WriteLine("Cliente Inexistente!");
                                continue;
                            }

                            //get client/conection id
                            var conectionId = _clientsConnected[index-1];

                            //send the message
                            hubContext.Clients.Client(conectionId).newMessage(texto);
                        }
                        catch (Exception)
                        {
                            Console.WriteLine("Invalid Command!");
                            continue;
                        }                        
                    }                    
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

        //[Authorize(Users = "all")]
        [HubName("MyHub")]
        public class MyHub : Hub
        {
            //crea una nueva instancia por cada conexion/desconexion/reconexion
            public MyHub()
            {
                Console.WriteLine("New instance MyHub...");
            }

            public override Task OnConnected()
            {
                //get user name
                var usuario = this.Context.QueryString["usuario"];

                Console.WriteLine("Conexion: {0}, {1}", usuario, this.Context.ConnectionId.ToString());

                //save client id to connected clients list
                _clientsConnected.Add(this.Context.ConnectionId);
                Console.WriteLine("Clients Connected: {0}", _clientsConnected.Count);
                
                //agrego la conexion a un grupo 
                this.AddToGroup(this.Context.ConnectionId);

                return base.OnConnected();
            }

            public override Task OnDisconnected(bool stopCalled)
            {
                Console.WriteLine("Desconexion: {0}, {1}", this.Context.ConnectionId.ToString(), stopCalled);

                //remove client id from connected clients list
                _clientsConnected.Remove(Context.ConnectionId);
                Console.WriteLine("Clients Connected: {0}", _clientsConnected.Count);

                return base.OnDisconnected(stopCalled);
            }

            public override Task OnReconnected()
            {
                Console.WriteLine("Reconected: {0}", this.Context.ConnectionId.ToString());

                //save client id to connected clients list
                _clientsConnected.Add(this.Context.ConnectionId);
                Console.WriteLine("Clients Connected: {0}", _clientsConnected.Count);
                
                //agrego la conexion a un grupo 
                this.AddToGroup(this.Context.ConnectionId);

                return base.OnReconnected();
            }

            public void AddToGroup(string conectionId)
            {
                if (_clientsConnected.Count % 2 == 0)
                {
                    this.Groups.Add(conectionId, GROUPNAME_PARES);
                }
                else
                {
                    this.Groups.Add(conectionId, GROUPNAME_IMPARES);
                }
            }
        }
    }
}
