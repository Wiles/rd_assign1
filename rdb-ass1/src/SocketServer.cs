
using System;
using System.Net;
using System.Collections;

namespace HekarisAwesome
{


	public class SocketServer:IDisposable
	{

		public SocketServer ()
		{
			_SerSocket = new SocketServer();
		}
		
		public void Bind( int Port = _DefaultPort ){
		}
		
		public void Send( int ID, ref Byte[] buf ){
			
		}
		
		public void MessageLoop (){
		}
		
		const int _DefaultPort = 8021;
		private SocketServer _SerSocket;
	}
}
