using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FpsLib
{
    public class OutcomeInfo : Packet
    {
        public int Outcome { get; set; }
    }
    
    public class QueuePermission : Packet
    {
        public string req { get; set; }
    }
    public class QueueCancelPermission : Packet
    {
        public string req { get; set; }
    }
    public class StartGamePermission : Packet
    {
        public string req { get; set; }
    }

    public class EndGame : Packet
    {
        public string req { get; set; }
    }
}
