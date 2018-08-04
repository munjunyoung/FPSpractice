using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoginSceneReceiveScript : MonoBehaviour {

    /// <summary>
    /// when receving Logininfo msg from Server, Execute
    /// next Scene
    /// </summary>
    /// <param name="id"></param>
    public void LoginProcess(string id)
    {
        GameObject.Find("GameManager").GetComponent<GameManagerScript>().userID = id;
        SceneManager.LoadScene("02Lobby");
    }
}
