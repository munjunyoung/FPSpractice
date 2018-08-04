using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LobbySceneReceiveScript : MonoBehaviour {
    public GameObject GameStartButton;
    public GameObject QueuePanel;
    public bool queueState = false;
    public bool matchingComplete = false;
    
    /// <summary>
    /// when receiving QueuePermission msg, execute
    /// show the QueuePanel 
    /// </summary>
    public void QueueEntryProcess()
    {
        QueuePanel.SetActive(true);
        QueuePanel.GetComponentInChildren<Text>().text = "Queue Entry...";
        GameStartButton.GetComponentInChildren<Text>().text = "CANCEL";
        queueState = true;
    }

    /// <summary>
    /// when receving QueueCancel msg, execute
    /// show off QueuePanel
    /// </summary>
    public void QueueCancelProcess()
    {
        QueuePanel.SetActive(false);
        GameStartButton.GetComponentInChildren<Text>().text = "START";
        queueState = false;
    }

    /// <summary>
    /// Matching Complete
    /// </summary>
    public void MatchingCompleteFunc()
    {
        QueuePanel.GetComponentInChildren<Text>().text = "COMPELETE MATCHING !";
        QueuePanel.GetComponent<Animation>().Stop();
        QueuePanel.GetComponent<Image>().color = new Color(0, 1, 2 / 255);
        matchingComplete = true;
        StartCoroutine(StartIngame());
    }
    /// <summary>
    /// next Scene Load Coroutine
    /// </summary>
    /// <returns></returns>
    IEnumerator StartIngame()
    {
        yield return new WaitForSeconds(3f);
        matchingComplete = false;
        SceneManager.LoadScene("03Ingame");
    }
}
