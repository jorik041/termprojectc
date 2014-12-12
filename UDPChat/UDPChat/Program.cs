using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using JarlooChat;

namespace UDPChat
{
    class Program
    {
        static void Main(string[] args)
        {
            Chat chat = new Chat();
            string decision = Console.ReadLine();
            Thread ListenThread = new Thread(new ThreadStart(chat.Listen));
            ListenThread.Start();
            while (true)
            {
                chat.Send(Console.ReadLine());
            }
        }
    }
}
