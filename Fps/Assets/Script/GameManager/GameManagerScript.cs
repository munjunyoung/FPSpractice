using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Client;
using System.Net;
using UnityEngine.SceneManagement;
namespace GameManager
{
    public class GameManagerScript : MonoBehaviour
    {
        
        private void Awake()
        {

            ClientNetworkManager.ConnectToServer(ClientNetworkManager.ip, 23000);
            DontDestroyOnLoad(this.gameObject);
        }

        private void Update()
        {
           // if (ClientNetworkManager.Connected == false)
           //     ClientNetworkManager.ConnectToServer("172.30.1.23", 23000);
        }


    }

    
}