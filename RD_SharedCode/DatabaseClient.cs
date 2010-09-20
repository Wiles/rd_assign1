using System;
using System.Net;
using System.Net.Sockets;
using System.Collections.Generic;
using System.Text;

namespace RD_SharedCode
{
	public class DatabaseClient : IDisposable
	{
		Socket client;
		public DatabaseClient()
		{
			client = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
		}

		public void Dispose()
		{
			client.Close();
		}

		public void Connect(string address, int port)
		{
			client.Connect(new IPEndPoint(IPAddress.Parse(address), port));
		}

		public void Disconnect()
		{
			client.Dispose();
		}

		public void Insert(DataRecord record)
		{
			byte[] sendbuffer = DataRecord.ToBytes(record);
			sendbuffer[0] = (byte)DatabaseMessage.Comm_Insert;
			client.Send(sendbuffer);

			// Listen for Response
			byte[] recvbuffer = new byte[Shared.kMaxNetBuffer];
			if (client.Receive(recvbuffer) > 0)
			{

			}
		}

		public void Update(DataRecord record)
		{
			byte[] sendbuffer = DataRecord.ToBytes(record);
			sendbuffer[0] = (byte)DatabaseMessage.Comm_Update;
			client.Send(sendbuffer);

			// Listen for Response
			byte[] recvbuffer = new byte[Shared.kMaxNetBuffer];
			if (client.Receive(recvbuffer) > 0)
			{

			}
		}
#if false
		public DataRecord Find(int memberid)
		{
			byte[] sendbuffer = new byte[5];
			sendbuffer[0] = (byte)DatabaseMessage.Comm_Find;
			Shared.ByteArrayToInt32(memberid);
			client.Send(sendbuffer);

			// Listen for Response
		}
#endif

	}
}
