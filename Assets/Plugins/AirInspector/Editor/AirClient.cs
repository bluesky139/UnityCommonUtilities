using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using UnityEngine;

namespace AirInspector
{
    class AirClient : AirConn
    {
        public void StartConnect(string ip, int port, RecvCallback recvCallback)
        {
            try
            {
                this.recvCallback = recvCallback;
                Debug.Log("Start connect " + ip + ":" + port);
                state.socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                IPAddress ipAddress;
                if (!IPAddress.TryParse(ip, out ipAddress))
                    ipAddress = Dns.GetHostEntry(ip).AddressList[0];
                IPEndPoint remoteEP = new IPEndPoint(ipAddress, port);
                state.socket.BeginConnect(remoteEP, ConnectCallback, null);
            }
            catch (Exception e)
            {
                Debug.LogError("Exception on connecting " + e.Message);
                recvCallback(Error.Error, null);
            }
        }

        void ConnectCallback(IAsyncResult ar)
        {
            state.socket.EndConnect(ar);
            if (state.socket.Connected)
            {
                Debug.Log("AirClient is connected.");
                state.socket.BeginReceive(state.buffer, 0, state.buffer.Length, 0, RecvCallback, null);
            }
            else
            {
                Debug.LogError("AirClient can't connect.");
                recvCallback(Error.Error, null);
            }
        }

        public void Disconnect()
        {
            state.socket.Shutdown(SocketShutdown.Both);
            state.socket.Close();
        }
    }
}
