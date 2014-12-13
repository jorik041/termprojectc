using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

using System.Security.Cryptography;

using System.Net;
using System.Net.Sockets;

namespace JarlooChat
{
	class Chat
	{
		private readonly string username;
		private bool firstMeet;

		public const char separator = (char)0x2; // Start Of Text

		public const char dissconnectHead = (char)0x4; // EOT - End Of Transmition
		public const char messageHead = (char)0x7;

		public const char publicKeyHead = (char)0x11;
		public const char secretKeyHead = (char)0x12;
		public const char newKeyHead = (char)0x13;


		public Chat (string nameOfUser)
		{
			Random rnd = new Random (1);
			username = nameOfUser;
			firstMeet = true;

			rsa = new RSACryptoServiceProvider ();
			des = new DESCryptoServiceProvider ();
			rnd.NextBytes(des.Key);
			des.IV = des.Key;

			Send (Encoding.Unicode.GetString(rsa.ExportParameters(false).Modulus), publicKeyHead, false);
		}

		~Chat()
		{
			Send("", dissconnectHead, false);
		}

		public void digest (string data)
		{
			char head = data [0];
			int separatorIndex = data.IndexOf (separator);
			string nickname = data.Substring (1, separatorIndex - 1); // [1:sepIndex-1]
			string use_data = data.Substring (separatorIndex + 1); // [sepIndex+1:]

			if (nickname != username) {
				switch (head) {
				case messageHead:
					try
					{
					string text = Encoding.Unicode.GetString(decryptor.TransformFinalBlock (Encoding.Unicode.GetBytes (use_data), 0, use_data.Length));
					}
					catch (Exception e) {
					}
					Console.WriteLine (nickname + ": " + use_data);
					break;

				case publicKeyHead:
					firstMeet = false;

					RSAParameters keyInfo = new RSAParameters ();
					RSACryptoServiceProvider copy = new RSACryptoServiceProvider ();
					byte[] publicKey = Encoding.Unicode.GetBytes (use_data);
					byte[] publicExp = {17};
					keyInfo.Modulus = publicKey;
					keyInfo.Exponent = publicExp;
					copy.ImportParameters(keyInfo);
					rsa = copy;

					byte[] secretKey = rsa.Encrypt(des.Key, false);

					Send(Encoding.Unicode.GetString(secretKey), secretKeyHead, false);
					break;

				case secretKeyHead:
					if(firstMeet == true) {
						byte[] keys = rsa.Decrypt(Encoding.Unicode.GetBytes (use_data), false);

						des.Key = xor(des.Key, keys);
						des.IV = des.Key;
						cryptor = des.CreateEncryptor();
						decryptor = des.CreateDecryptor();

						firstMeet = false;

						byte[] newKey = rsa.Encrypt(des.Key, false);

						Send(Encoding.Unicode.GetString(newKey), newKeyHead, false);
					}
					break;

				case newKeyHead:
					try{
					byte[] rsa_key1 = new byte[1024]; Array.Resize(ref rsa_key1, rsa.KeySize);
					byte[] des_key1 = new byte[1024]; Array.Resize(ref des_key1, rsa.KeySize);

					RSAParameters keyInfo1 = new RSAParameters (); //
					RSACryptoServiceProvider copy1 = new RSACryptoServiceProvider ();
					RSACryptoServiceProvider rsa1 = new RSACryptoServiceProvider ();
					byte[] publicKey1 = Encoding.Unicode.GetBytes (use_data);
					byte[] publicExp1 = {17};
					keyInfo.Modulus = publicKey1;
					keyInfo.Exponent = publicExp1;
					rsa1 = copy1;

					byte[] key = rsa.Decrypt(Encoding.Unicode.GetBytes (use_data), false);
					des.Key = key;
					Console.WriteLine(Encoding.Unicode.GetString (des.Key));
					des.IV = des.Key;
					}
					catch (Exception e)
					{}
					break;

				case dissconnectHead:
					Console.WriteLine ("\t" + nickname + " left chat");
					break;

				default:
					throw new InvalidOperationException ();
				}
			}
		}

		public void Send (string data, char head = messageHead, bool encryption = true)
		{
			Byte[] Buffer = {};
			UdpClient udpclient = new UdpClient();

			IPAddress multicastaddress = IPAddress.Parse("239.0.0.222");
			udpclient.JoinMulticastGroup(multicastaddress);
			IPEndPoint remoteep = new IPEndPoint(multicastaddress, 2222);

			Byte[] buffer = Encoding.Unicode.GetBytes(head + username + separator + data);

			if (encryption == true) {
				try
				{
					Buffer = cryptor.TransformFinalBlock (buffer, 0, buffer.Length);
				}
				catch (Exception e)
				{
				}
				}

			udpclient.Send(buffer, buffer.Length, remoteep);
		}
		/*
		public void Send (byte[] data, char head = messageHead, bool encryption = true)
		{
			UdpClient udpclient = new UdpClient();

			IPAddress multicastaddress = IPAddress.Parse("239.0.0.222");
			udpclient.JoinMulticastGroup(multicastaddress);
			IPEndPoint remoteep = new IPEndPoint(multicastaddress, 2222);

			byte[] buffer = Encoding.ASCII.GetBytes(head + username + separator);
			int oldSize = buffer.Length;

			Array.Resize (ref buffer, buffer.Length + data.Length);

			Array.Copy (data, 0, buffer, oldSize, data.Length);

			udpclient.Send(Encoding.Unicode.GetString(buffer), buffer.Length, remoteep);
		}*/

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
				string strData = Encoding.Unicode.GetString(data);

				digest (strData);
			}
		}

		RSACryptoServiceProvider rsa;
		DESCryptoServiceProvider des;
		ICryptoTransform cryptor;
		ICryptoTransform decryptor;

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

