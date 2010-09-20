using System;

namespace RD_Assign1
{
    public interface ISocketListener
    {
        int GetID();

        void OnConnect(DatabaseServer Server);
        void OnReceive(DatabaseServer Server, byte[] buffer);
        void OnClose(DatabaseServer Server);
    }
}
