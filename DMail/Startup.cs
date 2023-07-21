using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.ServiceProcess;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;

namespace DMail
{
    public class Startup
    {
        public static string DomainName = "dmasker.net";

        private List<Task> ClientTasks = new();

        public Startup()
        {
            
        }

        public async Task Run()
        {
            var ipEndPoint = new IPEndPoint(IPAddress.Any, 25);
            TcpListener listener = new(ipEndPoint);

            try
            {
                listener.Start();




                while (true)
                {
                    TcpClient client = await listener.AcceptTcpClientAsync();

                    foreach (var task in ClientTasks.ToList())
                    {
                        if(task.IsCompleted)
                        {
                            ClientTasks.Remove(task);
                        }
                    }

                    var handler = new ClientHandler(client);
                    ClientTasks.Add(handler.HandleClient());
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.StackTrace);
            }
            finally
            {
                listener.Stop();
            }
        }
    }
}
