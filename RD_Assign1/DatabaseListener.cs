using System;
using System.Collections.Generic;
using RD_SharedCode;

namespace RD_Assign1
{
	/// <summary>
	/// Recieves and executes client commands on a per-database basis.
	/// </summary>
	public class DatabaseListener : ISocketListener
	{
		// Used for error checking. Just the size of the command data alone;
		const int kBufferInsertSize = sizeof(Int32) + DataRecord.kFirstNameMax + DataRecord.kLastNameMax + sizeof(Int64);
		const int kBufferUpdateSize = sizeof(Int32) + DataRecord.kFirstNameMax + DataRecord.kLastNameMax + sizeof(Int64);
		const int kBufferFindSize = sizeof(Int32);

		Int32 ID;
		Database database;

		public DatabaseListener(Database database, Int32 ID)
		{
			this.database = database;
			this.ID = ID;
		}

		public int GetID()
		{
			return this.ID;
		}

		public void OnConnect(DatabaseServer Server)
		{
		}

		public void OnReceive(DatabaseServer Server, byte[] buffer)
		{
			// Command message is first byte
			DatabaseMessage command = (DatabaseMessage)buffer[0];
			switch (command)
			{
				case DatabaseMessage.Comm_Insert:
					try
					{
						PerformInsert(buffer);
					}
					catch (ArgumentException)
					{
						byte[] sendbuffer = { (byte)DatabaseMessage.Error_InvalidArgs };
						Server.Send(GetID(), sendbuffer);
					}
					catch (KeyNotFoundException)
					{

					}
					catch (OutOfMemoryException)
					{

					}
					break;
				case DatabaseMessage.Comm_Update:
					try
					{
						PerformUpdate(buffer);
					}
					catch (Exception)
					{
					}
					break;
				case DatabaseMessage.Comm_Find:
					try
					{
						DataRecord record = PerformFind(buffer);
						byte[] sendbuffer = DataRecord.ToBytes(record);
						sendbuffer[0] = (byte)DatabaseMessage.Recv_Find;

						Server.Send(GetID(), sendbuffer);
					}
					catch (KeyNotFoundException)
					{
					}
					catch (ArgumentException)
					{
					}
					break;
			}
		}

		public void OnClose(DatabaseServer Server)
		{
		}

		private void PerformInsert(byte[] buffer)
		{
			if (buffer.Length < kBufferInsertSize)
			{
				// Not enough arguments
				throw new ArgumentException();
			}
			else
			{
				DataRecord record = DataRecord.FromBytes(buffer);
				this.database.Insert(record);
			}
		}

		private void PerformUpdate(byte[] buffer)
		{
			if (buffer.Length < kBufferUpdateSize)
			{
				throw new ArgumentException();
			}
			else
			{
				DataRecord record = DataRecord.FromBytes(buffer);
				this.database.Update(record);
			}
		}

		private DataRecord PerformFind(byte[] buffer)
		{
			if (buffer.Length < kBufferFindSize)
			{
				throw new ArgumentException();
			}

			int memid = Shared.ByteArrayToInt32(buffer, 1);
			return this.database.Find(memid);
		}
	}
}
