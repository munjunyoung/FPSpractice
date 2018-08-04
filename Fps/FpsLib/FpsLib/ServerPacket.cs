using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FpsLib
{
    /// <summary>
    /// send Queue Entry permission to client
    /// </summary>
    public class QueuePermission : Packet
    {
        public string Req { get; set; }
    }
    /// <summary>
    /// send Queue Cancel permission to client
    /// </summary>
    /// 
    public class QueueCancelPermission : Packet
    {
        public string Req { get; set; }
    }
    /// <summary>
    /// send MatchingComplete msg to client;
    /// </summary>
    /// 
    public class QueueMatchingCompletePermission : Packet
    {
        public string Req { get; set; }
    }

    /// <summary>
    /// To create player in Client
    /// </summary>
    public class ClientPlayerIns : Packet
    {
        public int ClientNum { get; set; }
        public PositionInfo Pos { get; set; }
        public RotationInfo Rot { get; set; }
        public int HP { get; set; }

        public ClientPlayerIns(int num, PositionInfo p, RotationInfo r, int h)
        {
            ClientNum = num;
            Pos = p;
            Rot = r;
            HP = h;
        }
    }

    /// <summary>
    /// send createEnemy msg 
    /// </summary>
    public class EnemyPlayerIns : Packet
    {
        public int EnemyNumber { get; set; }
        public PositionInfo Pos { get; set; }
        public RotationInfo Rot { get; set; }

        public EnemyPlayerIns(int num, PositionInfo p, RotationInfo r)
        {
            EnemyNumber = num;
            Pos = p;
            Rot = r;
        }
    }

    /// <summary>
    /// EnemyMoving data;
    /// </summary>
    public class EnemyPlayerMoving : Packet
    {
        public int EnemyNumber { get; set; }
        public PositionInfo Pos { get; set; }

        public EnemyPlayerMoving(int num, PositionInfo p)
        {
            EnemyNumber = num;
            Pos = p;
        }
    }

    /// <summary>
    /// EnemyRotation data;
    /// </summary>
    public class EnemyPlayerRotation : Packet
    {
        public int EnemyNumber { get; set; }
        public RotationInfo Rot { get; set; }

        public EnemyPlayerRotation(int num, RotationInfo r)
        {
            EnemyNumber = num;
            Rot = r;
        }
    }

    /// <summary>
    /// Eenemy shoot Data
    /// </summary>
    public class EnemyShootData : Packet
    {
        public int ClientNum { get; set; }
        public PositionInfo Pos { get; set; }
        public RotationInfo Rot { get; set; }
        public RotationInfo Dir { get; set; }

        public EnemyShootData(int num, PositionInfo p, RotationInfo r, RotationInfo d)
        {
            ClientNum = num;
            Pos = p;
            Rot = r;
            Dir = d;
        }
    }
    
    /// <summary>
    /// player take damage hp setting
    /// </summary>
    public class ClientTakeDamage : Packet
    {
        public int Num { get; set; }
        public int HP { get; set; }

        public ClientTakeDamage(int n, int h)
        {
            Num = n;
            HP = h;
        }
    }

    /// <summary>
    /// set Player Death
    /// </summary>
    public class ClientDeath : Packet
    {
        public int Num { get; set; }

        public ClientDeath(int n)
        {
            Num = n;
        }
    }
    
    /// <summary>
    /// set Enemy Death
    /// </summary>
    public class EnemyDeath : Packet
    {
        public int Num { get; set; }

        public EnemyDeath(int n)
        {
            Num = n;
        }
    }
    /// <summary>
    /// result data of matching from server to client
    /// </summary>
    public class OutcomeInfo : Packet
    {
        public int Outcome { get; set; }
    }

    /// <summary>
    /// endGame Req
    /// </summary>
    public class EndGame : Packet
    {
        public string Req { get; set; }
    }

}
