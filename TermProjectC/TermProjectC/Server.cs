using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Net.NetworkInformation;
using System.Threading;

namespace TermProjectC
{
    class Server
    {
        private static TcpListener listener;
        private static TcpClient client;
        private static IPAddress input_ip;
        public IPAddress GetIp() { return input_ip; }
        private static byte[] buffer;
        private static string str;
        private static Dictionary<IPAddress, string> nicknames_dict;
        public TcpClient GetClient() { return client; }

        public Server(int port)
        {
            listener = null;
            listener = new TcpListener(IPAddress.Any, port);
            listener.Start();
            client = new TcpClient();
            buffer = new byte[1024];
            str = "";
            nicknames_dict = new System.Collections.Generic.Dictionary<IPAddress, string>();

        }

        protected string WriteBufferToString()
        {
            string answer = Encoding.UTF32.GetString(buffer);            
            for (int i = 0; i < answer.Length; i++ )
            {
                if (answer[i] != '\0')
                {
                    str += answer[i];
                }

            }
            buffer = new byte[1024];
            return answer;            
        }


        public string GetString()
        {
            string answer = str;
            str = "";
            return answer;
        }

        public void Handling()
        {
           // listener.Start();
            while (true)
            {                
                //while (!listener.Pending());
                client = listener.AcceptTcpClient();
                client.GetStream().Read(buffer, 0, 1024);
                WriteBufferToString();
                IPAddress inp_ip = (((IPEndPoint)client.Client.RemoteEndPoint).Address);
                if (!nicknames_dict.ContainsKey(inp_ip))
                {
                    NewUserEnter(inp_ip);
                }
                client.GetStream().Dispose();
                client.Close();
                client = new TcpClient();
               
            }
        }

        protected void NewUserEnter(IPAddress inp_ip)
        {
            
            int port = (((IPEndPoint)client.Client.RemoteEndPoint).Port);
            string data = "Enter your nickname \0";
            byte[] buffer = Encoding.UTF32.GetBytes(data);
            client.GetStream().Dispose();
            client.Close();
            client = new TcpClient();
            client.Connect(inp_ip,port);
            client.GetStream().Write(buffer,0,buffer.Length);
            client.GetStream().Dispose();
            client.Close();
            buffer = new byte [1024];
            client = new TcpClient();
            client.Connect(inp_ip,port);
            client.GetStream().Read(buffer,0,buffer.Length);
            client.GetStream().Dispose();
            client.Close();
            data = Encoding.UTF32.GetString(buffer);
            nicknames_dict.Add(inp_ip, data);
        }

        ~Server()
        {
            if (listener != null)
            {
                listener.Stop();
            }
        }

    }
}
