/**
 * @file 
 * @author  Hekar Kahni, Samuel Lewis
 * @version 1.0
 *
 * @section DESCRIPTION
 * 
 */


using System;
using System.Net;
using System.Net.Sockets;
using System.Collections.Generic;
using System.Text;

namespace RD_SharedCode
{
    /// <summary>
    /// Client connection to DatabaseServer
    /// </summary>
    public class DatabaseClient : IDisposable
    {
		Socket Client;

        /// <summary>
        /// Client connection DatabaseServer
        /// </summary>
        public DatabaseClient()
        {
			Client = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        }

        /// <summary>
        /// Release allocated resources
        /// </summary>
        public void Dispose()
        {
			Client.Close();
        }

        /// <summary>
        /// Connect to IP4 Address of client
        /// </summary>
        /// <param name="address">Address in string form ie. "127.0.0.1"</param>
        /// <param name="port">Port to connect o</param>
        public void Connect(string address, int port)
        {
			Client.Connect(new IPEndPoint(IPAddress.Parse(address), port));
        }

        /// <summary>
        /// Disconnect from server
        /// </summary>
        public void Disconnect()
        {
            byte[] sendbuffer = new byte[1];
            sendbuffer[0] = (byte)DatabaseMessage.Server_Close;
            Client.Send(sendbuffer);

            Client.Disconnect(false);
			Client.Dispose();
        }

        /// <summary>
        /// Insert a record into the connected Database
        /// </summary>
        /// <param name="record">Record to insert (memberid ignored)</param>
        public void Insert(DataRecord record)
        {
            byte[] sendbuffer = record.ToBytes();
            sendbuffer[0] = (byte)DatabaseMessage.Comm_Insert;
			Client.Send(sendbuffer);

            // Listen for Response
            byte[] recvbuffer = new byte[Shared.kMaxNetBuffer];
			if (Client.Receive(recvbuffer) > 0)
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
                else if (message == DatabaseMessage.Error_DatabaseError)
                {
                    throw new DatabaseException();
                }
            }
        }

        /// <summary>
        /// Update a record in the connected Database server
        /// </summary>
        /// <param name="record">New contains of Record</param>
        public void Update(DataRecord record)
        {
            byte[] sendbuffer = record.ToBytes();
            sendbuffer[0] = (byte)DatabaseMessage.Comm_Update;
			Client.Send(sendbuffer);

            // Listen for Response
            byte[] recvbuffer = new byte[Shared.kMaxNetBuffer];
			if (Client.Receive(recvbuffer) > 0)
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
                else if (message == DatabaseMessage.Error_DatabaseError)
                {
                    throw new DatabaseException();
                }
            }
        }

        /// <summary>
        /// Find a record in the connected Database server and wait for the return
        /// </summary>
        /// <param name="memberid">Memberid of the record to download</param>
        /// <returns>Retrieved record</returns>
        public DataRecord Find(int memberid)
        {
            while (true)
            {
                byte[] sendbuffer = new DataRecord(memberid, "", "", DateTime.Now).ToBytes();
                sendbuffer[0] = (byte)DatabaseMessage.Comm_Find_MemberID;
                Client.Send(sendbuffer);

                // Listen for Response
                byte[] recvbuffer = new byte[Shared.kMaxNetBuffer];
                if (Client.Receive(recvbuffer) > 0)
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
                    else if (message == DatabaseMessage.Error_DatabaseError)
                    {
                        throw new DatabaseException();
                    }
                }
            }

            throw new FormatException();
        }

    }
}
