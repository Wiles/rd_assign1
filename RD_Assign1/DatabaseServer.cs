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

		// Store recieved data between ASync requests
		private class RecieveObject
		{
			public Socket client;
			public ISocketListener listener;
			public byte[] buffer = new byte[Shared.kMaxNetBuffer];
		}

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

		private void OnClientConnected(Object client)
		{
			Socket handler = (Socket)client;

			Console.WriteLine("(DataServer) Accepted Client Connection");

			// Add the client to the hashtable
			this.Clients.Add(handler.GetHashCode(), handler);

			// Create the listener
			ISocketListener listener = new DatabaseListener(this.database, handler.GetHashCode());
			listener.OnConnect(this);

			Console.WriteLine("(DataServer) Setting up Recieve State");

			bool listening = true;
			byte[] buffer = new byte[Shared.kMaxNetBuffer];
			while (listening)
			{
				int size = handler.Receive(buffer, Shared.kMaxNetBuffer, 0);
				if (size > 0)
				{
					if (buffer[0] == (byte)DatabaseMessage.Server_Close)
					{
						CloseClientConnection(listener, handler);
						listening = false;
					}
					else
					{
						listener.OnReceive(this, buffer);
					}
				}
				else
				{
					CloseClientConnection(listener, handler);
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

		public void MessageLoop()
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
