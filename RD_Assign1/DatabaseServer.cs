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

		// Maximum packet data size
		private const int kMaxNetBuffer = 2048;

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

			foreach(KeyValuePair<int, Socket> client in this.Clients)
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
			SendMutex.WaitOne();
			this.Clients[ID].Send(buffer);
			SendMutex.ReleaseMutex();
		}

		private class RecieveObject
		{
			public Socket client;
			public ISocketListener listener;
			public byte[] buffer = new byte[kMaxNetBuffer];
		}

		private void OnClientConnected(IAsyncResult result)
		{
			Socket server = (Socket)result.AsyncState;
			Socket handler = server.EndAccept(result);

			// Add the client to the hashtable
			this.Clients.Add(handler.GetHashCode(), handler);

			// Create the listener
			ISocketListener listener = new DatabaseListener(this.database, this.Clients.Count);
			listener.OnConnect(this);
	
			RecieveObject recvstate = new RecieveObject();
			recvstate.client = handler;
			recvstate.listener = listener;

			handler.BeginReceive(recvstate.buffer, 0, kMaxNetBuffer, 0,
				new AsyncCallback(OnRecieved), recvstate);
		}

		private void OnRecieved(IAsyncResult result)
		{
			RecieveObject recvstate = (RecieveObject)result.AsyncState;
			ISocketListener listener = recvstate.listener;
			Socket handler = recvstate.client;

			int size = handler.EndReceive(result);

			if (size > 0)
			{
				listener.OnReceive(this, recvstate.buffer);
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
