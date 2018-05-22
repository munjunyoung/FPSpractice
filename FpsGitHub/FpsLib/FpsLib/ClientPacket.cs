using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FpsLib
{
    public class ExitReq : Packet
    {
        public string Req { get; set; }
    }
    public class QueueEntry : Packet
    {
        public string req { get; set; }
    }
    public class QueueCancelReq : Packet
    {
        public string Req { get; set; }
    }
    public class StartGameReq : Packet
    {
        public string Req { get; set; }
    }
}
