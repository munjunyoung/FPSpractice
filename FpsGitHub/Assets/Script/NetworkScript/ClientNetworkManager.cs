using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net.Sockets;
using System;
using Newtonsoft.Json;
using FpsLib;
using System.Text;
using System.Linq;

namespace Client
{
    static class ClientNetworkManager
    {
        public static string ip = "172.30.1.1";
        public static Socket _clientSocket = null;
        public static Socket _serverSocket = null;
        private static readonly byte[] _tempBuffer = new byte[4096];
        
        public static string ClientID = null;
        public static bool Connected = false;

        public static string receiveMsg = null;
        
        public static string queueMsg = null;
        public static bool queueState = false;
        public static bool queueCancel = false;
        public static bool gameStart = false;
        public static PositionInfo playerPos = null;

        #region Connect
        public static void ConnectToServer(string hostName, int hostPort)
        {
            _clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            try
            {
                _clientSocket.BeginConnect(hostName, hostPort, OnConnectCallBack, _clientSocket);
                //연결시작
            }
            catch (Exception e)
            {
                Debug.Log("ConnectToServer e : " + e);
            }
        }

        //beginReceive 연결 콜백함수
        private static void OnConnectCallBack(IAsyncResult ar)
        {
            try
            {
                var tempSocket = (Socket)ar.AsyncState;
                tempSocket.EndConnect(ar); //보류중인 비동기 연결 요청을 끝냄
                _serverSocket = tempSocket;

                if (_serverSocket.Connected == true)
                    Debug.Log("서버와 연결되었습니다");
                Receive();
                Connected = true;
            }
            catch (Exception e)
            {
                Debug.Log("OnConnectCallBack e : " + e);
            }
        }

        #endregion

        #region Send Func
        public static void Send(Packet packet)
        {
            var packetStr = JsonConvert.SerializeObject(packet, Formatting.Indented, new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.Objects
            });

            var sendBuf = Encoding.UTF8.GetBytes(packetStr); 

            _clientSocket.BeginSend(sendBuf, 0, sendBuf.Length, SocketFlags.None, SendCallBack, _clientSocket);

        }

        private static void SendCallBack(IAsyncResult ar)
        {
            var sock = (Socket)ar.AsyncState;
           // var size = sock.EndSend(ar);
        }
        #endregion

        #region Receive Func
        public static void Receive()
        {
            System.Array.Clear(_tempBuffer, 0, _tempBuffer.Length);
            if (_serverSocket.Connected == false)
            {
                Debug.Log("소켓연결이 끊겼습니다.");
                return;
            }
            //Array.Clear(buffer, 0, buffer.Length);

            try
            {
                _serverSocket.BeginReceive(_tempBuffer, 0, _tempBuffer.Length, SocketFlags.None, ReceveCallBack, _serverSocket);
            }
            catch (Exception e)
            {
                Debug.Log("Receive Exeception : " + e);
            }
        }
        
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
                    Debug.Log("아무것도 없는 Request.");
                    return;
                }
                ClientProcessData();
            }
            catch (Exception e)
            {
                Debug.Log("ReceiveCallBack Exception : " + e);
            }

        }

        private static void ClientProcessData()
        {
            var packetStr = Encoding.UTF8.GetString(_tempBuffer);

            var receiveData = JsonConvert.DeserializeObject<Packet>(packetStr, new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.Objects
            });
            Debug.Log("Receive Data MsgName  : " + receiveData.msgName);

            ClientProcessRequest(receiveData);

            Receive();
        }

        private static void ClientProcessRequest(Packet req)
        {
            switch (req.msgName)
            {
                case "LoginInfo":
                    var Id = JsonConvert.DeserializeObject<LoginInfo>(req.data);
                    ClientID = Id.Id;   
                    break;

                case "QueuePermission":
                    var PermissionStr = JsonConvert.DeserializeObject<QueuePermission>(req.data);
                    if(PermissionStr.req=="YES")
                    {
                        queueMsg = PermissionStr.req;
                        queueState = true;
                    }
                    break;

                case "QueueCancelPermission":
                    Debug.Log("QueueCancelPermission Receive");
                    queueCancel = true;
                    queueState = false;
                    break;

                case "StartGamePermission":
                    Debug.Log("StartGamePermission Receive");
                    gameStart = true;
                    break;

                case "PositionInfo":
                    Debug.Log("PositionInfor Receive");
                    var pos = JsonConvert.DeserializeObject<PositionInfo>(req.data);
                    playerPos = pos;
                    
                    break;
                default:
                    Debug.Log("Default MsgName : " + req.msgName);
                    break;
            }
        }
        #endregion;
        
        //소켓닫자
        public static void SocketClose()
        {
            if (!_clientSocket.Connected)
                return;
            _clientSocket.Shutdown(SocketShutdown.Both);
            _clientSocket.Close();
        }
    }
}