using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace DMail.Handlers
{
    internal class BaseHandler
    {
        protected TcpClient Client { get; set; }

        private readonly StreamReader reader;

        public BaseHandler(TcpClient client)
        {
            Client = client;
            reader = new(client.GetStream());
        }

        public async Task WriteLine(string line)
        {
            await Write(line + "\r\n");
        }

        public async Task Write(string text)
        {
            await Client.GetStream().WriteAsync(Encoding.ASCII.GetBytes(text));
        }

        public int Available()
        {
            return Client.Available;
        }

        public async Task<string> ReadLine()
        {
            while(Client.Available < 1)
            {
                await Task.Delay(1);
            }

            return await reader.ReadLineAsync() ?? "";
        }

        public void Dispose()
        {
            reader.Dispose();
        }
    }
}
