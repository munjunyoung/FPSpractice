using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Client;
using UnityEngine.UI;
using FpsLib;
using UnityEngine.SceneManagement;

public class LoginSceneUIScript : MonoBehaviour {

    #region LoginButton
    /// <summary>
    /// LoginButton OnClick add Func
    /// </summary>
    public void LoginButtonFunc()
    {
        if (ClientNetworkManager._tcpSocket.Connected == true)
        {
            Text ID = GameObject.Find("IdText").GetComponent<Text>();
            if (ID.text == null)
                Debug.Log("입력해라 아이디");
            else
            {
                ClientNetworkManager.Send(new LoginInfo
                {
                    Id = ID.text
                });
            }
        }
        else
        {
            Debug.Log("네트워크 연결이 되지 않았습니다.");
            ClientNetworkManager.ConnectToServer(ClientNetworkManager.ip, ClientNetworkManager.portNumber);
        }
    }
    #endregion

    #region ExitButton
    public void ExitButtonClickFunc()
    {
        ClientNetworkManager.Send(new ExitReq());
        Application.Quit();
    }
    #endregion
}
