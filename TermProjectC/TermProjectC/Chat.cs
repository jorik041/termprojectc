using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace TermProjectC
{
      class Chat
    {
        Client client;
        Server serv;
        Thread server_thread;
        int out_port;
        protected void ServerSetUp()
        {
            Console.WriteLine("Enter input_port (Press enter to use default)");
            string user_input = Console.ReadLine();
            int inp_port = 8080;
                out_port = 8081;
            serv = null;
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
        }
        protected void SendMessage()
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Thread.Sleep(4);
            Console.Write("You: ");
            Console.ForegroundColor = ConsoleColor.White;
            string answer = Console.ReadLine();
            if (answer == "::quit")
            {
            }
            client.SendMessage(answer);
        }
        protected void ClientSetUp()
        {
            client = new Client(out_port);
        }
        public void PrimaryHandling()
        {
            //ServerSetUp();
            //ClientSetUp();
            server_thread = new Thread(new ThreadStart(serv.Handling));
            server_thread.Start();
            while (true)
            {
                SendMessage();
            }
        }
        public Chat()
        {
            ServerSetUp();
            ClientSetUp();
        }
        ~Chat()
        {
            server_thread.Abort();
        }
      }
}
