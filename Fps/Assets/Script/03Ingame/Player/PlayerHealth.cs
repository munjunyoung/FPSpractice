using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class PlayerHealth : MonoBehaviour
{
    Player playerSC;
    Slider HealthSlider;
    Color redColor = new Color(1, 150 / 255f, 150 / 255f, 1);

    // Use this for initialization
    void Start()
    {
        if (GetComponent<Player>().Client)
        {
            playerSC = GetComponent<Player>();
            HealthSlider = GameObject.Find("HealthSlider").GetComponent<Slider>();
            SetHealthUI(playerSC.hp);
        }
    }
    
    /// <summary>
    /// set Health UI value
    /// </summary>
    public void SetHealthUI(int hp)
    {
        playerSC.hp = hp;
        if (playerSC.hp > 30)
            HealthSlider.transform.Find("Fill Area").transform.Find("Fill").GetComponent<Image>().color = Color.white;
        else
            HealthSlider.transform.Find("Fill Area").transform.Find("Fill").GetComponent<Image>().color = redColor;

        HealthSlider.value = playerSC.hp;
    }

    /// <summary>
    /// player take Damage Func
    /// </summary>
    /// <param name="damage"></param>
    public void TakeDamage(int hp)
    {
        //player take damage action..
        SetHealthUI(hp);
    }

    /// <summary>
    /// player Death Func
    /// </summary>
    public void Death()
    {
        if (playerSC.Client)
        {
            SetHealthUI(0);
            GetComponent<PlayerMovement>().enabled = false;
            GetComponent<Player>().aliveState = false;
            GameObject.Find("EffectPanel").GetComponent<Image>().color = new Color(39 / 255f, 39 / 255f, 39 / 255f, 100 / 255f);
        }
        //GetComponent<Animator>().SetTrigger("Run");
        GetComponent<Collider>().isTrigger = true;

        //..animation Death and object Destroy
        //..and shooting moving send gg
    }

    /// <summary>
    /// SET UI after player Death
    /// </summary>
    public void SetEndUI()
    {

    }
}
