using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Client;
using FpsLib;
using Newtonsoft.Json;
using UnityEngine.SceneManagement;
using UdpServer;

public class ProcessNetworkPacket : MonoBehaviour
{
    bool isIngame = false;
    GameManagerScript gameManagerSc;

    private void Awake()
    {
        ClientNetworkManager.ConnectToServer(ClientNetworkManager.ip, ClientNetworkManager.portNumber);
        DontDestroyOnLoad(this.gameObject);
        gameManagerSc = GameObject.Find("GameManager").GetComponent<GameManagerScript>();
    }
    
    private void FixedUpdate()
    {
        ProcessDataQueue();
    }

    /// <summary>
    /// Dequeue Data and Process to pass ProcessRequestClient Func
    /// </summary>
    private void ProcessDataQueue()
    {
        if (UdpClientNetworkManager.receiveUdpDataQueue.Count > 0)
        {
            Packet p = UdpClientNetworkManager.receiveUdpDataQueue.Dequeue();
            ProcessUdpRequestIngame(p);
        }
        if (ClientNetworkManager.receiveDataQueue.Count > 0)
        {
            Packet p = ClientNetworkManager.receiveDataQueue.Dequeue();
            if (!isIngame)
                ProcessTCPRequestClient(p);
            else
                ProcessTcpRequestIngame(p);
        }
       
    }

    /// <summary>
    /// Process Data according to packetMsg
    /// </summary>
    /// <param name="req"></param>
    void ProcessTCPRequestClient(Packet req)
    {
        switch (req.MsgName)
        {
            //Scene 01Login
            case "LoginInfo":
                var info = JsonConvert.DeserializeObject<LoginInfo>(req.Data);
                gameManagerSc.loginScene.LoginProcess(info.Id);
                break;
            //Scene 02Lobby
            case "QueuePermission":
                var PermissionStr = JsonConvert.DeserializeObject<QueuePermission>(req.Data);
                if (PermissionStr.Req == "YES")
                {
                    gameManagerSc.lobbyScene.QueueEntryProcess();
                }
                break;

            case "QueueCancelPermission":
                Debug.Log("[PROCESS PACKET] TCP : QueueCancelPermission Receive");
                gameManagerSc.lobbyScene.QueueCancelProcess();
                break;

            case "QueueMatchingCompletePermission":
                Debug.Log("[PROCESS PACKET ] TCP : StartGamePermission Receive");
                gameManagerSc.lobbyScene.MatchingCompleteFunc();
                isIngame = true;
                break;
                
            default:
                Debug.Log("[PROCESS PACKET] TCP : Default MsgName : " + req.MsgName);
                break;
        }

    }

    /// <summary>
    /// Tcp Process 
    /// </summary>
    /// <param name="req"></param>
    void ProcessTcpRequestIngame(Packet req)
    {
        switch(req.MsgName)
        {
            case "ClientPlayerIns":
                var playerInsData = JsonConvert.DeserializeObject<ClientPlayerIns>(req.Data);
                gameManagerSc.ingameScene.PlayerIns(playerInsData);
                UdpClientNetworkManager.UdpStart(ClientNetworkManager.ip, ClientNetworkManager.portNumber);
                UdpClientNetworkManager.Send(playerInsData);
                break;
            case "EnemyPlayerIns":
                var enemyInsData = JsonConvert.DeserializeObject<EnemyPlayerIns>(req.Data);
                gameManagerSc.ingameScene.EnemyIns(enemyInsData.EnemyNumber, enemyInsData.Pos);
                break;
            case "ClientTakeDamage":
                var damageData = JsonConvert.DeserializeObject<ClientTakeDamage>(req.Data);
                gameManagerSc.ingameScene.PlayerTakeDamage(damageData.HP);
                break;
            case "ClientDeath":
                var cDeathData = JsonConvert.DeserializeObject<ClientDeath>(req.Data);
                gameManagerSc.ingameScene.PlayerDeath(cDeathData.Num);
                break;
            case "EnemyDeath":
                var eDeathData = JsonConvert.DeserializeObject<EnemyDeath>(req.Data);
                gameManagerSc.ingameScene.PlayerDeath(eDeathData.Num);
                break;
            default:
                Debug.Log("[PROCESS PACKET] TCP INGAME : Ingame Default MsgName : " + req.MsgName);
                break;
        }
    }

    /// <summary>
    /// Udp Process
    /// </summary>
    /// <param name="req"></param>
    void ProcessUdpRequestIngame(Packet req)
    {
        switch(req.MsgName)
        {
            case "EnemyPlayerMoving":
                var enemyMoveData = JsonConvert.DeserializeObject<EnemyPlayerMoving>(req.Data);
                gameManagerSc.ingameScene.EnemyMove(enemyMoveData.EnemyNumber, enemyMoveData.Pos);
                break;
            case "EnemyPlayerRotation":
                var enemyRotData = JsonConvert.DeserializeObject<EnemyPlayerRotation>(req.Data);
                gameManagerSc.ingameScene.EnemyRotation(enemyRotData.EnemyNumber, enemyRotData.Rot);
                break;
            case "MoveStop":
                var stopData = JsonConvert.DeserializeObject<MoveStop>(req.Data);
                gameManagerSc.ingameScene.EnemyStop(stopData.Num);
                break;
            case "ClientShootData":
                var pShootData = JsonConvert.DeserializeObject<ClientShootData>(req.Data);
                gameManagerSc.ingameScene.PlayerShoot(pShootData);
                break;
            case "EnemyShootData":
                var eShootData = JsonConvert.DeserializeObject<EnemyShootData>(req.Data);
                gameManagerSc.ingameScene.EnemyShoot(eShootData);
                break;
            default:
                Debug.Log("[PROCESS PACKET] UDP INGAME : Ingame Default MsgName : " + req.MsgName);
                break;
        }
    }
}
