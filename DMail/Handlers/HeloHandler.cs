using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace DMail.Handlers
{
    internal class HeloHandler : BaseHandler
    {
        private string domain;

        private StringBuilder ReversePath { get; set; } = new();
        private StringBuilder ForwardPath { get; set; } = new();
        private StringBuilder Mail { get; set; } = new();

        public HeloHandler(TcpClient client, string domain) : base(client)
        {
            this.domain = domain;
        }

        public async Task HandleClient()
        {
            await WriteLine("250 smtp.dmasker.net");
            bool done = false;

            while(!done)
            {
                string input = await ReadLine();

                string command = input;
                string text = "";
                if (input.Contains(' '))
                {
                    command = input.Split(' ')[0];

                    text = input.Substring(input.IndexOf(' '));
                }


                command = command.ToUpper(CultureInfo.InvariantCulture);

                switch (command)
                {
                    case "RSET":
                        ResetBuffers();
                        await WriteLine("250 OK");
                        break;
                    case "NOOP":
                        await WriteLine("250 OK");
                        break;
                    case "QUIT":
                        await WriteLine("221 smpt.dmasker.net Service closing transmission channel");
                        done = true;
                        break;
                    case "MAIL":
                        if(string.IsNullOrWhiteSpace(text))
                        {
                            await WriteLine("501 Syntax error in parameters or arguments");
                            break;
                        }
                        ResetBuffers();
                        ReversePath.Append(text);
                        await WriteLine("250 OK");
                        break;
                    case "RCPT":
                        if(string.IsNullOrWhiteSpace(text))
                        {
                            await WriteLine("501 Syntax error in parameters or arguments");
                            break;
                        }
                        ForwardPath.Append(text).Append("\r\n");
                        await WriteLine("250 OK");
                        break;
                    case "DATA":
                        {
                            await WriteLine("354 Start mail input; end with <CRLF>.<CRLF>");
                            var reading = true;
                            while(reading)
                            {
                                text = await ReadLine();
                                if(text == ".")
                                {
                                    reading = false;
                                }
                                else
                                {
                                    if(text.StartsWith("."))
                                    {
                                        text = text.Substring(1);
                                    }
                                }
                                Mail.Append(text).Append("\r\n");
                            }
                            await WriteLine("250 OK");
                            break;
                        }

                    default:
                        await WriteLine("500 command not recognized");
                        break;
                }
            }
        }

        private void ResetBuffers()
        {
            ReversePath.Clear();
            ForwardPath.Clear();
            Mail.Clear();
        }
    }
}
