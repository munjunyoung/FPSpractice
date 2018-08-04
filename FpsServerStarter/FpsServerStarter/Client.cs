using FpsLib;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Linq;

namespace FpsServerStarter
{
    internal class Client
    {
        public Socket SockTcp { get; set; }
        private readonly byte[] _tempBuffer = new byte[4096];
        private readonly List<byte[]> bodyBufList = new List<byte[]>();
        private readonly int headSize = 4; //headbuf, bodybuf div offset

        public string LoginID = null;
        public bool loginState = false;
        public bool queueState = false;
        public bool ingameState = false;
        public int hp = 0;
        public bool aliveState = false;
        //Udp Communication
        public IPEndPoint myEndPoint = null;
        
        //ingame
        public int playerNumber;
        public PositionInfo startPosition;

        public delegate void myEventHandler<T>(object sender, Packet p);
        public event myEventHandler<Packet> IngameRequest;

        
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="socket"></param>
        public Client(Socket socket)
        {
            SockTcp = socket;
            //SockUdp = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
           
        }

        #region Send

        /// <summary>
        /// Send Data func through TCP;
        /// </summary>
        /// <param name="packet"></param>
        public void BeginSend(Packet packet)
        {
            var packetStr = JsonConvert.SerializeObject(packet, Formatting.Indented, new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.Objects
            });

            var bodyBuf = Encoding.UTF8.GetBytes(packetStr);
            var headBuf = BitConverter.GetBytes(bodyBuf.Length);

            List<byte> sendPacket = new List<byte>();
            sendPacket.AddRange(headBuf);
            sendPacket.AddRange(bodyBuf);
            
            SockTcp.BeginSend(sendPacket.ToArray(), 0, sendPacket.Count, SocketFlags.None, OnSendCallBack, SockTcp);
            Console.WriteLine("[TCP] Send : [" + packet.MsgName + "] to [" + SockTcp.RemoteEndPoint + "]");
        }

        /// <summary>
        /// Send CallBack
        /// </summary>
        /// <param name="ar"></param>
        private void OnSendCallBack(IAsyncResult ar)
        {
            Console.WriteLine("[TCP] OnSend..");
            var sock = (Socket)ar.AsyncState;
            //var size = sock.EndSend(ar);
        }
        #endregion

        #region Receive
        /// <summary>
        /// Tcp Receive
        /// </summary>
        public void BeginReceive()
        {
            if (!SockTcp.Connected)
                return;

            Array.Clear(_tempBuffer, 0, _tempBuffer.Length);
            try
            {
                SockTcp.BeginReceive(_tempBuffer, 0, _tempBuffer.Length, SocketFlags.None, OnReceiveCallBack, SockTcp);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }
        /// <summary>
        /// recevieCallBack
        /// </summary>
        /// <param name="ar"></param>
        private void OnReceiveCallBack(IAsyncResult ar)
        {
            var clientSock = (Socket)ar.AsyncState;

            if (!clientSock.Connected)
                return;
            try
            {
                var size = clientSock.EndReceive(ar);

                Console.WriteLine("[TCP] Receive Data Size : " + size);
                if (size == 0)
                {
                    Console.WriteLine("Unexpected Requuest. Remove Client");
                    return;
                }

                CheckPacketSize(size);

                //checkPacketSize함수에서 처리한 리스트만큼 처리할수 있도록
                foreach (var p in bodyBufList)
                    ProcessData(p);

                bodyBufList.Clear();
                BeginReceive();//re Receive
            }
            catch (Exception e)
            {
                Console.WriteLine("receiveCallback Exception : " + e);
            }
        }
        /// <summary>
        /// 만약 전체 사이즈가 헤더의 사이즈보다 클경우 나누어서 bodyBufList에 추가
        /// </summary>
        /// <param name="totalSize"></param>
        private void CheckPacketSize(int totalSize)
        {
            var tempSize = 0;
            while (totalSize > tempSize)
            {
                var bodySize = _tempBuffer[tempSize];
                byte[] bodyBuf = new byte[1024]; //사이즈별로 동적할당

                Array.Copy(_tempBuffer, tempSize + headSize, bodyBuf, 0, bodySize);
                bodyBufList.Add(bodyBuf);
                tempSize += (bodySize + headSize);
            }
        }
        /// <summary>
        /// 패킷데이터 처리
        /// </summary>
        private void ProcessData(byte[] bodyPacket)
        {
            var packetStr = Encoding.UTF8.GetString(bodyPacket);

            var receivedPacket = JsonConvert.DeserializeObject<Packet>(packetStr, new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.Objects
            });

            if (!ingameState)
            {
                Console.WriteLine("[TCP]Receive Msg : Lobby[" + receivedPacket.MsgName + "]");
                ProcessRequest(receivedPacket);
            }
            else
            {
                Console.WriteLine("[TCP]Receive Msg : Ingame[" + receivedPacket.MsgName + "]");
                IngameRequest(this, receivedPacket);
            }

        }

        /// <summary>
        /// 클라이언트 요청 처리
        /// </summary>
        /// <param name="req"></param>
        private void ProcessRequest(Packet req)
        {
            switch (req.MsgName)
            {
                case "LoginInfo":
                    var loginData = JsonConvert.DeserializeObject<LoginInfo>(req.Data);
                    GameManager.AddClient(this, loginData);
                    BeginSend(loginData);
                    break;
                case "QueueEntry":
                    queueState = true;
                    BeginSend(new QueuePermission
                    {
                        Req = "YES"
                    });
                    MatchingManager.MatchingProcess(this);
                    break;
                case "QueueCancelReq":
                    queueState = false;
                    MatchingManager.QueueRemoveClient(this);
                    BeginSend(new QueueCancelPermission());
                    break;
                case "ExitReq":
                    Close();
                    break;
                default:
                    Console.WriteLine("default MsgName : " + req.MsgName);
                    break;
            }
        }

        #endregion
        /// <summary>
        /// Close TCPSocket
        /// </summary>
        public void Close()
        {
            Console.WriteLine("[TCP] SocketClose : [" + SockTcp.RemoteEndPoint + "]");
            SockTcp.Shutdown(SocketShutdown.Both); //양쪽모두 사용하지 않도록함
            SockTcp.Close();
        }
    }
}




//json convert
/* Console.WriteLine("Receving..");
        var clientSSock = (Socket)ar.AsyncState;

        if(_tempBuffer[0] == 0 )
        {
            Console.WriteLine("Unexpected Req");
           // Close();
            return;
        }



        //json 변환
        var tempData = JsonConvert.DeserializeObject<Packet>(packetStr);
        tempData.data = tempData.Data.Replace("\\\"", "\"");
        tempData.Data = tempData.Data.Substring(1, tempData.Data.Length - 2);

        Console.WriteLine(string.IsNullOrEmpty(tempData.msgName) ? "MsgName has no request. " : tempData.msgName);

        switch(tempData.msgName)
        {
            case "OnLoginRequest":
                Console.Write("Client ID : " + tempData.Data);
                break;
        }*/


/*      if (_buffer.Count < 4)
      {
          BeginReceive();
          Console.WriteLine("t1");
      }
      else
      {
          Console.WriteLine("t2");
          if (_currentPacketLength < 0)
          {
              Console.WriteLine("t3");
              _currentPacketLength = BitConverter.ToInt32(_buffer.Take(4).ToArray(), 0);
              //Console.WriteLine("Parse {0} payload length.", _currentPacketLength);
          }

          if (_buffer.Count < _currentPacketLength + 4)
          {
              BeginReceive();
              Console.WriteLine("t4");
          }
          else
          {
              Console.WriteLine("t5");
              var packetStr = Encoding.UTF8.GetString(_buffer.Skip(4).Take(_currentPacketLength).ToArray());

              _buffer.RemoveRange(0, _currentPacketLength + 4);
              _currentPacketLength = int.MinValue;

              var receivedPacket = JsonConvert.DeserializeObject<Packet>(packetStr, new JsonSerializerSettings
              {
                  TypeNameHandling = TypeNameHandling.Objects
              });

              Console.WriteLine(receivedPacket.MsgName);

              // To distinguish whether client is ingame or not.
              if (!ingameState)
              {
                  Console.WriteLine("Request : Lobby [" + receivedPacket.MsgName + "]");

                  ProcessRequest(receivedPacket);
              }
              else
              {
                  Console.WriteLine("Request : Ingame [" + receivedPacket.MsgName + "]");

                  IngameRequest(this, receivedPacket);
              }

              if (_buffer.Count > 0)
                  ProcessData();

              BeginReceive();
          }
      }*/
