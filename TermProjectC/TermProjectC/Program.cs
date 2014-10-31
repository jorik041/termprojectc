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
    class Program
    {
        static void Main(string[] args)
        {
            new Server(8080);
        }
    }
    class Server
    {
        TcpListener listener;
        TcpClient client;
        public Server(int port)
        {
            listener = new TcpListener(IPAddress.Any, port);
            client = new TcpClient();
            byte[] buffer = new byte [1024];
            string str;
            while (true)
            {
                // Принимаем новых клиентов
                listener.Start();
                client = listener.AcceptTcpClient();
                client.GetStream().Read(buffer,0,1024);                
                str = Encoding.ASCII.GetString(buffer);
                Console.WriteLine(str);
                //Console.WriteLine("Client connected with IP {0}", client.Client.AddressFamily.);
                Console.WriteLine(((IPEndPoint)client.Client.RemoteEndPoint).Address.ToString());
                client.Close();
                listener.Stop();
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
    /*class Client
    {


    }*/
}
