using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;

namespace FpsLib
{
    public class Packet
    {
        [JsonIgnore]
        public string MsgName => GetType().Name;
        [JsonIgnore]
        public string Data => JsonConvert.SerializeObject(this);
    }
    
    /// <summary>
    /// To send and Receive LoginInfo
    /// </summary>
    public class LoginInfo : Packet
    {
        public string Id { get; set; }
    }
    
    /// <summary>
    /// IngamePacket Position Info
    /// </summary>
    public class PositionInfo : Packet
    {
        public float X { get; set; }
        public float Y { get; set; }
        public float Z { get; set; }

        public PositionInfo(float x, float y, float z)
        {
            X = x;
            Y = y;
            Z = z;
        }
    }

    /// <summary>
    /// ingamePacket Rotation Info
    /// </summary>
    public class RotationInfo : Packet
    {
        public float X { get; set; }
        public float Y { get; set; }
        public float Z { get; set; }

        public RotationInfo(float x, float y, float z)
        {
            X = x;
            Y = y;
            Z = z;
        }
    }

    /// <summary>
    /// stopMove for anim change
    /// </summary>
    public class MoveStop : Packet
    {
        public int Num { get; set; }
        
        public MoveStop(int num)
        {
            Num = num;
        }
    }

    /// <summary>
    /// Player shoot Data
    /// </summary>
    public class ClientShootData : Packet
    {
        public int ClientNum { get; set; }
        public PositionInfo Pos { get; set; }
        public RotationInfo Rot { get; set; }
        public RotationInfo Dir { get; set; }

        public ClientShootData(int num, PositionInfo p, RotationInfo r, RotationInfo d)
        {
            ClientNum = num;
            Pos = p;
            Rot = r;
            Dir = d;
        }
    }

}
