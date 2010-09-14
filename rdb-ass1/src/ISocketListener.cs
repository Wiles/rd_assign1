
using System;

namespace HekarisAwesome
{


	public interface ISocketListener
	{
		void OnBind( SocketServer Server );
		void OnReceive( SocketServer Server );
		void OnClose( SocketServer Server );
		int GetID( );
		
	}
}
