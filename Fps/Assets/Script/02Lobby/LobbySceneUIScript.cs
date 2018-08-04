using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Client;
using UnityEngine.UI;
using FpsLib;
using UnityEngine.SceneManagement;

public class LobbySceneUIScript : MonoBehaviour
{
    #region Scene 02 LobbyUI
    /// <summary>
    /// StartButton OnClick Add Func
    /// </summary>
    public void StartButtonFunc()
    {
        if (ClientNetworkManager._tcpSocket.Connected == true)
        {
            if (!GetComponent<LobbySceneReceiveScript>().queueState)
                ClientNetworkManager.Send(new QueueEntry());
            else
                ClientNetworkManager.Send(new QueueCancelReq());
        }
        else
        {
            Debug.Log("server Disconnect");
            ClientNetworkManager.ConnectToServer(ClientNetworkManager.ip, 23000);
        }
    }
 
    #endregion
}
