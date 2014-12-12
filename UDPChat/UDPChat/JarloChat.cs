using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace JarlooChat
{
    class Chat
    {
        private static string user_last_message;
        public void Send(string data)
        {
            UdpClient udpclient = new UdpClient();

            IPAddress multicastaddress = IPAddress.Parse("239.0.0.222");
            udpclient.JoinMulticastGroup(multicastaddress);
            IPEndPoint remoteep = new IPEndPoint(multicastaddress, 2222);

            Byte[] buffer = null;
            Byte[] tech_buffer = null;
            
            string tech_data = "Username";
            
            buffer = Encoding.Unicode.GetBytes(data);
            tech_buffer = Encoding.Unicode.GetBytes(tech_data);

            udpclient.Send(tech_buffer, tech_buffer.Length, remoteep);
            udpclient.Send(buffer, buffer.Length, remoteep);
            
            user_last_message = data;
           
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
                Byte[] tech_data = client.Receive(ref localEp);
                string string_tech_data = Encoding.Unicode.GetString(tech_data);
                Byte[] data = client.Receive(ref localEp);
                string strData = Encoding.Unicode.GetString(data);
                if (strData != user_last_message)
                {
                    Console.Write("{0}Anonymous: ", string_tech_data);
                    Console.WriteLine(strData);
                }
            }
        }
    }
}
