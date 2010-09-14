
using System;

namespace HekarisAwesome
{


	public class DatabaseListener:ISocketListener
	{
		int _ID;

		public DatabaseListener ( int ID )
		{
			_ID = ID;
		}
		
		public void OnBind( SocketServer Server ){}
		public void OnReceive( SocketServer Server ){}
		public void OnClose( SocketServer Server ){}
		public int GetID( ){
			return _ID;
		}
		
	}
}
