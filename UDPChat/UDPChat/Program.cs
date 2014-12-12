using System;
using System.Collections.Generic;
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
			Console.Write ("Enter ur nick: ");
            Chat chat = new Chat(Console.ReadLine ());
            // string decision = Console.ReadLine();
            Thread ListenThread = new Thread(new ThreadStart(chat.Listen));
            ListenThread.Start();
            while (true)
            {
                chat.Send(Console.ReadLine());
            }
        }
    }
}
