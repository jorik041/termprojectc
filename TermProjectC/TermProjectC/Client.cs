using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.Net.NetworkInformation;


namespace TermProjectC
{
    class Client
    {
        private static IPAddress inp_ip;
        private static TcpClient client;
        private static int port;
        public Client(IPAddress ip, int por)
        {
            inp_ip = ip;
            client = new TcpClient();
            port = por;
        }
        public Client(int por)
        {
            client = new TcpClient();
            port = por;
            inp_ip = IPAddress.Loopback;
        }

        public void SendMessage(string text)
        {
            Connect(inp_ip);
            //client.Connect(inp_ip, port);
            byte[] buffer = new byte[1024];
            text += '\0';
            buffer = Encoding.UTF32.GetBytes(text);
            client.GetStream().Write(buffer, 0, buffer.Length);
            client.GetStream().Dispose();
            client.Close();
            client = new TcpClient();
        }
        public void Connect(IPAddress ip)
        {
            inp_ip = ip;
            // client.Connect(inp_ip, port);
            while (!client.Connected)
            {
                try
                {
                    client.Connect(inp_ip, port);
                    //Console.WriteLine("Connection Attempt succeeded");
                }
                catch
                {
                    Console.WriteLine("Connection Attempt failed");
                }
            }
        }
        ~Client()
        {
            client.Close();
        }
    }
}
