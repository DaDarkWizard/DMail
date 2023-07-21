using DMail.Handlers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace DMail
{
    internal class ClientHandler
    {
        public TcpClient _client;
        public StreamReader reader;

        public ClientHandler(TcpClient client)
        {
            _client = client;
            reader = new StreamReader(_client.GetStream());
        }

        public async Task HandleClient()
        {
            try
            {

                Console.WriteLine("Handling client");

                var data = Encoding.ASCII.GetBytes("220 smtp.dmasker.net Simple Mail Transfer Service Ready\r\n");
                await _client.GetStream().WriteAsync(data);

                while (_client.Available < 1)
                {
                    if (!_client.Connected) return;
                    await Task.Delay(1);
                }

                var reply = await reader.ReadLineAsync();

                if (reply?.StartsWith("HELO") ?? false)
                {
                    var handler = new HeloHandler(_client, reply.Substring(5));
                    await handler.HandleClient();
                    handler.Dispose();
                    return;
                }
                else
                {
                    await _client.GetStream().WriteAsync(Encoding.ASCII.GetBytes("500 command not recognized\r\n"));
                    return;
                }
            }
            finally
            {
                _client.Dispose();
                reader.Dispose();
            }
        }
    }
}
