﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

using System.Net;
using System.Net.Sockets;

namespace JarlooChat
{
    class Chat
    {
		private readonly string username;

		public const char separator = (char)0x2; // Start Of Text
		public const char messageHead = (char)0x10;
		public const char firstMeetHead = (char)0x11;
		public const char passwordHead = (char)0x12;

		public Chat (string nameOfUser)
		{
			username = nameOfUser;
		}

		public void digest (string data)
		{
			char head = data [0];
			int separatorIndex = data.IndexOf (separator);
			string tech_data = data.Substring (1, separatorIndex - 1);
			string use_data = data.Substring (separatorIndex + 1);

			Console.WriteLine ("tech_data: " + tech_data);
			Console.WriteLine ("use_data: " + use_data);
		}

        public void Send (string data, char head = messageHead)
        {
            UdpClient udpclient = new UdpClient();

            IPAddress multicastaddress = IPAddress.Parse("239.0.0.222");
            udpclient.JoinMulticastGroup(multicastaddress);
            IPEndPoint remoteep = new IPEndPoint(multicastaddress, 2222);

			string tech_data = "";

			if (head == messageHead)
				tech_data = username;

			Byte[] buffer = Encoding.Unicode.GetBytes(head + tech_data + separator + data);

            udpclient.Send(buffer, buffer.Length, remoteep);
        }

        public void Listen()
        {
            UdpClient client = new UdpClient();

            client.ExclusiveAddressUse = false;
            IPEndPoint localEp = new IPEndPoint(IPAddress.Any, 2222);

            client.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
            client.ExclusiveAddressUse = false;

            client.Client.Bind(localEp);
            
            IPAddress multicastaddress = IPAddress.Parse("239.0.0.222");
            client.JoinMulticastGroup(multicastaddress);

            Console.WriteLine("Listening this will never quit so you will need to ctrl-c it");

            while (true)
            {
                Byte[] data = client.Receive(ref localEp);
                string strData = Encoding.Unicode.GetString(data);

				digest (strData);
            }
        }
    }
}
