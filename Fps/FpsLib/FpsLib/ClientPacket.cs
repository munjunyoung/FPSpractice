using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FpsLib
{
    /// <summary>
    /// To Exit game from client
    /// </summary>
    public class ExitReq : Packet
    {
        public string Req { get; set; }
    }
    /// <summary>
    /// To entry Queue from client
    /// </summary>
    public class QueueEntry : Packet
    {
        public string Req { get; set; }
    }
    /// <summary>
    /// To cancel queue from client
    /// </summary>
    public class QueueCancelReq : Packet
    {
        public string Req { get; set; }
    }

    /// <summary>
    /// To entry ingameScene from client
    /// </summary>
    public class StartGameReq : Packet
    {
        public string Req { get; set; }
    }

    /// <summary>
    /// To Move Player in client
    /// </summary>
    public class ClientPlayerMoving : Packet
    {
        public int ClientNum { get; set; }
        public PositionInfo Pos { get; set; }

        public ClientPlayerMoving(int num, PositionInfo p)
        {
            ClientNum = num;
            Pos = p;
        }
    }

    /// <summary>
    /// playerRotation data;
    /// </summary>
    public class ClientPlayerRotation : Packet
    {
        public int ClientNum { get; set; }
        public RotationInfo Rot { get; set; }

        public ClientPlayerRotation(int num, RotationInfo r)
        {
            ClientNum = num;
            Rot = r;
        }
    }
    /// <summary>
    /// PlayerDamage 
    /// </summary>
    public class EnemyTakeDamage : Packet
    {
        public int Num { get; set; }
        public string GunType { get; set; }
        public string DamagePosition { get; set; }

        public EnemyTakeDamage(int n, string g, string dp)
        {
            Num = n;
            GunType = g;
            DamagePosition = dp;
        }
    }
}
