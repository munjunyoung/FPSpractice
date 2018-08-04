using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FpsLib;
using Client;

public class IngameSceneManagerScript : MonoBehaviour {
    public GameObject Playerprefab;
    GameObject[] playerObject;
    GameObject playerClientObject;
   

    private void Awake()
    {
        playerObject = new GameObject[100];
        ClientNetworkManager.Send(new StartGameReq());
    }

    /// <summary>
    /// 플레이어 생성
    /// </summary>
    /// <param name="num"></param>
    /// <param name="p"></param>
    public void PlayerIns(ClientPlayerIns data)
    {
        playerClientObject = Instantiate(Playerprefab);
        playerClientObject.transform.position = TransPosToVector(data.Pos);
        playerClientObject.GetComponent<Player>().number = data.ClientNum;
        playerClientObject.GetComponent<Player>().Client = true;
        playerClientObject.GetComponent<Player>().hp = data.HP;
        playerObject[data.ClientNum] = playerClientObject;
        Debug.Log("[IngameSceneManagerSC] : Client Player Number : " + data.ClientNum);
    }
    
    /// <summary>
    /// player take damage set hp
    /// </summary>
    /// <param name="hp"></param>
    public void PlayerTakeDamage(int hp)
    {
        playerClientObject.GetComponent<PlayerHealth>().TakeDamage(hp);
    }

    public void PlayerDeath(int num)
    {
        playerObject[num].GetComponent<PlayerHealth>().SetHealthUI(0);
        playerObject[num].GetComponent<PlayerHealth>().Death();
    }
    
    public void EnemyDeath(int num)
    {

    }
    /// <summary>
    /// 적 생성
    /// </summary>
    /// <param name="num"></param>
    /// <param name="p"></param>
    public void EnemyIns(int num, PositionInfo p)
    {
        playerObject[num] = Instantiate(Playerprefab);
        playerObject[num].transform.position = TransPosToVector(p);
        playerObject[num].GetComponent<Player>().number = num;
        playerObject[num].GetComponent<Player>().Client = false;
        
        
        Debug.Log("[IngameSceneManagerSC] : Enemy Player Number : " + num);
    }
    
    /// <summary>
    /// 적움직임
    /// </summary>
    /// <param name="num"></param>
    /// <param name="p"></param>
    public void EnemyMove(int num, PositionInfo p)
    {
        if (playerObject[num] != null)
        {
            playerObject[num].transform.position = TransPosToVector(p);
            playerObject[num].GetComponent<Animator>().SetTrigger("Run");
        }
    }
    
    /// <summary>
    /// Enemy Rotation Trans Func
    /// </summary>
    /// <param name="num"></param>
    /// <param name="r"></param>
    public void EnemyRotation(int num, RotationInfo r)
    {
        if(playerObject[num] != null)
        {
            playerObject[num].transform.eulerAngles = TransRotToVector(r);
        }
    }

    /// <summary>
    /// Player Shoot Func
    /// </summary>
    /// <param name="data"></param>
    public void PlayerShoot(ClientShootData data)
    {
        var p = TransPosToVector(data.Pos);
        var r = TransRotToVector(data.Rot);
        var d = TransRotToVector(data.Dir);

        playerClientObject.GetComponent<PlayerShooting>().ShellInsServer(data.ClientNum, p, r, d, playerClientObject.GetComponent<Player>().Client);
    }

    /// <summary>
    /// enemyShootData clientHealth decrease
    /// </summary>
    /// <param name="data"></param>
    public void EnemyShoot(EnemyShootData data)
    {
        if(playerObject[data.ClientNum] != null)
        {
            var p = TransPosToVector(data.Pos);
            var r = TransRotToVector(data.Rot);
            var d = TransRotToVector(data.Dir);
            playerObject[data.ClientNum].GetComponent<PlayerShooting>().ShellInsServer(data.ClientNum,p, r, d, playerObject[data.ClientNum].GetComponent<Player>().Client);
        }
    }

    /// <summary>
    /// Enemy move Stop Func
    /// </summary>
    /// <param name="num"></param>
    public void EnemyStop(int num)
    {
        if (playerObject[num] != null)
        {
            playerObject[num].GetComponent<Animator>().SetTrigger("Idle");
            Debug.Log("Stop Num : " + num);
        }
    }

    #region TransData Pos and Rotation
    /// <summary>
    /// Vector로 변경 
    /// </summary>
    /// <param name="p"></param>
    /// <returns></returns>
    public Vector3 TransPosToVector(PositionInfo p)
    {
        Vector3 pos = new Vector3(p.X, p.Y, p.Z);
        return pos;
    }

    /// <summary>
    /// PositionInfo로 변경
    /// </summary>
    /// <param name="p"></param>
    /// <returns></returns>
    public PositionInfo TransVectorToPos(Vector3 p)
    {
        PositionInfo pos = new PositionInfo(p.x, p.y, p.z);
        return pos;
    }

    /// <summary>
    /// Vector로 변경 
    /// </summary>
    /// <param name="r"></param>
    /// <returns></returns>
    public Vector3 TransRotToVector(RotationInfo r)
    {
        Vector3 pos = new Vector3(r.X, r.Y, r.Z);
        return pos;
    }

    /// <summary>
    /// rotationInfo로 변경
    /// </summary>
    /// <param name="r"></param>
    /// <returns></returns>
    public RotationInfo TransVectorToRot(Vector3 r)
    {
        RotationInfo rot = new RotationInfo(r.x, r.y, r.z);
        return rot;
    }
#endregion

}
