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
            byte[] sendbuffer =  new byte[1];
            sendbuffer[0] = (byte)DatabaseMessage.Server_Close;
            client.Send(sendbuffer);

            client.Disconnect(false);
			client.Dispose();
		}

		public void Insert(DataRecord record)
		{
			byte[] sendbuffer = record.ToBytes();
			sendbuffer[0] = (byte)DatabaseMessage.Comm_Insert;
			client.Send(sendbuffer);

			// Listen for Response
			byte[] recvbuffer = new byte[Shared.kMaxNetBuffer];
			if (client.Receive(recvbuffer) > 0)
			{
				DatabaseMessage message = (DatabaseMessage)recvbuffer[0];
				if (message == DatabaseMessage.Error_InvalidArgs)
				{
					throw new ArgumentException();
				}
				else if (message == DatabaseMessage.Error_OutOfMemory)
				{
					throw new OutOfMemoryException();
				}
			}
		}

		public void Update(DataRecord record)
		{
			byte[] sendbuffer = record.ToBytes();
			sendbuffer[0] = (byte)DatabaseMessage.Comm_Update;
			client.Send(sendbuffer);

			// Listen for Response
			byte[] recvbuffer = new byte[Shared.kMaxNetBuffer];
			if (client.Receive(recvbuffer) > 0)
			{
				DatabaseMessage message = (DatabaseMessage)recvbuffer[0];
				if (message == DatabaseMessage.Error_InvalidArgs)
				{
					throw new ArgumentException();
				}
				else if (message == DatabaseMessage.Error_OutOfMemory)
				{
					throw new OutOfMemoryException();
				}
			}
		}

        public DataRecord Find(int memberid)
        {
            while (true)
            {
                byte[] sendbuffer = new DataRecord(memberid, "", "", DateTime.Now).ToBytes();
                sendbuffer[0] = (byte)DatabaseMessage.Comm_Find_MemberID;
                client.Send(sendbuffer);

                // Listen for Response
                byte[] recvbuffer = new byte[Shared.kMaxNetBuffer];
                if (client.Receive(recvbuffer) > 0)
                {
                    DatabaseMessage message = (DatabaseMessage)recvbuffer[0];
                    if (message == DatabaseMessage.Success)
                    {
                        continue;
                    }
                    if (message == DatabaseMessage.Error_InvalidArgs)
                    {
                        throw new ArgumentException();
                    }
                    else if (message == DatabaseMessage.Error_OutOfMemory)
                    {
                        throw new OutOfMemoryException();
                    }
                    else if (message == DatabaseMessage.Client_Found_Record)
                    {
                        DataRecord record = DataRecord.FromBytes(recvbuffer);
                        return record;
                    }
                    else if (message == DatabaseMessage.Error_ItemNotFound)
                    {
                        throw new KeyNotFoundException();
                    }
                }
            }

            throw new FormatException();
        }

	}
}
