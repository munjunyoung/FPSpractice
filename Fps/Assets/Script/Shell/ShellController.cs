using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShellController : MonoBehaviour {

    public GameObject explosionEffectPrefab;
    public Vector3 dir;
    public Vector3 position;
	// Use this for initialization
	void Start () {
        Destroy(this.gameObject, 5f);
	}

    // Update is called once per frame
    private void Update()
    {
        GetComponent<Rigidbody>().AddForceAtPosition(dir.normalized*20000f, position);
    }

    private void OnTriggerEnter(Collider other)
    {
        GameObject explosionEffect = Instantiate(explosionEffectPrefab);
        explosionEffect.transform.position = this.transform.position;

        Destroy(this.gameObject);
    }
}
