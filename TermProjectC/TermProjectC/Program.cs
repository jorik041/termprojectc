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
        IPAddress input_ip = IPAddress.Parse("127.0.0.1");
        static void Main(string[] args)
        {
            /*Console.WriteLine("Enter input_port (Press enter to use default)");
            string user_input = Console.ReadLine();
            int inp_port = 8080,
                out_port = 8081;
            Server serv = null;
            if (user_input != "")
            {
                inp_port = Convert.ToInt32(user_input);
                Console.WriteLine("Enter output_port");
                out_port = Convert.ToInt32(Console.ReadLine());
            }
            while (serv == null)
            {
                try
                {
                    serv = new Server(inp_port);
                }
                catch
                {
                    serv = null;
                    Console.WriteLine("A mistake has been caught. Probably, Input Port is invalid so you should enter new input and output ports. Enter new inp_port: ");
                    inp_port = Convert.ToInt32(Console.ReadLine());
                    Console.WriteLine("Enter new out_port: ");
                    out_port = Convert.ToInt32(Console.ReadLine());
                }
            }
            Thread Thread = new Thread(new ThreadStart(serv.Handling));
            Thread.Start();
            Client client;
            if (serv.GetIp() != null)
            {
                client = new Client(serv.GetIp(), out_port);
            }
            else
            {
                client = new Client(out_port);
            }
            while (true)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Thread.Sleep(4);
                Console.Write("You: ");
                Console.ForegroundColor = ConsoleColor.White;
                string answer = Console.ReadLine();
                if (answer == "::quit")
                {
                    break;
                }
                client.SendMessage(answer);
            }
            Thread.Abort();
            return;
             */
            Chat chat = new Chat();
            chat.PrimaryHandling();
        }
    }
    
}
