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
using System.Xml;
using System.Collections.Generic;
using RD_SharedCode;

namespace RD_Assign1
{
    /// <summary>
    /// Recieves and executes client commands on a per-client basis.
    /// </summary>
    public class DatabaseListener : ISocketListener
    {
        Int32 ID;
        Database database;

        /// <summary>
        /// Listens on behalf of a database for requests
        /// </summary>
        /// <param name="database">Database to listen on behalf of</param>
        /// <param name="ID">The Unique Identifier for this Listener</param>
        public DatabaseListener(Database database, Int32 ID)
        {
            this.database = database;
            this.ID = ID;
        }

        /// <summary>
        /// The unique identifier of the listener
        /// </summary>
        /// <returns>The unique identifier of the listener</returns>
        public int GetID()
        {
            return this.ID;
        }

        /// <summary>
        /// On the connection of a client.
        /// </summary>
        /// <param name="Server">The DatabaseServer that initiated the connection</param>
        public void OnConnect(DatabaseServer Server)
        {
        }

        /// <summary>
        /// On the recievement of a message from a client. Does the message handling.
        /// </summary>
        /// <param name="Server">The DatabaseServer that initiated the connection</param>
        /// <param name="buffer">The recieved message buffer</param>
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
                catch (XmlException)
                {
                    ErrorReply(Server, DatabaseMessage.Error_DatabaseError);
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
                catch (XmlException)
                {
                    ErrorReply(Server, DatabaseMessage.Error_DatabaseError);
                }

                SuccessReply(Server);

                break;
            case DatabaseMessage.Comm_Find_MemberID:
                try
                {
                    DataRecord record = PerformFind(buffer);
                    byte[] sendbuffer = record.ToBytes();
                    sendbuffer[0] = (byte)DatabaseMessage.Client_Found_Record;

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
                catch (XmlException)
                {
                    ErrorReply(Server, DatabaseMessage.Error_DatabaseError);
                }

                SuccessReply(Server);

                break;
            }
        }

        /// <summary>
        /// On the disconnection of a client
        /// </summary>
        /// <param name="Server">DatabaseServer that the client disconnected from</param>
        public void OnClose(DatabaseServer Server)
        {
        }

        /// <summary>
        /// Converts and inserts a buffer into the Database
        /// </summary>
        /// <param name="buffer">Raw client buffer</param>
        private void PerformInsert(byte[] buffer)
        {
            DataRecord record = DataRecord.FromBytes(buffer);
            this.database.Insert(record);
        }

        /// <summary>
        /// Performs an update on a record from a network recievement
        /// </summary>
        /// <param name="buffer">Raw client buffer</param>
        private void PerformUpdate(byte[] buffer)
        {
            DataRecord record = DataRecord.FromBytes(buffer);
            this.database.Update(record);
        }

        /// <summary>
        /// Performs a find based on a network buffer and returns a client found
        /// </summary>
        /// <param name="buffer">Raw client buffer</param>
        /// <returns>DataRecord corresponding to memberid in network buffer</returns>
        private DataRecord PerformFind(byte[] buffer)
        {
            DataRecord record = DataRecord.FromBytes(buffer);
            return this.database.Find(record.MemberID);
        }

        /// <summary>
        /// Replies to a client stating a successful recieve
        /// </summary>
        /// <param name="server">Server to send the reply</param>
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

        /// <summary>
        /// Replies to a client with an error message
        /// </summary>
        /// <param name="server">Server to send the reply</param>
        /// <param name="reply">Error Message</param>
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
