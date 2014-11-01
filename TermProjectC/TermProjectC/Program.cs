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
            while (serv == null)
            {
                try
                {
                    serv = new Server(inp_port);
                }
                catch
                {
                    serv = null;
                    Console.WriteLine("Input Port is invalid. Enter new inp_port: ");
                    inp_port = Convert.ToInt32(Console.ReadLine());
                    Console.WriteLine("Input Port is invalid. Enter new out_port: ");
                    out_port = Convert.ToInt32(Console.ReadLine());
                }
            }
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
                Console.ForegroundColor = ConsoleColor.Red;
                Thread.Sleep(4);
                Console.Write("You: ");
                Console.ForegroundColor = ConsoleColor.White;
                string answer = Console.ReadLine();
                client.SendMessage(answer);
            }
            Thread.Abort();
            return;
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
            listener.Start();
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
                str = Encoding.UTF32.GetString(buffer);
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
             //listener.Start();
             //client = listener.AcceptTcpClient();
             while (!listener.Pending());
             Console.WriteLine("Connection set up");
             client = listener.AcceptTcpClient();
             while (true)
             {
                 try
                 {
                     client.GetStream().Read(buffer, 0, 1024);
                     input_ip = ((IPEndPoint)client.Client.RemoteEndPoint).Address;
                 }
                 catch
                 {
                     Console.WriteLine("Connection Lost\n");
                     Thread.Sleep(1000);
                 }
                str = Encoding.UTF32.GetString(buffer);
                //Console.WriteLine("Stranger wrote:" + str);
                Console.ForegroundColor = ConsoleColor.Red;
                Console.Write("\nStranger wrote:");
                Console.ForegroundColor = ConsoleColor.White;
                int i = 0;
                 while (str[i] != '\0')
                 {
                    Console.Write(str[i++]);
                 }
                 Console.ForegroundColor = ConsoleColor.Red;
                 Console.Write("\nYou:");
                 str = "";
                buffer = new byte [1024];
                //Console.WriteLine("Client connected with IP {0}", client.Client.AddressFamily.);
                //Console.WriteLine(((IPEndPoint)client.Client.RemoteEndPoint).Address.ToString());
                
                
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
            text += '\0';
            buffer = Encoding.UTF32.GetBytes(text);
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
        ~Client()
        {
            client.Close();
        }
    }
}
