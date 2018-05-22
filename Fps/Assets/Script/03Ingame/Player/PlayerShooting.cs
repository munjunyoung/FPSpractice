using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerShooting : MonoBehaviour {
    public GameObject TargetObject; // 조준점 타겟 오브젝트; raycasting
    public Transform fireTransform;
    public Rigidbody shellPrefab;
    public GameObject[] aims;

    float shootCooltime = 0.5f;
    float reboundValue = 0f;
	
	void Update () {
        PlayerShootFunc();
        returnAim();
    }

    void PlayerShootFunc()
    {
        shootCooltime += Time.deltaTime;
        if (shootCooltime > 0.1f)
        {
            if(Input.GetMouseButton(0))
            {
                ShellIns();
                shootCooltime = 0f;
            }
        }
    }

    //Create Shell Func
    void ShellIns() 
    {
        Rigidbody shellInstance = Instantiate(shellPrefab);
        shellInstance.transform.position = fireTransform.position;
        shellInstance.transform.LookAt(TargetObject.transform.position);
        //shellInstance.velocity = fireForce * TargetObject.transform.forward;

        //실제 총알 반동
        Vector2 reboundCircle = Random.insideUnitCircle * reboundValue; //x, y circle 랜덤값
        TargetObject.transform.localPosition = new Vector3(reboundCircle.x, reboundCircle.y, TargetObject.transform.localPosition.z);
        Vector3 shellDir = TargetObject.transform.position - shellInstance.transform.position; // 방향설정
        shellInstance.GetComponent<ShellController>().dir = shellDir;
        shellInstance.GetComponent<ShellController>().position = shellInstance.transform.position;

        shootReaction();
    }
    // Aim..
    void shootReaction() // 에임이미지함수
    {
        //에임 이미지 벌어지게하기
        foreach (GameObject aim in aims)
        {
            if (aim.transform.localPosition.x < 25)
            {
                aim.transform.Translate(Vector3.right * 7f);
                reboundValue += 1f;
            }
        }
        GetComponent<PlayerMovement>().rotationY+= 0.5f;
        GetComponent<PlayerMovement>().rotationX += Random.Range(-1.0f, 1.0f);
    }

    void returnAim() // 에임회복함수
    {
        foreach (GameObject aim in aims)
        {
            if (aim.transform.localPosition.x > 0)
            {
                aim.transform.localPosition = Vector3.Lerp(aim.transform.localPosition, new Vector3(1f, 0, 0), Time.deltaTime*2f);
                reboundValue = Mathf.Lerp(reboundValue, 0f, Time.deltaTime);
            }
        }
    }
}
