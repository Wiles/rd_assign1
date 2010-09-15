using System;

namespace RD_Assign1
{
    public class DatabaseListener : ISocketListener
    {
        int _ID;

        public DatabaseListener(int ID)
        {
            _ID = ID;
        }

        public int GetID()
        {
            return _ID;
        }

        public void OnBind(SocketServer Server)
        {
        }

        public void OnConnect(SocketServer Server)
        {
        }

        public void OnReceive(SocketServer Server)
        {
        }

        public void OnClose(SocketServer Server)
        {
        }
    }
}
