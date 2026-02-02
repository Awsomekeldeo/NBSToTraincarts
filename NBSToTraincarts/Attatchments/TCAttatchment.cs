using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NBSToTraincarts.Attatchments
{
    internal class TCAttatchment
    {
        public string type { get; set; }
        public AttatchmentPosition position { get; set; }
        public Dictionary<int, TCAttatchment> attatchments { get; set; }
        

        public TCAttatchment()
        {
            type = "empty";
            position = new AttatchmentPosition();
            attatchments = new Dictionary<int, TCAttatchment>();
        }
    }
}
