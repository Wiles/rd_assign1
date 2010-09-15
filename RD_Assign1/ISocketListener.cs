using System;

namespace RD_Assign1
{
    public interface ISocketListener
    {
        int GetID();

        void OnBind(SocketServer Server);
        void OnConnect(SocketServer Server);
        void OnReceive(SocketServer Server);
        void OnClose(SocketServer Server);
    }
}
