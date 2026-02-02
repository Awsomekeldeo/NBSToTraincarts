using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NBSToTraincarts
{
    public class Track
    {
        protected Dictionary<int, Note> notes = new Dictionary<int, Note>();

        public Track() { }

        public void AddNote(int tick, Note note)
        {
            notes.Add(tick, note);
        }

        public Dictionary<int, Note> GetNotes()
        {
            return notes;
        }
    }
}
