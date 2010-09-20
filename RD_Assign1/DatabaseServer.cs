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

		private void OnClientConnected(IAsyncResult result)
		{
			Socket server = (Socket)result.AsyncState;
			Socket handler = server.EndAccept(result);

			Console.WriteLine("(DataServer) Accepted Client Connection");

			// Add the client to the hashtable
			this.Clients.Add(handler.GetHashCode(), handler);

			// Create the listener
			ISocketListener listener = new DatabaseListener(this.database, handler.GetHashCode());
			listener.OnConnect(this);

			Console.WriteLine("(DataServer) Setting up Recieve State");

			RecieveObject recvstate = new RecieveObject();
			recvstate.client = handler;
			recvstate.listener = listener;

			Console.WriteLine("(DataServer) Listening for Response");

			handler.BeginReceive(recvstate.buffer, 0, Shared.kMaxNetBuffer, 0,
				new AsyncCallback(OnRecieved), recvstate);
		}

		private void OnRecieved(IAsyncResult result)
		{
			RecieveObject recvstate = (RecieveObject)result.AsyncState;
			ISocketListener listener = recvstate.listener;
			Socket handler = recvstate.client;

			Console.WriteLine("(DataServer) Recieving Data");

			int size = handler.EndReceive(result);

			if (size > 0)
			{
				Console.WriteLine("(DataServer) Recieved Data. Processing Data");
				try
				{
					listener.OnReceive(this, recvstate.buffer);
					Console.WriteLine("(DataServer) Completed OnRecieve");
				}
				catch (Exception ex)
				{
					Console.WriteLine(ex.StackTrace);
					Console.ReadKey();
				}
			}
			
			// Close the connection
			listener.OnClose(this);
			handler.Close();
			this.Clients.Remove(handler.GetHashCode());
		}

		public void MessageLoop()
		{
			while (this.Running)
			{
				if (this.Socket.Poll(1000000, SelectMode.SelectRead))
				{
					this.Socket.BeginAccept(new AsyncCallback(OnClientConnected), this.Socket);
				}
			}
		}
	}
}
