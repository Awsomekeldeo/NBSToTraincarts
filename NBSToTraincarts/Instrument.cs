using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NBSToTraincarts
{
    /**
     * <summary>
     * A class for defining NBS Instruments, see <see cref="Instruments"/> for a list of Vanilla instruments.
     * </summary>
     */
    public class Instrument
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string SoundEvent { get; set; }

        public Instrument(int id, string name, string soundevent) 
        {
            Id = id;
            Name = name;
            SoundEvent = soundevent;
        }

        public override bool Equals(object? obj)
        {
            if (ReferenceEquals (null, obj)) return false;
            if (ReferenceEquals (this, obj)) return true;
            if (obj.GetType () != GetType()) return false;

            if (obj is Instrument inst)
            {
                return Id == inst.Id;
            }

            return false;
        }

        public bool IsVanilla()
        {
            if (Id > 0 && Id <= 15)
            {
                return true;
            }

            return false;
        }

        public static bool operator ==(Instrument left, Instrument right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(Instrument left, Instrument right) 
        {
            return !left.Equals(right); 
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}
