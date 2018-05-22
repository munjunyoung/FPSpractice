using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Client;
using FpsLib;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
public class UIManagerScript : MonoBehaviour {

    private void Update()
    {
        if(SceneManager.GetActiveScene().name=="01Login")
        {
            LoginProcess();
        }
        if(SceneManager.GetActiveScene().name=="02Lobby")
        {
            StartButtonProcess();
        }
    }

    #region 01 LoginUI
    public void LoginButtonClickFunc()
    {
        if (ClientNetworkManager.Connected == true)
        {
            Text ID = GameObject.Find("IdText").GetComponent<Text>();
            if (ID == null)
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
            ClientNetworkManager.ConnectToServer(ClientNetworkManager.ip, 23000);
        }
    }
    #endregion

    #region 02 LobbyUI
    public void StartButtonFunc()
    {
        if (ClientNetworkManager.Connected == true)
        {
            if (ClientNetworkManager.queueState == false)
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

    #region Update Func
    bool QueueState = false;
    GameObject GameStartButton = null;
    GameObject QueuePanel = null;
    
    void LoginProcess()
    {
        if (ClientNetworkManager.ClientID != null)
            SceneManager.LoadScene("02Lobby");
    }
    /// <summary>
    /// Find UI
    /// </summary>
    void StartButtonAwakeSetting()
    {
        if (QueuePanel == null || GameStartButton== null)
        {
            GameStartButton = GameObject.Find("GameStartButton");
            GameStartButton.GetComponent<Button>().onClick.AddListener(() => StartButtonFunc());
            QueuePanel = GameStartButton.transform.Find("QueuePanel").gameObject;
        }
    }

    public void StartButtonProcess()
    {
        StartButtonAwakeSetting();

        if (ClientNetworkManager.queueMsg == "YES" && QueueState == false)
        {
            QueuePanel.SetActive(true);
            QueuePanel.GetComponentInChildren<Text>().text = "Queue Entry...";
            GameStartButton.GetComponentInChildren<Text>().text = "CANCEL";
            ClientNetworkManager.queueMsg = null;
            QueueState = true;
        }

        if (ClientNetworkManager.queueCancel && QueueState)
        {
            QueuePanel.SetActive(false);
            GameStartButton.GetComponentInChildren<Text>().text = "START";
            ClientNetworkManager.queueCancel = false;
            QueueState = false;
        }

        if (ClientNetworkManager.gameStart == true)
        {
            QueuePanel.GetComponentInChildren<Text>().text = "COMPELETE MATCHING !";
            QueuePanel.GetComponent<Animation>().Stop();
            QueuePanel.GetComponent<Image>().color = new Color(0, 1, 2 / 255);
            ClientNetworkManager.gameStart = false;
            StartCoroutine(StartIngame());
        }
    }
    //next Scene Load Coroutine
    IEnumerator StartIngame()
    {
        yield return new WaitForSeconds(3f);
        SceneManager.LoadScene("03Ingame");
    }
    #endregion


    #region Common UI

    public void ExitButtonClickFunc()
    {
        ClientNetworkManager.Send(new ExitReq());
        Application.Quit();
    }
    #endregion
}
