/**
 * @file
 * @author  Hekar Kahni, Samuel Lewis
 * @version 1.0
 *
 * @section DESCRIPTION
 *
 * 
 */


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
						ErrorReply(Server, DatabaseMessage.Error_InvalidArgs);
					}
					catch (KeyNotFoundException)
					{
						ErrorReply(Server, DatabaseMessage.Error_InvalidArgs);
					}
					catch (OutOfMemoryException)
					{
						ErrorReply(Server, DatabaseMessage.Error_OutOfMemory);
					}
					catch (IndexOutOfRangeException)
					{
						ErrorReply(Server, DatabaseMessage.Error_InvalidArgs);
					}
					catch (FormatException)
					{
						ErrorReply(Server, DatabaseMessage.Error_InvalidArgs);
					}   

					SuccessReply(Server);

					break;
				case DatabaseMessage.Comm_Update:
					try
					{
						PerformUpdate(buffer);
					}
					catch (KeyNotFoundException)
					{
						ErrorReply(Server, DatabaseMessage.Error_ItemNotFound);
					}
					catch (IndexOutOfRangeException)
					{
						ErrorReply(Server, DatabaseMessage.Error_InvalidArgs);
					}
					catch (FormatException)
					{
						ErrorReply(Server, DatabaseMessage.Error_InvalidArgs);
					}

					SuccessReply(Server);

					break;
				case DatabaseMessage.Comm_Find:
					try
					{
						DataRecord record = PerformFind(buffer);
						byte[] sendbuffer = record.ToBytes();
						sendbuffer[0] = (byte)DatabaseMessage.Client_Find;

						Server.Send(GetID(), sendbuffer);
					}
					catch (KeyNotFoundException)
					{
						ErrorReply(Server, DatabaseMessage.Error_ItemNotFound);
					}
					catch (IndexOutOfRangeException)
					{
						ErrorReply(Server, DatabaseMessage.Error_InvalidArgs);
					}

					
					SuccessReply(Server);

					break;
			}
		}

		public void OnClose(DatabaseServer Server)
		{
		}

		private void PerformInsert(byte[] buffer)
		{
			DataRecord record = DataRecord.FromBytes(buffer);
			this.database.Insert(record);
		}

		private void PerformUpdate(byte[] buffer)
		{
			DataRecord record = DataRecord.FromBytes(buffer);
			this.database.Update(record);
		}

		private DataRecord PerformFind(byte[] buffer)
		{
            DataRecord record = DataRecord.FromBytes(buffer);
			return this.database.Find(record.MemberID);
		}

		private void SuccessReply(DatabaseServer server)
		{
			try
			{
				byte[] sendbuffer = { (byte)DatabaseMessage.Success };
				server.Send(GetID(), sendbuffer);
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.StackTrace);
				Console.ReadKey();
			}
		}

		private void ErrorReply(DatabaseServer server, DatabaseMessage reply)
		{
			try
			{
				byte[] sendbuffer = { (byte)reply };
				server.Send(GetID(), sendbuffer);
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.StackTrace);
				Console.ReadKey();
			}
		}
	}
}
