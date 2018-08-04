using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FpsLib;
using Newtonsoft.Json;
using System.Net;
using System.Net.Sockets;

namespace FpsServerStarter.Ingame
{
    class GameRoom
    {
        public int gameRoomNumber = 0;
        public bool startGame = false;
        public int peopleCount = 0;
        public List<Client> ClientListInGameRoom = new List<Client>();
        public List<PositionInfo> StartPos = new List<PositionInfo>();
        public UdpServer udpS = null;

        /// <summary>
        /// Constructor
        /// </summary>
        public GameRoom()
        {
            startGame = false;
            SetStartPos();
            udpS = new UdpServer();
            udpS.UdpRequest += IngameUdpProcessRequest;
        }

        /// <summary>
        /// Add Client in GameRoom List
        /// </summary>
        /// <param name="c"></param>
        public void AddClientInGameRoom(Client c)
        {
            c.BeginSend(new QueueMatchingCompletePermission());
            c.IngameRequest += IngameTcpProcessRequest;
            //set client position and number

            c.startPosition = StartPos[peopleCount];
            c.playerNumber = peopleCount;
            
            c.hp = 100;
            peopleCount++;
            ClientListInGameRoom.Add(c);

            Console.WriteLine("GAME ROOM : GameRoonClientList Count = " + ClientListInGameRoom.Count);
        }
        
        /// <summary>
        /// PlayerIns
        /// </summary>
        /// <param name="c"></param>
        public void InsPlayers(Client c)
        {
            c.aliveState = true;
            c.hp = 100;
            var ClientInsInfo = new ClientPlayerIns(c.playerNumber, ClientListInGameRoom.Find(x => x.playerNumber == c.playerNumber).startPosition, null, c.hp);
            
            c.BeginSend(ClientInsInfo);

            if (c.playerNumber != 1)
            {
                foreach (Client oc in ClientListInGameRoom)
                {
                    if (oc.playerNumber != c.playerNumber)
                    {
                        //다른 플레이어들에게 나의 존재를 생성하도록 통보 
                        oc.BeginSend(new EnemyPlayerIns(c.playerNumber, c.startPosition, null));
                        //다른 플레이어들의 정보를 받음
                        c.BeginSend(new EnemyPlayerIns(oc.playerNumber, oc.startPosition, null));
                    }
                }
            }
        }

        /// <summary>
        /// Player Move Func
        /// </summary>
        /// <param name="num"></param>
        /// <param name="p"></param>
        public void MovePlayer(int num, PositionInfo p)
        {
            foreach (var cl in ClientListInGameRoom)
            {
                if (cl.playerNumber != num)
                {
                    if (cl.myEndPoint != null)
                        udpS.Send(new EnemyPlayerMoving(num, p), cl.myEndPoint);
                }
            }
        }
        
        /// <summary>
        /// Player Rotate Func
        /// </summary>
        /// <param name="num"></param>
        /// <param name="r"></param>
        public void RotatePlayer(int num, RotationInfo r)
        {
            foreach(var cl in ClientListInGameRoom)
            {
                if(cl.playerNumber != num)
                {
                    if (cl.myEndPoint != null)
                        udpS.Send(new EnemyPlayerRotation(num, r), cl.myEndPoint);
                }
            }
        }

        /// <summary>
        /// StopPlayer Send Msg to All
        /// </summary>
        /// <param name="num"></param>
        public void StopPlayer(int num)
        {
            foreach(var cl in ClientListInGameRoom)
            {
                if(cl.playerNumber !=num)
                {
                    if(cl.myEndPoint !=null)
                        udpS.Send(new MoveStop(num), cl.myEndPoint);
                }
            }
        }

        /// <summary>
        /// shoot Player Send Msg to All
        /// </summary>
        /// <param name="num"></param>
        /// <param name="p"></param>
        /// <param name="r"></param>
        public void ShootPlayer(ClientShootData data)
        {
            foreach(var cl in ClientListInGameRoom)
            {
                if (cl.playerNumber != data.ClientNum)
                {
                    if (cl.myEndPoint != null)
                        udpS.Send(new EnemyShootData(data.ClientNum,data.Pos,data.Rot, data.Dir), cl.myEndPoint);
                }
                else
                {
                    if (cl.myEndPoint != null)
                        udpS.Send(data, cl.myEndPoint);
                }
            }
        }

        /// <summary>
        /// takeDamage Player 
        /// </summary>
        /// <param name="data"></param>
        public void TakeDamagePlayerFunc(EnemyTakeDamage data)
        {
            //..gunType Damage
            //..damage position 
            if (ClientListInGameRoom[data.Num].aliveState)
            {
                ClientListInGameRoom[data.Num].hp -= 30;
                if (ClientListInGameRoom[data.Num].hp > 0)
                    ClientListInGameRoom[data.Num].BeginSend(new ClientTakeDamage(data.Num, ClientListInGameRoom[data.Num].hp));
                else
                {
                    ClientListInGameRoom[data.Num].aliveState = false;
                    ClientListInGameRoom[data.Num].BeginSend(new ClientDeath(data.Num));
                    foreach (Client oc in ClientListInGameRoom)
                    {
                        if (oc.playerNumber != data.Num)
                        {
                            ClientListInGameRoom[data.Num].BeginSend(new EnemyDeath(data.Num));
                        }
                    }
                }
            }
        }
        

        /// <summary>
        /// gameStart CountDown
        /// </summary>
        public void CountDown()
        {
            Console.WriteLine("GAME ROOM" + gameRoomNumber + " : Start Game");
            startGame = true;
        }

        #region Process
        /// <summary>
        /// IngameRequest through TCP
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="p"></param>
        public void IngameTcpProcessRequest(object sender, Packet p)
        {
            var c = sender as Client;

            switch (p.MsgName)
            {
                case "StartGameReq":
                    InsPlayers(c);
                    break;
                case "EnemyTakeDamage" :
                    var damageData = JsonConvert.DeserializeObject<EnemyTakeDamage>(p.Data);
                    TakeDamagePlayerFunc(damageData);
                    break;
                default:
                    Console.WriteLine("[Game Room] TCP : Packet Default Message : " + p.MsgName);
                    break;
            }
        }

        /// <summary>
        /// ingameRequest through UDP
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="Req"></param>
        public void IngameUdpProcessRequest(IPEndPoint sender, Packet Req)
        {
            switch (Req.MsgName)
            {
                case "ClientPlayerIns":
                    var clientData = JsonConvert.DeserializeObject<ClientPlayerIns>(Req.Data);
                    ClientListInGameRoom[clientData.ClientNum].myEndPoint = sender;
                    break;
                case "ClientPlayerMoving":
                    var moveData = JsonConvert.DeserializeObject<ClientPlayerMoving>(Req.Data);
                    MovePlayer(moveData.ClientNum, moveData.Pos);
                    break;
                case "ClientPlayerRotation":
                    var rotData = JsonConvert.DeserializeObject<ClientPlayerRotation>(Req.Data);
                    RotatePlayer(rotData.ClientNum, rotData.Rot);
                    break;
                case "MoveStop":
                    var stopData = JsonConvert.DeserializeObject<MoveStop>(Req.Data);
                    StopPlayer(stopData.Num);
                    break;
                case "ClientShootData":
                    var shootData = JsonConvert.DeserializeObject<ClientShootData>(Req.Data);
                    ShootPlayer(shootData);
                    break;
                case "ExitReq":
                    
                    break;
                default:
                    Console.WriteLine("[Game Room] UDP : Packet Default Message : " + Req.MsgName);
                    break;
            }
        }
        #endregion

        
        /// <summary>
        /// 
        /// </summary>
        public void SetStartPos()
        {
            //StartPos.Add(new PositionInfo(new Random().Next(-45, 45), 1, new Random().Next(-45, 45)));
            StartPos.Add(new PositionInfo(0, 1, 0));
            StartPos.Add(new PositionInfo(10, 1, 10));
            StartPos.Add(new PositionInfo(15, 1, 10));
            StartPos.Add(new PositionInfo(20, 1, 10));
            StartPos.Add(new PositionInfo(15, 1, 10));
        }
    }
}
