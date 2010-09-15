using System;
using System.Net;
using System.Net.Sockets;
using System.Collections.Generic;

namespace RD_Assign1
{
	public class SocketServer:IDisposable
	{
        private const int kDefaultPort = 8021;

        // Client connection count
        private const int kBackLog = 128;

        // Maximum packet data size
        private const int kMaxNetBuffer = 2048;

        private bool Running = true;

        private Socket Socket;
        private List<Socket> Clients;
        private List<ISocketListener> Listeners;

		public SocketServer ()
		{
            this.Socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            this.Clients = new List<Socket>();
            this.Listeners = new List<ISocketListener>();
		}

        public void Dispose()
        {
            this.Running = false;
            this.Socket.Close();

            foreach( Socket c in this.Clients )
            {
                c.Close();
            }
        }
		
		public void Bind( int port = kDefaultPort )
        {
            this.Socket.Bind(new IPEndPoint(0, port));
            this.Socket.Listen(kBackLog);

            foreach (ISocketListener listener in Listeners)
            {
                listener.OnBind(this);
            }
		}
		
		public void Send( int ID, ref Byte[] buf )
        {
            this.Clients[ID].Send(buf);
		}
		
		public void MessageLoop ()
        {
            while (this.Running)
            {
                byte[] buffer = new byte[kMaxNetBuffer];
                

            }
		}
    }
}
