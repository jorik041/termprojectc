using System;
using System.Collections.Generic;
using System.Text;

using System.Security.Cryptography;

using System.Net;
using System.Net.Sockets;

namespace JarlooChat
{
    class Chat
    {
		private readonly string username;
		private bool firstMeet;

		private List<string> WaitForConnect;

		public const char separator = (char)0x2; // Start Of Text

		public const char dissconnectHead = (char)0x4; // EOT - End Of Transmition
		public const char messageHead = (char)0x10;
		public const char firstMeetHead = (char)0x11;
		public const char passwordHead = (char)0x12;

		public Chat (string nameOfUser)
		{
			username = nameOfUser;

			WaitForConnect = new List<string> ();

			rsa = new RSACryptoServiceProvider ();
			des = new DESCryptoServiceProvider ();

			des.GenerateKey (); des.GenerateIV ();

			cryptor = des.CreateEncryptor ();
			decryptor = des.CreateDecryptor ();

			firstMeet = true;

			byte[] keyToTransport = rsa.ExportParameters (false).Modulus;

			byte[] buffer = {0};
			Array.Resize (ref buffer, keyToTransport.Length);
			Array.Copy (keyToTransport, buffer, keyToTransport.Length);

			Send (buffer, firstMeetHead);
		}

		~Chat()
		{
			Send("", dissconnectHead);
			rsa.Clear ();
			des.Clear ();
		}

		private char digest (string data)
		{
			char head = data [0];
			int separatorIndex = data.IndexOf (separator);
			string nickname = data.Substring (1, separatorIndex - 1); // [1:sepIndex-1]
			string use_data = data.Substring (separatorIndex + 1); // [sepIndex+1:]

			if (nickname != username) {
				switch (head) {
				case messageHead:
					Console.WriteLine ("\tmsg from " + nickname);
					use_data = decrypt (use_data);
					Console.WriteLine (nickname + ": " + use_data);

					break;
			
				case firstMeetHead:
					Console.WriteLine ("\tfirst meet " + nickname);
					WaitForConnect.Add (nickname);

					RSAParameters keyInfo = new RSAParameters ();
					RSACryptoServiceProvider rsa_toCopy = new RSACryptoServiceProvider ();
					byte[] publicKey = Encoding.ASCII.GetBytes (use_data);
					byte[] publicExp = {1,0,1};

					keyInfo.Modulus = publicKey;
					keyInfo.Exponent = publicExp;
					rsa_toCopy.ImportParameters (keyInfo);

					rsa = rsa_toCopy;

					byte[] key = rsa.Encrypt (des.Key, false);
					byte[] iv = rsa.Encrypt (des.IV, false);

					byte[] buffer = {0}; // быдлокод
					Array.Resize (ref buffer, key.Length + iv.Length);
					Array.Copy (key, buffer, key.Length);
					Array.Copy (iv, 0, buffer, key.Length, iv.Length);
						
					Send (Encoding.ASCII.GetString(buffer), passwordHead);
					break;
			
				case passwordHead:
					Console.WriteLine ("\tpassword from " + nickname);

					byte[] buff = Encoding.ASCII.GetBytes(decrypt(use_data));

					byte[] inputKey = {0}; Array.Resize(ref inputKey, des.KeySize);
					byte[] inputIv = {0}; Array.Resize(ref inputIv, des.KeySize);

					for(int i = 0; i < des.KeySize; i++)
						inputKey[i] = buff[i];
					for(int i = 0; i < des.KeySize; i++)
						inputIv[i] = buff[i + des.KeySize];

					if (firstMeet == true)
					{
						des.Key = xor(des.Key, inputKey);
						des.IV = xor(des.IV, inputIv);

						for(int i = 0; i < des.KeySize; i++)
							buff[i] = des.Key[i];
						for(int i = 0; i < des.KeySize; i++)
							buff[i + des.KeySize] = des.IV[i];

						Send(Encoding.ASCII.GetString(buff), passwordHead);

						firstMeet = false;
					}

					if (WaitForConnect.Contains(nickname))
					{
						des.Key = inputKey;
						des.IV = inputIv;

						for(int i = 0; i < des.KeySize; i++)
							buff[i] = des.Key[i];
						for(int i = 0; i < des.KeySize; i++)
							buff[i + des.KeySize] = des.IV[i];

						WaitForConnect.Remove(nickname);
					}
					break;
			
				case dissconnectHead:
					Console.WriteLine ("\t" + nickname + " left chat");
					break;
			
				default:
					throw new InvalidOperationException();
				}
			}

			return head;
		}

        public void Send (string data, char head = messageHead) // Использует шифрование
        {
            UdpClient udpclient = new UdpClient();

            IPAddress multicastaddress = IPAddress.Parse("239.0.0.222");
            udpclient.JoinMulticastGroup(multicastaddress);
            IPEndPoint remoteep = new IPEndPoint(multicastaddress, 2222);

			data = Encoding.ASCII.GetString (crypt (data)); // быдлокод

			Byte[] buffer = Encoding.ASCII.GetBytes(head + username + separator + data);

            udpclient.Send(buffer, buffer.Length, remoteep);
        }

		public void Send (byte[] data, char head = firstMeetHead) // Не использует шифрование
		{
			UdpClient udpclient = new UdpClient();

			IPAddress multicastaddress = IPAddress.Parse("239.0.0.222");
			udpclient.JoinMulticastGroup(multicastaddress);
			IPEndPoint remoteep = new IPEndPoint(multicastaddress, 2222);

			byte[] buffer = Encoding.ASCII.GetBytes(head + username + separator);
			int oldSize = buffer.Length;

			Array.Resize (ref buffer, buffer.Length + data.Length);

			Array.Copy (data, 0, buffer, oldSize, data.Length);

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

            Console.WriteLine("\tListening this will never quit so you will need to ctrl-c it");

            while (true)
            {
                Byte[] data = client.Receive(ref localEp);
				string strData = Encoding.ASCII.GetString(data);

				digest (strData);
            }
        }

		private RSACryptoServiceProvider rsa;
		private DESCryptoServiceProvider des;
		private ICryptoTransform cryptor;
		private ICryptoTransform decryptor;

		private byte[] crypt (string data)
		{
			byte[] buffer = Encoding.ASCII.GetBytes (data);

			return cryptor.TransformFinalBlock (buffer, 0, buffer.Length);
		}

		private byte[] crypt (byte[] data)
		{
			return cryptor.TransformFinalBlock (data, 0, data.Length);
		}

		private string decrypt (byte[] data)
		{
			byte[] buffer = decryptor.TransformFinalBlock (data, 0, data.Length);

			return Encoding.ASCII.GetString (buffer);
		}

		private string decrypt (string data)
		{
			byte[] buffer = Encoding.ASCII.GetBytes (data);
			byte[] crypted = decryptor.TransformFinalBlock (buffer, 0, buffer.Length);

			return Encoding.ASCII.GetString (crypted);
		}

		private byte[] xor (byte[] one, byte[] two)
		{
			byte[] res = {0};
			Array.Resize(ref res, one.Length);

			for(int i = 0; i < res.Length; ++i)
			{
				res[i] = (byte)((int)one [i] ^ (int)two [i]);
			}

			return res;
		}
    }
}
