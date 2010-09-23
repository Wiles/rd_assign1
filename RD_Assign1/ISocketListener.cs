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

namespace RD_Assign1
{
    /// <summary>
    /// Listener for a SocketServer
    /// </summary>
    public interface ISocketListener
    {
        /// <summary>
        /// Unique Identifier for Socket Listener
        /// </summary>
        /// <returns>Unique Identifier for Socket Listener</returns>
        int GetID();

        /// <summary>
        /// On the connection of a client
        /// </summary>
        /// <param name="Server">Server that client connected to</param>
        void OnConnect(DatabaseServer Server);

        /// <summary>
        /// On the recieving of a packet
        /// </summary>
        /// <param name="Server">Server that recieved message</param>
        /// <param name="buffer">Network buffer</param>
        void OnReceive(DatabaseServer Server, byte[] buffer);

        /// <summary>
        /// On the disconnection of a client
        /// </summary>
        /// <param name="Server">Affected server</param>
        void OnClose(DatabaseServer Server);
    }
}
