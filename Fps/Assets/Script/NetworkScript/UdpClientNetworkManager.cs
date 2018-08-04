using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net;
using System.Net.Sockets;
using System.Text;
using FpsLib;
using Newtonsoft.Json;
using System.Threading;

namespace UdpServer
{
     class UdpClientNetworkManager
    {
        public static UdpClient udpSock = null;
        static byte[] udpTempBuffer = new byte[4096];
        static IPEndPoint serverEndPoint;
        public static Thread udpReceiveThread;

        public static Queue<Packet> receiveUdpDataQueue = new Queue<Packet>();

        /// <summary>
        /// Udp Start Func
        /// </summary>
        /// <param name="hostip"></param>
        /// <param name="hostport"></param>
        public static void UdpStart(string hostip, int hostport)
        {
            udpSock = new UdpClient(hostip,hostport);
            Debug.Log("[UDP] : UDP Start");
            udpReceiveThread = new Thread(new ThreadStart(Receive));
            udpReceiveThread.Start();
        }

        /// <summary>
        /// Send
        /// </summary>
        /// <param name="p"></param>
        public static void Send(Packet p)
        {
            var packetStr = JsonConvert.SerializeObject(p, Formatting.Indented, new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.Objects
            });

            var data = Encoding.UTF8.GetBytes(packetStr);
          
            udpSock.Send(data, data.Length);
            Debug.Log("[UDP] Send : [" + p.MsgName + "]");
        }

        /// <summary>
        /// Receive
        /// </summary>
        public static void Receive()
        {
            while (true)
            {
                udpTempBuffer = udpSock.Receive(ref serverEndPoint);
                UdpProcessData(udpTempBuffer, serverEndPoint);
            }
        }

        /// <summary>
        /// Process
        /// </summary>
        /// <param name="data"></param>
        /// <param name="s"></param>
        public static void UdpProcessData(byte[] data, IPEndPoint s)
        {
            var packetStr = Encoding.UTF8.GetString(data);

            var receivedPacket = JsonConvert.DeserializeObject<Packet>(packetStr, new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.Objects
            });
            Debug.Log("[UDP]Receive Msg : [" + receivedPacket.MsgName + "]");
            receiveUdpDataQueue.Enqueue(receivedPacket);
            //request
        }

        public static void CloseUdpSock()
        {
            if(udpSock!=null)
                udpSock.Close();
        }
    }
}
