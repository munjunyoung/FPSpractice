using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net.Sockets;
using System;
using Newtonsoft.Json;
using FpsLib;
using System.Text;
using System.Linq;
using System.Net;


namespace Client
{
    class ClientNetworkManager
    {
        public static string ip = "127.0.0.1";
        public static int portNumber = 23000;
        public static Socket _tcpSocket = null;
        
        private static readonly byte[] _tempBuffer = new byte[4096];
        private static readonly List<byte[]> bodyBufList = new List<byte[]>();
        private static readonly int headSize = 4;
        
        public static Queue<Packet> receiveDataQueue = new Queue<Packet>();

        #region Connect
        /// <summary>
        /// Connect to Server socket
        /// </summary>
        /// <param name="hostIP"></param>
        /// <param name="hostPort"></param>
        public static void ConnectToServer(string hostIP, int hostPort)
        {
            _tcpSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            
            try
            {
                _tcpSocket.BeginConnect(hostIP, hostPort, OnConnectCallBack, _tcpSocket);
                //연결시작
            }
            catch (Exception e)
            {
                Debug.Log("[TCP] ConnectToServer e : " + e);
            }
        }

        /// <summary>
        /// connect Callback func
        /// </summary>
        /// <param name="ar"></param>
        private static void OnConnectCallBack(IAsyncResult ar)
        {
            try
            {
                var tempSocket = (Socket)ar.AsyncState;
                tempSocket.EndConnect(ar); //보류중인 비동기 연결 요청을 끝냄
                _tcpSocket = tempSocket;

                if (_tcpSocket.Connected == true)
                    Debug.Log("[TCP] : 서버와 연결되었습니다");
                Receive();
            }
            catch (Exception e)
            {
                Debug.Log("[TCP] OnConnectCallBack e : " + e);
            }
        }
        
        #endregion

        #region Send Func
        /// <summary>
        /// Func that send Data  headbuf = bodysize 
        /// </summary>
        /// <param name="packet"></param>
        public static void Send(Packet packet)
        {
            var packetStr = JsonConvert.SerializeObject(packet, Formatting.Indented, new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.Objects
            });
            
            var bodyBuf = Encoding.UTF8.GetBytes(packetStr);
            var headBuf = BitConverter.GetBytes(bodyBuf.Length);
            //리스트보다 배열이 빠르므로 배열로 처리해보자
            List<byte> sendPacket = new List<byte>();
            sendPacket.AddRange(headBuf);
            sendPacket.AddRange(bodyBuf);
            
           // Array.Copy(headBuf, 0, sendPacket, 0, headBuf.Length);
           // Array.Copy(bodyBuf, 0, sendPacket, headBuf.Length, bodyBuf.Length);

            _tcpSocket.BeginSend(sendPacket.ToArray(), 0, sendPacket.Count, SocketFlags.None, SendCallBack, _tcpSocket);
             Debug.Log("[TCP] Send : [" + packet.MsgName + "]");
        }

        /// <summary>
        /// SendCallBack
        /// </summary>
        /// <param name="ar"></param>
        private static void SendCallBack(IAsyncResult ar)
        {
            var sock = (Socket)ar.AsyncState;
           // var size = sock.EndSend(ar);
        }
        #endregion

        #region Receive Func

        /// <summary>
        /// Func that receive data
        /// </summary>
        public static void Receive()
        {
            System.Array.Clear(_tempBuffer, 0, _tempBuffer.Length);
            if (_tcpSocket.Connected == false)
            {
                Debug.Log("[TCP] 소켓연결이 끊겼습니다.");
                return;
            }
            //Array.Clear(buffer, 0, buffer.Length);

            try
            {
                _tcpSocket.BeginReceive(_tempBuffer, 0, _tempBuffer.Length, SocketFlags.None, ReceveCallBack, _tcpSocket);
            }
            catch (Exception e)
            {
                Debug.Log("[TCP] Receive Exeception : " + e);
            }
        }
        
        /// <summary>
        /// TCP Receive Callback Func
        /// </summary>
        /// <param name="ar"></param>
        private static void ReceveCallBack(IAsyncResult ar)
        {
            var tempSocket = (Socket)ar.AsyncState;

            if (!tempSocket.Connected)
                return;

            try
            {
                var size = tempSocket.EndReceive(ar);
                
                if (size == 0)
                {
                    Debug.Log("[TCP] 아무것도 없는 Request.");
                    return;
                }
                CheckPacketSize(size);

                foreach(var p in bodyBufList)
                ClientProcessData(p);

                bodyBufList.Clear();
                Receive();
            }
            catch (Exception e)
            {
                Debug.Log("[TCP] ReceiveCallBack Exception : " + e);
            }
        }

        /// <summary>
        /// 만약 전체 사이즈가 헤더의 사이즈보다 클경우 나누어서 bodyBufList에 추가
        /// </summary>
        /// <param name="totalSize"></param>
        private static void CheckPacketSize(int totalSize)
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
        /// when Receving Data, Process Data;
        /// </summary>
        private static void ClientProcessData(byte[] bodyPacket)
        {
            var packetStr = Encoding.UTF8.GetString(bodyPacket);

            var receiveData = JsonConvert.DeserializeObject<Packet>(packetStr, new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.Objects
            });
            Debug.Log("[TCP]Receive Data MsgName  : [" + receiveData.MsgName + "]");

            // enQueue RecevingData;
            receiveDataQueue.Enqueue(receiveData);
        }
        #endregion;
        
        /// <summary>
        /// Close TCP Socket
        /// </summary>
        public static void SocketClose()
        {
            if (!_tcpSocket.Connected)
                return;
            _tcpSocket.Shutdown(SocketShutdown.Both);
            _tcpSocket.Close();
        }

        /// <summary>
        /// socket Connected Check;
        /// </summary>
        /// <returns></returns>
        public static bool SocketCheck()
        {
            bool checkSocketVar = false;
            if (_tcpSocket == null)
                checkSocketVar = false;
            else
                checkSocketVar = _tcpSocket.Connected;

            return checkSocketVar;
        }
    }
}