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
        public string msgName => GetType().Name;
        [JsonIgnore]
        public string data => JsonConvert.SerializeObject(this);
    }

    public class LoginInfo : Packet
    {
        public string Id { get; set; }
    }
    
    public class PositionInfo : Packet
    {
        public float x { get; set; }
        public float y { get; set; }
        public float z { get; set; }

        public PositionInfo(float X, float Y , float Z)
        {
            x = X;
            y = Y;
            z = Z;
        }
    }
}
