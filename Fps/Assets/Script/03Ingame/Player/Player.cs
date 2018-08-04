using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Client;
using UdpServer;
using FpsLib;

public class Player : MonoBehaviour
{
    public bool Client = false;
    public int number = 0;
    public int hp = 0;
    public bool aliveState = true;
    //Send Cooltime Count
    int posCount = 0;
    int rotCount = 0;
    int stopCount = 0;
    bool stopAction = false;

    float shootCooltime = 0f;

    GameObject camObject;
    IngameSceneManagerScript ingameScript;
    PlayerShooting shootScript;
    private void Awake()
    {
        if (!ClientNetworkManager.SocketCheck())
            Client = true;
    }

    private void Start()
    {
        shootScript = GetComponent<PlayerShooting>();
        ingameScript = GameObject.Find("IngameManager").GetComponent<IngameSceneManagerScript>();
        if (Client)
        {
            camObject = GameObject.Find("Main Camera");
            camObject.GetComponent<CameraManager>().playerCenterObject = this.transform.Find("CenterObject").gameObject; //this.transform.Find("Bip001").transform.Find("Bip001 Pelvis").transform.Find("Bip001 Spine").gameObject;
        }
    }
    private void FixedUpdate()
    {
        if (ClientNetworkManager.SocketCheck())
        {
            if (Client)
            {
                if (aliveState)
                {
                    ///Position
                    if (Mathf.Abs(GetComponent<PlayerMovement>().playerMoving.x) > 0.1f || Mathf.Abs(GetComponent<PlayerMovement>().playerMoving.z) > 0.1f)
                        SendClientPosition();
                    else
                        SendClientStop();

                    ///Rotation
                    if (Mathf.Abs(Input.GetAxis("Mouse X")) > 0)
                        SendClientRotation();

                    if (Input.GetMouseButton(0))
                        PlayerShootFunc();
                }
            }

        }
    }

    /// <summary>
    /// shoot Delay Func
    /// </summary>
    void PlayerShootFunc()
    {
        shootCooltime += Time.deltaTime;
        if (shootCooltime > 0.1f)
        {
            ShootSendToServer();
            shootScript.ShootReaction();
            shootCooltime = 0f;
        }
    }

    /// <summary>
    /// client Position Send func
    /// </summary>
    public void SendClientPosition()
    {
        if (posCount == 0)
        {
            var p = ingameScript.TransVectorToPos(this.transform.position);
            ClientPlayerMoving sendData = new ClientPlayerMoving(number, p);
            UdpClientNetworkManager.Send(sendData);
        }
        posCount++;
        if (posCount > 1)
            posCount = 0;

        if (stopAction == true)
        {
            stopAction = false;
            stopCount = 0;
        }
    }

    /// <summary>
    /// send Client Stop
    /// </summary>
    public void SendClientStop()
    {
        if (stopAction == false)
        {
            MoveStop sendData = new MoveStop(number);
            UdpClientNetworkManager.Send(sendData);

            stopCount++;
            if (stopCount > 7)
            {
                stopAction = true;
            }
        }
    }

    /// <summary>
    /// Client Rotation Data send func
    /// </summary>
    public void SendClientRotation()
    {
        if (rotCount == 0)
        {
            var r = ingameScript.TransVectorToRot(this.transform.eulerAngles);
            ClientPlayerRotation sendData = new ClientPlayerRotation(number, r);
            UdpClientNetworkManager.Send(sendData);
        }
        rotCount++;
        if (rotCount > 3)
            rotCount = 0;
    }

    /// <summary>
    /// send Shoot permission to Server
    /// </summary>
    public void ShootSendToServer()
    {
        shootScript.ShootRebound();
        PositionInfo p = ingameScript.TransVectorToPos(shootScript.fireTransform.position);
        RotationInfo r = ingameScript.TransVectorToRot(shootScript.shellRot);
        RotationInfo d = ingameScript.TransVectorToRot(shootScript.shellDir);
        UdpClientNetworkManager.Send(new ClientShootData(GetComponent<Player>().number, p, r, d));
    }

}
