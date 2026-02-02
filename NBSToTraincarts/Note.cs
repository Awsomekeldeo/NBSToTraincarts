using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NBSToTraincarts
{
    public class Note
    {
        public int Tick { get; set; }
        private int _key;
        public int Key { get => _key; set { _key = Math.Clamp(value, 0, 87); } }
        public int Velocity { get; set; }
        public int Pan { get; set; }
        public int Pitch { get; set; }
        public Instrument Instrument { get; set; }

        public Note(int tick, Instrument instrument, int key, int velocity, int pan, int pitch)
        {
            Instrument = instrument;
            Tick = tick;
            Key = key;
            Velocity = velocity;
            Pan = pan;
            Pitch = pitch;
        }
    }
}
