using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Client;
using FpsLib;
public class ShellController : MonoBehaviour
{

    public GameObject explosionEffectPrefab;
    public Vector3 dir;
    public Vector3 position;
    public int clientNum;
    public bool client;
    float shellSpeed = 3000f;
    // Use this for initialization
    void Awake()
    {
        Destroy(this.gameObject, 1.5f);
    }

    // Update is called once per frame
    private void Update()
    {
        GetComponent<Rigidbody>().AddForceAtPosition(dir.normalized * shellSpeed, position);
    }

    /// <summary>
    /// Trigger
    /// </summary>
    /// <param name="other"></param>
    private void OnTriggerEnter(Collider other)
    {
        //Player on trigger
        if (other.tag == "Player")
        {
            if (other.GetComponent<Player>().number != clientNum)
            {
                GameObject explosionEffect = Instantiate(explosionEffectPrefab);
                explosionEffect.transform.position = this.transform.position;
                Debug.Log("trigger확인");
                if (client)
                {
                    ClientNetworkManager.Send(new EnemyTakeDamage(other.GetComponent<Player>().number, null, null));
                    Debug.Log("trigger확인2");
                }
                Destroy(this.gameObject);
            }
        }
        if (other.transform.gameObject.layer == 8)
        {
            GameObject explosionEffect = Instantiate(explosionEffectPrefab);
            explosionEffect.transform.position = this.transform.position;

            Destroy(this.gameObject);
        }
        
        if(other.tag == "TestTag")
        {
            Debug.Log("trigger확");
        }
    }
}
