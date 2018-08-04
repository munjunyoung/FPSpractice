using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Client;
using System.Net;
using UnityEngine.SceneManagement;
using FpsLib;
using UdpServer;
public class GameManagerScript : MonoBehaviour
{
    public string userID;
    public static PositionInfo startPos;
    public static int playerNumber;

    public LoginSceneReceiveScript loginScene;
    public LobbySceneReceiveScript lobbyScene;
    public IngameSceneManagerScript ingameScene;

    private void Awake()
    {
        DontDestroyOnLoad(this.gameObject);
    }

    #region SceneManager
    /// <summary>
    /// 
    /// </summary>
    void OnEnable()
    {
        Debug.Log("[GameManager] : OnEnable called");
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    /// <summary>
    /// When each Scene is Loaded, Find Object
    /// </summary>
    /// <param name="scene"></param>
    /// <param name="mode"></param>
    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        switch (scene.name)
        {
            case "01Login":
                loginScene = GameObject.Find("UIManager").GetComponent<LoginSceneReceiveScript>();
                break;
            case "02Lobby":
                lobbyScene = GameObject.Find("UIManager").GetComponent<LobbySceneReceiveScript>();
                break;
            case "03Ingame":
                ingameScene = GameObject.Find("IngameManager").GetComponent<IngameSceneManagerScript>();
                break;
            default:
                break;
        }
        Debug.Log("[GameManager] : OnSceneLoaded : " + scene.name);
    }
    #endregion
    void OnApplicationQuit()
    {
        if (ClientNetworkManager._tcpSocket.Connected)
            ClientNetworkManager.Send(new ExitReq());
        if(UdpClientNetworkManager.udpReceiveThread.IsAlive)
            UdpClientNetworkManager.udpReceiveThread.Abort();
        
        UdpClientNetworkManager.CloseUdpSock();
        ClientNetworkManager.SocketClose();
        Debug.Log("[Game Manager] : 강제종료");
    }

}