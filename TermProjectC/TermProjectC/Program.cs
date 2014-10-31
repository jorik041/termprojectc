using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.Net.NetworkInformation;
using System.Threading;



namespace TermProjectC
{
    class Program
    {
        IPAddress input_ip;
        static void Main(string[] args)
        {
            int inp_port = Convert.ToInt32(Console.ReadLine());
            int out_port = Convert.ToInt32(Console.ReadLine());
            Server serv = new Server(inp_port);
            Thread Thread = new Thread(new ThreadStart(serv.HandlingWithoutClosing));
            Thread.Start();
            Client client;
            if (serv.GetIp() != null)
            {
                client = new Client(serv.GetIp(), out_port);
            }
            else
            {
                client = new Client(out_port);
                client.Connect(IPAddress.Loopback);
            }
            client.SendMessage(Console.ReadLine());
        }
    }
    class Server
    {
        private static TcpListener listener;
        private static TcpClient client;
        private static IPAddress input_ip;
        public IPAddress GetIp() { return input_ip; }
        private static byte[] buffer;
        private static string str;
        public TcpClient GetClient() { return client; }
        public Server(int port)
        {
            listener = new TcpListener(IPAddress.Any, port);
            client = new TcpClient();
            buffer = new byte [1024];
           
        }
        public void Handling()
        {            
            while (true)
            {
                listener.Start();
                client = listener.AcceptTcpClient();
                client.GetStream().Read(buffer, 0, 1024);
                str = Encoding.ASCII.GetString(buffer);
                Console.WriteLine(str);
                //Console.WriteLine("Client connected with IP {0}", client.Client.AddressFamily.);
                Console.WriteLine(((IPEndPoint)client.Client.RemoteEndPoint).Address.ToString());
                input_ip = ((IPEndPoint)client.Client.RemoteEndPoint).Address;
                client.Close();
                listener.Stop();
            }
        }
        public void HandlingWithoutClosing()
        {
             listener.Start();
             client = listener.AcceptTcpClient();
             while (true)
             {
                client.GetStream().Read(buffer, 0, 1024);
                str = Encoding.ASCII.GetString(buffer);
                Console.WriteLine(str);
                //Console.WriteLine("Client connected with IP {0}", client.Client.AddressFamily.);
                Console.WriteLine(((IPEndPoint)client.Client.RemoteEndPoint).Address.ToString());
                input_ip = ((IPEndPoint)client.Client.RemoteEndPoint).Address;
             }
        }
        
        ~Server()
        {
            if (listener != null)
            {
                listener.Stop();
            }
        }

    }
    class Client
    {
        private static IPAddress inp_ip;
        TcpClient client;
        int port;
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
        }
        public void SendMessage(string text)
        {
            client.Connect(inp_ip, port);
            byte[] buffer = new byte[1024];
            buffer = Encoding.ASCII.GetBytes(text);
            client.GetStream().Write(buffer, 0, buffer.Length);
        }
        public void Connect(IPAddress ip)
        {
            inp_ip = ip;
        }
    }
}
