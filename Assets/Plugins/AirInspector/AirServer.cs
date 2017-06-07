using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text;
using UnityEngine;

namespace AirInspector
{
    class AirServer : AirConn
    {
        int port = 9999;
        int bindRetry = 20;
        Socket listener;

        public void StartListening(RecvCallback recvCallback)
        {
            this.recvCallback = recvCallback;
            listener = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            bool binded = false;
            while (bindRetry > 0)
            {
                if (binded = TryBind(listener, port))
                    break;
                --port;
                --bindRetry;
            }
            if (!binded)
            {
                Debug.LogError("Can't bind port on AirServer.");
                return;
            }

            listener.Listen(100);
            listener.BeginAccept(AcceptCallback, listener);
            Debug.Log("Air server is listening at 0.0.0.0:" + port);

            //string ip = "0.0.0.0";
            //NetworkInterface[] adapters = NetworkInterface..GetAllNetworkInterfaces();
            //Debug.Log("Air server adapters " + adapters.Length);
            //foreach (NetworkInterface adapter in adapters)
            //{
            //    if (adapter.Supports(NetworkInterfaceComponent.IPv4))
            //    {
            //        UnicastIPAddressInformationCollection unicast = adapter.GetIPProperties().UnicastAddresses;
            //        foreach (UnicastIPAddressInformation uni in unicast)
            //        {
            //            if (uni.Address.AddressFamily == AddressFamily.InterNetwork && uni.Address.ToString().StartsWith("10.10.10."))
            //            {
            //                ip = uni.Address.ToString();
            //                break;
            //            }
            //        }
            //    }
            //    if (ip != "0.0.0.0")
            //        break;
            //}
        }

        bool TryBind(Socket listener, int port)
        {
            try
            {
                IPEndPoint endPoint = new IPEndPoint(IPAddress.Parse("0.0.0.0"), port);
                listener.Bind(endPoint);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public override void Close()
        {
            base.Close();
            try
            {
                if (listener != null)
                    listener.Close();
            }
            catch
            { }
            listener = null;
            port = 9999;
            bindRetry = 10;
        }

        void AcceptCallback(IAsyncResult ar)
        {
            Socket listener = (Socket) ar.AsyncState;
            Socket handler  = listener.EndAccept(ar);
            Debug.Log("Air server is connnected.");

            listener.Close();
            state = new StateObject();
            state.socket = handler;
            handler.BeginReceive(state.buffer, 0, StateObject.kBufferSize, 0, RecvCallback, state);
        }
    }
}
