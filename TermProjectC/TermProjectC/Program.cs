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
            Server serv = null;
            serv = new Server(inp_port);        
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
            while (true)
            {
                client.SendMessage(Console.ReadLine());
                string debug = serv.Debug();
            }
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
        public string Debug() { return str; }
        public Server(int port)
        {
            listener = new TcpListener(IPAddress.Any, port);
            client = new TcpClient();
            buffer = new byte [1024];
            str = "";
           
        }
        public void Handling()
        {            
            while (true)
            {
                listener.Start();
                client = listener.AcceptTcpClient();
                client.GetStream().Read(buffer, 0, 1024);
                str = Encoding.ASCII.GetString(buffer);
                Console.WriteLine("Stranger Wrote:");
                Console.WriteLine(str);
                //Console.WriteLine("Client connected with IP {0}", client.Client.AddressFamily.);
                //Console.WriteLine(((IPEndPoint)client.Client.RemoteEndPoint).Address.ToString());
                input_ip = ((IPEndPoint)client.Client.RemoteEndPoint).Address;
                client.Close();
                listener.Stop();
            }
        }
        public void HandlingWithoutClosing()
        {
             listener.Start();
             //client = listener.AcceptTcpClient();
             while (!listener.Pending());
             Console.WriteLine("Connection set up");
             client = listener.AcceptTcpClient();
             while (true)
             {               
                client.GetStream().Read(buffer, 0, 1024);
                str = Encoding.ASCII.GetString(buffer);
                str.Replace("\n","");
                Console.WriteLine("Stranger wrote:" + str);
                //Console.WriteLine("Client connected with IP {0}", client.Client.AddressFamily.);
                //Console.WriteLine(((IPEndPoint)client.Client.RemoteEndPoint).Address.ToString());
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
        }
        public void SendMessage(string text)
        {
            byte[] buffer = new byte[1024];
            buffer = Encoding.ASCII.GetBytes(text);
            client.GetStream().Write(buffer, 0, buffer.Length);
        }
        public void Connect(IPAddress ip)
        {
            inp_ip = ip;
            while (!client.Connected)
            {
                try
                {
                    client.Connect(inp_ip, port);
                    Console.WriteLine("Connection Attempt succeeded");
                }
                catch
                {
                    Console.WriteLine("Connection Attempt failed");
                }
            }
        }
    }
}
