using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UdpServer;
using FpsLib;

public class PlayerShooting : MonoBehaviour
{
    public GameObject TargetObject; // 조준점 타겟 오브젝트; raycasting
    public Transform fireTransform;
    public Rigidbody shellPrefab;
    public GameObject shootEffectPrefab;
    public GameObject[] aims;
    public Vector3 shellDir;
    public Vector3 shellRot;

    float shootCooltime = 0.5f;
    float reboundValue = 0f;


    private void Start()
    {
        if (GetComponent<Player>().Client)
        {
            TargetObject = GameObject.Find("Main Camera").transform.Find("TargetTransform").gameObject;
            aims[0] = GameObject.Find("CenterAim").transform.Find("Up").transform.Find("UpAim").gameObject;
            aims[1] = GameObject.Find("CenterAim").transform.Find("Right").transform.Find("RightAim").gameObject;
            aims[2] = GameObject.Find("CenterAim").transform.Find("Down").transform.Find("DownAim").gameObject;
            aims[3] = GameObject.Find("CenterAim").transform.Find("Left").transform.Find("LeftAim").gameObject;
        }
    }
    void Update()
    {
        if (GetComponent<Player>().Client)
        {
            ReturnAim();
            PlayerShootFunc();
            fireTransform.LookAt(TargetObject.transform);
            var fwd = fireTransform.TransformDirection(Vector3.forward);
            //RaycastHit hit;
            
            Debug.DrawRay(fireTransform.position, fwd * 50, Color.green);
        }
    }

    void PlayerShootFunc()
    {
        if (Input.GetMouseButton(0))
        {
            shootCooltime += Time.deltaTime;
            if (shootCooltime > 0.1f)
            {
                ShellIns();
                shootCooltime = 0f;
            }
        }
    }

    /// <summary>
    /// shell Create Func
    /// </summary>
    void ShellIns()
    {
        Rigidbody shellInstance = Instantiate(shellPrefab);
        shellInstance.transform.position = fireTransform.position;
        //shellInstance.transform.LookAt(TargetObject.transform.position);
        //shellInstance.velocity = fireForce * TargetObject.transform.forward;
        //실제 총알 반동
        
        Vector3 shellDir;
        if (reboundValue<=0)
        {
            TargetObject.transform.localPosition = new Vector3(0, 0f, 200f);
            fireTransform.LookAt(TargetObject.transform.position);
            shellInstance.transform.rotation = fireTransform.rotation;
            shellDir = TargetObject.transform.position - shellInstance.transform.position;
        }
        else
        {
            Vector2 reboundCircle = Random.insideUnitCircle * reboundValue; //x, y circle 랜덤값
            TargetObject.transform.localPosition = new Vector3(reboundCircle.x, reboundCircle.y, TargetObject.transform.localPosition.z);
            fireTransform.LookAt(TargetObject.transform.position);
            shellInstance.transform.rotation = fireTransform.rotation;
            shellDir = TargetObject.transform.position - shellInstance.transform.position; // 방향설정
            
        }

        shellInstance.GetComponent<ShellController>().dir = shellDir;
        shellInstance.GetComponent<ShellController>().position = shellInstance.transform.position;
        ShootReaction();
    }

    /// <summary>
    /// permission Shell Ins from Server
    /// </summary>
    /// <param name="p"></param>
    /// <param name="dir"></param>
    public void ShellInsServer(int num, Vector3 p, Vector3 r, Vector3 d ,bool client)
    {
        //Effect Ins
        GameObject shooteffectIns = Instantiate(shootEffectPrefab);
        shooteffectIns.transform.position = p;
        //Shell Ins
        Rigidbody shellInstance = Instantiate(shellPrefab);
        shellInstance.transform.position = p;
        shellInstance.transform.eulerAngles = r;
        shellInstance.GetComponent<ShellController>().clientNum = num;
        shellInstance.GetComponent<ShellController>().client = client;
      
        //  RaycastHit fwd = transform.transform
        //if(Physics.Raycast(fireTransform.position,fireTransform.forward,out hit, range))
        {

        }
        shellInstance.GetComponent<ShellController>().dir = d;
        shellInstance.GetComponent<ShellController>().position = shellInstance.transform.position;

    }

    /// <summary>
    /// player Shoot Rebound Func
    /// </summary>
    public void ShootRebound()
    {
        if (reboundValue <= 0)
        {
            TargetObject.transform.localPosition = new Vector3(0, 0, 200f);
            fireTransform.LookAt(TargetObject.transform.position);
            shellRot = fireTransform.eulerAngles;
            shellDir = TargetObject.transform.position - fireTransform.position;
        }
        else
        {
            Vector2 reboundCircle = Random.insideUnitCircle * reboundValue; //x, y circle 랜덤값
            TargetObject.transform.localPosition = new Vector3(reboundCircle.x, reboundCircle.y, TargetObject.transform.localPosition.z);
            fireTransform.LookAt(TargetObject.transform.position);
            shellRot = fireTransform.eulerAngles;
            shellDir = TargetObject.transform.position - fireTransform.position; // 방향설정
        }
    }
    /// <summary>
    /// Aim Reacton Func
    /// </summary>
    public void ShootReaction() // 에임이미지함수
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
        GetComponent<PlayerMovement>().rotationY += 0.5f;
        GetComponent<PlayerMovement>().rotationX += Random.Range(-1.0f, 1.0f);
    }

    /// <summary>
    /// AinReturn Func
    /// </summary>
    void ReturnAim() // 에임회복함수
    {
        foreach (GameObject aim in aims)
        {
            if (reboundValue>0)
            {
                aim.transform.localPosition = Vector3.Lerp(aim.transform.localPosition, new Vector3(1f, 0, 0), Time.deltaTime * 2f);
                reboundValue = Mathf.Lerp(reboundValue, -1f, Time.deltaTime);
            }
        }
    }
}
