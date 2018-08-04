using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;
using Newtonsoft.Json;
using FpsLib;
using System.Threading;

namespace FpsServerStarter.Ingame
{
    public class UdpServer
    {
        public UdpClient UdpMultiCast { get; set; }
        public byte[] udpTempBuffer = new byte[4096];
        public IPEndPoint sender = new IPEndPoint(IPAddress.Any, 0);

        public delegate void myEventHandler<T>(IPEndPoint sender, Packet p);
        public event myEventHandler<Packet> UdpRequest;

        /// <summary>
        /// Constructor
        /// </summary>
        public UdpServer()
        {
            UdpStart();
        }

        #region Udp Func
        /// <summary>
        /// Udp server Start
        /// </summary>
        public void UdpStart()
        {
            UdpMultiCast = new UdpClient(new IPEndPoint(IPAddress.Any, 23000));
            Console.WriteLine("[UDP] : UDP Server Start!");
            new Thread(new ThreadStart(BeginReceive)).Start();
        }

        /// <summary>
        /// Udp Data send to receiver, 
        /// </summary>
        /// <param name="p"></param>
        /// <param name="s"></param>
        public void Send(Packet p, IPEndPoint s)
        {
            var packetStr = JsonConvert.SerializeObject(p, Formatting.Indented, new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.Objects
            });

            var data = Encoding.UTF8.GetBytes(packetStr);

            UdpMultiCast.Send(data, data.Length, s);
            Console.WriteLine("[UDP] Send : [" + p.MsgName + "] ip : [" + s + "]");
        }

        /// <summary>
        /// UDP Data Receive from Sender
        /// </summary>
        public void BeginReceive()
        {
            while (true)
            {
                udpTempBuffer = UdpMultiCast.Receive(ref sender);
                UdpProcessData(udpTempBuffer, sender);
            }
        }
     
        /// <summary>
        /// Receive Proccess Data
        /// </summary>
        /// <param name="data"></param>
        /// <param name="s"></param>
        public void UdpProcessData(byte[] data, IPEndPoint s)
        {
            var packetStr = Encoding.UTF8.GetString(data);

            var receivedPacket = JsonConvert.DeserializeObject<Packet>(packetStr, new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.Objects
            });
            Console.WriteLine("[UDP]Receive Msg : UdpServer[" + receivedPacket.MsgName + "]");
            UdpRequest(s, receivedPacket);
        }
        
        #endregion

    }
}
