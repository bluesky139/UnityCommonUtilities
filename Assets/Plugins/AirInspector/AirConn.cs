using common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;

namespace AirInspector
{
    public class AirConn
    {
        protected class StateObject
        {
            public Socket socket;
            public const int kBufferSize = 15360;
            public byte[] buffer = new byte[kBufferSize];
            public StringBuilder sb = new StringBuilder();  
        }
        protected StateObject   state = new StateObject();
        protected RecvCallback  recvCallback;
        protected NetRingPacket ringPacket = new NetRingPacket();

        public bool isConnected
        {
            get
            {
                return state.socket != null && state.socket.Connected;
            }
        }

        public virtual void Close()
        {
            try
            {
				Debug.Log("AirConn close connection.");
				if (state != null && state.socket != null)
                {
                    if (state.socket.Connected)
                    {
                        state.socket.Shutdown(SocketShutdown.Both);
                    }
                    state.socket.Close();
                }
            }
            catch { }
            state        = null;
            ringPacket   = null;
            recvCallback = null;
        }

        protected void RecvCallback(IAsyncResult ar)
        {
            Debug.Log("AirConn RecvCallback");
            Socket handler = state.socket;
            int read = handler.EndReceive(ar);
            if (read > 0)
            {
                byte[] bytes = new byte[read];
                Array.Copy(state.buffer, bytes, read);
                ringPacket.WriteBytes(bytes);
                while (TryGetPacket(out bytes))
                    recvCallback(Error.OK, bytes);
                handler.BeginReceive(state.buffer, 0, state.buffer.Length, 0, RecvCallback, null);
            }
            else
            {
                Debug.LogWarning("Air connection error, recv size 0.");
                recvCallback(Error.Error, null);
            }
        }

        bool TryGetPacket(out byte[] bytes)
        {
            bytes = null;
            if (ringPacket.Length < sizeof(int))
                return false;

            byte[] bytesLen = null;
            if (!ringPacket.ReadBytes(out bytesLen, sizeof(int)))
                return false;
            int len = BitConverter.ToInt32(bytesLen, 0);

            if (ringPacket.Length < sizeof(int) + len)
                return false;
            if (!ringPacket.ReadBytes(out bytes, sizeof(int) + len))
            {
                Debug.LogError("Len is ok, ringPacket.ReadBytes error.");
                return false;
            }
            ringPacket.TrimStart(sizeof(int) + len);
            return true;
        }

        public void Send(byte[] bytes)
        {
            state.socket.BeginSend(bytes, 0, bytes.Length, 0, SendCallback, bytes.Length);
        }

        protected void SendCallback(IAsyncResult ar)
        {
            Debug.Log("AirConn SendCallback");
            int len = (int) ar.AsyncState;
            int sent = state.socket.EndSend(ar);
            if (len != sent)
            {
                Debug.LogError("AirConn send error, sent " + sent + " total " + len);
                recvCallback(Error.Error, null);
            }
        }
    }
}
