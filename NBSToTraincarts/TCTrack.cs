using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace NBSToTraincarts
{
    public class TCTrack : Track
    {
        new private List<Note> notes = new List<Note>();
        public Instrument Instrument { get; set; }
        public int Velocity { get; set; }
        public string TrackName { get; set; }

        public TCTrack(Instrument inst, int velocity, string name)
        {
            Instrument = inst;
            Velocity = velocity;
            TrackName = name;
        }

        public void AddNote(Note note)
        {
            notes.Add(note);
        }
        
        new public List<Note> GetNotes()
        {
            return notes;
        }
    }
}
