using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NBSToTraincarts.Attatchments
{
    internal class AttatchmentPosition
    {
        public string transform { get; set; }
        public double posX { get; set; }
        public double posY { get; set; }
        public double posZ { get; set; }
        public double rotX { get; set; }
        public double rotY { get; set; }
        public double rotZ { get; set; }

        public AttatchmentPosition()
        {
            transform = "HYBRID_ARMORSTAND_HEAD";
            posX = 0;
            posY = 0;
            posZ = 0;
            rotX = 0;
            rotY = 0;
            rotZ = 0;
        }
    }
}
