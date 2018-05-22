using FpsLib;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;


namespace FpsServerStarter
{
    internal class Client
    {
        public Socket Sock { get; set; }
        private readonly byte[] _tempBuffer = new byte[4096];
        private List<byte> _buffer = new List<byte>();

        public string LoginID = null;

        public bool loginState = false;
        public bool queueState = false;
        public bool ingameState = false;

        public PositionInfo startPosition;

        //Constructor
        public Client(Socket socket)
        {
            Sock = socket;
        }

        #region Send
        // Send Data to client
        public void BeginSend(Packet packet)
        {
            var packetStr = JsonConvert.SerializeObject(packet, Formatting.Indented, new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.Objects
            });
            var sendBuf = Encoding.UTF8.GetBytes(packetStr);

            Sock.BeginSend(sendBuf, 0, sendBuf.Length, SocketFlags.None, OnSendCallBack, Sock);
            Console.WriteLine("Send : [" + packet.msgName + "] to [" + Sock.RemoteEndPoint + "]");
        }

        private void OnSendCallBack(IAsyncResult ar)
        {
            Console.WriteLine("OnSend..");
            var sock = (Socket)ar.AsyncState;
            //var size = sock.EndSend(ar);
        }
        #endregion

        #region Receive
        //Receive asynchronous data
        public void BeginReceive()
        {
            if (!Sock.Connected)
                return;

            System.Array.Clear(_tempBuffer, 0, _tempBuffer.Length);
            try
            {
                Sock.BeginReceive(_tempBuffer, 0, _tempBuffer.Length, SocketFlags.None, OnReceiveCallBack, Sock);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }
        //receive Callback Func
        private void OnReceiveCallBack(IAsyncResult ar)
        {
            var clientSock = (Socket)ar.AsyncState;

            if (!clientSock.Connected)
                return;
            try
            {
                var size = clientSock.EndReceive(ar);

                if (size == 0)
                {
                    Console.WriteLine("Unexpected Requuest. Remove Client");
                    return;
                }
                ProcessData();
            }
            catch (Exception e)
            {
                Console.WriteLine("receiveCallback Exception : " + e);
            }
        }

        private void ProcessData()
        {
            var packetStr = Encoding.UTF8.GetString(_tempBuffer);

            var receivedPacket = JsonConvert.DeserializeObject<Packet>(packetStr, new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.Objects
            });

            if (!ingameState)
            {
                Console.WriteLine("Receive Msg : Lobby[" + receivedPacket.msgName+"]");
                ProcessRequest(receivedPacket);
            }
            else
            {
                Console.WriteLine("Receive Msg : Ingame[" + receivedPacket.msgName + "]");
               
            }

            BeginReceive();//re Receive
        }

        private void ProcessRequest(Packet req)
        {

            switch (req.msgName)
            {
                case "LoginInfo":
                    var loginData = JsonConvert.DeserializeObject<LoginInfo>(req.data);
                    LoginID = loginData.Id;
                    loginState = true;
                    GameManager.AddClient(this);
                    BeginSend(loginData);
                    break;
                case "QueueEntry":
                    //Queue Start Method...
                    queueState = true;
                    BeginSend(new QueuePermission
                    {
                        req = "YES"
                    });
                    GameManager.MatchingRequest(this);
                    break;
                case "QueueCancelReq":
                    queueState = false;
                    GameManager.QueueCancelRequest(this);
                    BeginSend(new QueueCancelPermission());
                    break;
                case "ExitReq":
                    Close();
                    break;
                default:
                    Console.WriteLine("default MsgName : " + req.msgName);
                    break;
            }
        }

        #endregion
        //소켓닫자
        public void Close()
        {
            Sock.Shutdown(SocketShutdown.Both); //양쪽모두 사용하지 않도록함
            Sock.Close();
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
