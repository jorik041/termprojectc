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
        public void Handling()
        {
           // listener.Start();
            while (true)
            {                
                //while (!listener.Pending());
                client = listener.AcceptTcpClient();
                client.GetStream().Read(buffer, 0, 1024);
                str = Encoding.UTF32.GetString(buffer);
                Console.ForegroundColor = ConsoleColor.Red;
                input_ip = ((IPEndPoint)client.Client.RemoteEndPoint).Address;
                Console.Write("\n({0}:{1})Stranger wrote:",input_ip,((IPEndPoint)client.Client.RemoteEndPoint).Port);
                Console.ForegroundColor = ConsoleColor.White;
                int i = 0;  
                while (str[i] != '\0')
                {
                    Console.Write(str[i++]);
                }
                Console.ForegroundColor = ConsoleColor.Red;
                Console.Write("\nYou:");
                Console.ForegroundColor = ConsoleColor.White;
                str = "";
                buffer = new byte[1024];
                client.GetStream().Dispose();
                client.Close();
                client = new TcpClient();
               
            }
        }
        public void HandlingWithoutClosing()
        {
            listener.Start();
            //client = listener.AcceptTcpClient();
            while (!listener.Pending()) ;
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
                Console.ForegroundColor = ConsoleColor.White;
                str = "";
                buffer = new byte[1024];
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
}
