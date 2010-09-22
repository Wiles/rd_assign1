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
using System.Net;
using System.Net.Sockets;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using RD_SharedCode;

namespace RD_Assign1
{
	public class DatabaseServer : IDisposable
	{
		private const int kDefaultPort = 8021;

		// Maximum Client backlog count (on listening)
		private const int kBackLog = 512;

		private bool Running = true;

		private Socket Socket;
		private Dictionary<int, Socket> Clients;
		private Database database;

		private Mutex SendMutex;
		private ManualResetEvent AcceptEvent = new ManualResetEvent(false);

		public DatabaseServer(Database database)
		{
			this.Socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
			this.Clients = new Dictionary<int, Socket>();
			this.database = database;
			SendMutex = new Mutex();
		}

		public void Dispose()
		{
			this.Running = false;
			this.Socket.Close();

			foreach (KeyValuePair<int, Socket> client in this.Clients)
			{
				client.Value.Close();
			}
		}

		public void Bind(int port = kDefaultPort)
		{
			this.Socket.Bind(new IPEndPoint(0, port));
			this.Socket.Listen(kBackLog);
		}

		public void Send(int ID, byte[] buffer)
		{
			try
			{
				if (this.Clients[ID].Connected)
				{
					SendMutex.WaitOne();
					this.Clients[ID].Send(buffer);
					SendMutex.ReleaseMutex();
				}
			}
			catch (KeyNotFoundException ex)
			{
				// Client not found
				Console.WriteLine("(DataServer) Failed to Respond to Client {0}", ID);
				Console.WriteLine("(DataServer) \tException: {0}", ex.Message);

			}
		}

		private void OnClientConnected(Object clientsocket)
		{
			Socket client = (Socket)clientsocket;

			Console.WriteLine("(DataServer) Accepted Client Connection");

			// Add the client to the hashtable
			this.Clients.Add(client.GetHashCode(), client);

			// Create the listener
			ISocketListener listener = new DatabaseListener(this.database, client.GetHashCode());
			listener.OnConnect(this);

			Console.WriteLine("(DataServer) Setting up Recieve State");

			bool listening = true;
			byte[] buffer = new byte[Shared.kMaxNetBuffer];
			while (listening)
			{
				try
				{
					int size = client.Receive(buffer, Shared.kMaxNetBuffer, 0);
					if (size > 0)
					{
						if (buffer[0] == (byte)DatabaseMessage.Server_Close)
						{
							CloseClientConnection(listener, client);
							listening = false;
						}
						else
						{
							listener.OnReceive(this, buffer);
						}
					}
					else
					{
						CloseClientConnection(listener, client);
						listening = false;
					}
				}
				catch (SocketException)
				{
					Console.WriteLine("(DataServer) Errornous Client Disconnection");
					CloseClientConnection(listener, client);
					listening = false;
				}
			}
		}

		private void CloseClientConnection(ISocketListener listener, Socket client)
		{
			listener.OnClose(this);
			client.Close();
			this.Clients.Remove(client.GetHashCode());
		}

		public void ServerLoop()
		{
			while (this.Running)
			{
				Socket client = this.Socket.Accept();

				// Pass the client to a listening thread
				ParameterizedThreadStart acceptthreadstart = OnClientConnected;
				Thread acceptthread = new Thread(acceptthreadstart);
				acceptthread.Start(client);
			}
		}
	}
}
