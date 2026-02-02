using System.Collections;
using System.ComponentModel;
using System.Net.Mail;
using System.Runtime.InteropServices;

using YamlDotNet.RepresentationModel;

namespace NBSToTraincarts
{
    static class NBSToTraincarts
    {
        private static Dictionary<int, Track> nbsTracks = new Dictionary<int, Track>();
        private static List<TCTrack> tcTracks = new List<TCTrack>();
        private static bool loop;
        private static int nbs_version;
        private static int loopStart;
        private static int maxLoopCount;
        private static int songLength;
        private static int customIndexDiff;
        private static decimal tempo;
        private static int timeSig;
        private static string songName = "";
        private static string songAuthor = "";
        private static TextWriter? writer;

        static void Main(string[] args)
        {

            if (args.Length == 0)
            {
                Console.WriteLine("Please enter a path to the input file.");
                return;
            }
            else if (args.Length == 1 && args[0] == null)
            {
                Console.WriteLine("Please enter a path to the input file.");
                return;
            }
            else if (args.Length == 2 && args[1] == null)
            {
                Console.WriteLine("Please enter a path to the output directory.");
                return;
            }

            FileStream inputFile = File.OpenRead(args[0]);

            try
            {
                BinaryReader reader = new BinaryReader(inputFile);

                

                //Reimpliment Note Block Studio's file load script in .NET
                int byte1 = reader.ReadByte();
                int byte2 = reader.ReadByte();

                if (byte1 == 0 && byte2 == 0)
                {
                    nbs_version = reader.ReadByte();

                    int songFirstIndex = reader.ReadByte();


                    if (nbs_version >= 3)
                    {
                        songLength = reader.ReadInt16();
                    }
                }
                else
                {
                    Console.WriteLine("The input file is not a valid nbs file.");
                    return;
                }

                int maxTracks = reader.ReadInt16();

                songName = ReadStringFromFile(reader);
                songAuthor = ReadStringFromFile(reader);
                string songOrigAuthor = ReadStringFromFile(reader);
                string songDesc = ReadStringFromFile(reader);

                decimal tickRate = reader.ReadInt16() / (decimal)100.0;
                tempo = (tickRate * 60 / (decimal)4.0);
                //Autosave data is ignored
                reader.ReadByte();
                reader.ReadByte();
                int x = reader.ReadByte();
                timeSig = Math.Clamp(x, 2, 8);

                //Stats (can be skipped since we aren't using this)
                reader.ReadInt32();
                reader.ReadInt32();
                reader.ReadInt32();
                reader.ReadInt32();
                reader.ReadInt32();

                //MIDI filename (also skipped)
                ReadStringFromFile(reader);

                //Looping
                if (nbs_version >= 4)
                {
                    loop = reader.ReadByte() == 1;
                    if (args[0].Contains("format4beta"))
                    {
                        loopStart = reader.ReadByte();
                        maxLoopCount = 0;
                    }
                    else
                    {
                        maxLoopCount = reader.ReadByte();
                        loopStart = reader.ReadInt16();
                    }
                }

                //Start parsing note data
                int currentTick = -1;
                while (true)
                {
                    int tick = reader.ReadInt16();
                    //Found terminator, exit the loop
                    if (tick == 0) break;
                    currentTick += tick;

                    int currentTrack = -1;
                    while (true)
                    {
                        int track = reader.ReadInt16();
                        //Found terminator, exit loop
                        if (track == 0) break;
                        currentTrack += track;

                        int inst = reader.ReadByte();
                        int key = reader.ReadByte();
                        int velocity;
                        int pan;
                        int pitch;
                        if (nbs_version >= 4)
                        {
                            velocity = reader.ReadByte();
                            pan = reader.ReadByte();
                            pitch = reader.ReadInt16();
                        }
                        else
                        {
                            velocity = 100;
                            pan = 100;
                            pitch = 0;
                        }

                        Note note = new Note(currentTick, Instruments.GetVanillaInstrumentById(inst), key, velocity, pan, pitch);
                        ComputeIfAbsent(nbsTracks, currentTrack, k => new Track()).AddNote(currentTick, note);
                    }
                }

                Console.WriteLine("Successfully read song data...");

                MergeTracks();
                Console.WriteLine("Finished merging tracks...");

                BuildYaml(Path.Combine(args[1], songName + ".yml"));
            }
            catch (IOException ex)
            {
                Console.WriteLine("Error reading file.");
            }
        }

        private static string ReadStringFromFile(BinaryReader reader)
        {
            string buffer = "";
            int length = reader.ReadInt32();
            for (int i = 0; i < length; i++)
            {
                char c = reader.ReadChar();
                buffer += c;
            }
            return buffer;
        }

        private static void MergeTracks()
        {
            //For each nbs track
            foreach ((int trackIdx, Track track) in nbsTracks)
            {
                //Iterate through notes
                Dictionary<int, Note> notes = track.GetNotes();
                foreach ((int noteIdx, Note note) in notes)
                {
                    bool foundTrack = false;
                    //Find a traincarts track to put the note in
                    foreach (TCTrack tcTrack in tcTracks)
                    {
                        //Velocity and instrument match, add the note to the track
                        if (note.Instrument == tcTrack.Instrument && note.Velocity == tcTrack.Velocity)
                        {
                            tcTrack.AddNote(note);
                            foundTrack = true;
                        }
                    }

                    if (!foundTrack)
                    {
                        //We did not find an existing track, have to make one
                        tcTracks.Add(new TCTrack(note.Instrument, note.Velocity, note.Instrument.Name + ", Vel: " + note.Velocity));
                        tcTracks.Last().AddNote(note);
                    }
                }
            }
        }

        private static void BuildYaml(string path)
        {
            YamlStream outputStream = new YamlStream();
            YamlDocument tcYaml = new YamlDocument(new YamlMappingNode("type", "ITEM"));
            outputStream.Add(tcYaml);

            YamlMappingNode rootNode = (YamlMappingNode)outputStream.Documents[0].RootNode;
            rootNode.Add("entityType", "MINECART");

            //Basic TC nodes that don't depend on the current track
            YamlMappingNode positionNode = new YamlMappingNode(
                    "transform", "HYBRID_ARMORSTAND_HEAD",
                    "posX", "0.0",
                    "posY", "0.0",
                    "posZ", "0.0",
                    "rotX", "0.0",
                    "rotY", "0.0",
                    "rotZ", "0.0");
            Console.WriteLine(positionNode.ToString());

            YamlMappingNode autoplayNode = new YamlMappingNode(
                    "type", "CONDITIONAL",
                    "left", new YamlMappingNode(
                        "type", "INPUT-PROPERTY",
                        "property", "tags",
                        "ofTrain", "false",
                        "expression", "playSound"),
                    "right", new YamlMappingNode(
                        "type", "CONSTANT",
                        "outuput", "0.0"
                        ),
                    "operator", "BOOL",
                    "falseOutput", new YamlMappingNode(
                        "type", "BOOLEAN",
                        "output", "0.0"
                        ),
                    "trueOutput", new YamlMappingNode(
                        "type", "BOOLEAN",
                        "output", "1.0")
                );

            YamlMappingNode attatchmentsNode = new YamlMappingNode();

            //Add base part of node for track to the yaml
            //YamlMappingNode trackNode = new YamlMappingNode("type", "EMPTY");
            //trackNode.Add("position", positionNode);
            //Console.WriteLine(trackNode.ToString());
            //trackNode.Add(positionNode);
            YamlMappingNode sequencerNode = new YamlMappingNode();

            sequencerNode.Add("type", "SEQUENCER");
            sequencerNode.Add("position", positionNode);
            YamlMappingNode eventNode = new YamlMappingNode();
            eventNode.Add("interrupt", "true");

            decimal tickRate = tempo * (4 / (decimal)60.0);
            decimal songLengthSeconds = songLength / tickRate;
            string timeSignature = timeSig + "/4";

            eventNode.Add("duration", songLengthSeconds.ToString());
            YamlSequenceNode effectsNode = new YamlSequenceNode();

            //Create a new effect channel for each track
            foreach (TCTrack track in tcTracks)
            {
                
                YamlSequenceNode notesNode = new YamlSequenceNode();
                
                //Add notes to node
                foreach(Note note in track.GetNotes())
                {
                    string noteEntry = "";

                    decimal noteTick = note.Tick / tickRate;
                    double actualKey = note.Key - 33;
                    double notePitch = Math.Pow(2, (actualKey - 12) / 12);

                    noteEntry += "t=" + (noteTick == 0 ? "0.0" : noteTick) + " s=" + notePitch;

                    notesNode.Add(new YamlScalarNode(noteEntry));
                }

                //Create midi map
                YamlMappingNode effectsConfigNode = new YamlMappingNode(
                        "timeSignature", timeSignature.ToString(),
                        "bpm", tempo.ToString(),
                        "pitchClasses", "12",
                        "notes", notesNode
                    );

                YamlMappingNode midiNode = new YamlMappingNode(
                    "type", "MIDI",
                    "effect", track.TrackName,
                    "config", effectsConfigNode
                    );

                effectsNode.Add(midiNode);
            }

            eventNode.Add("effects", effectsNode);

            if (loop)
            {
                sequencerNode.Add("loop", eventNode);
            }
            else
            {
                sequencerNode.Add("start", eventNode);
            }

            sequencerNode.Add("autoplay", autoplayNode);

            attatchmentsNode.Add("0", sequencerNode);

            int i = 0;
            foreach (TCTrack track in tcTracks)
            {
                YamlMappingNode soundNode = new YamlMappingNode();
                soundNode.Add("type", "SOUND");

                YamlMappingNode soundEventNode = new YamlMappingNode();
                soundEventNode.Add("key", track.Instrument.SoundEvent);
                soundEventNode.Add("category", "record");

                YamlMappingNode volumeNode = new YamlMappingNode();
                volumeNode.Add("base", ((double)track.Velocity / 100.0).ToString());
                volumeNode.Add("random", "0.0");

                soundNode.Add("position", positionNode);
                soundNode.Add("sound", soundEventNode);
                soundNode.Add("volume", volumeNode);
                soundNode.Add("names", new YamlSequenceNode(track.TrackName));

                attatchmentsNode.Add((i + 1).ToString(), soundNode);
                i++;
            }

            YamlMappingNode itemNode = new YamlMappingNode(
                    "==", "org.bukkit.inventory.ItemStack",
                    "DataVersion", "4440",
                    "id", "minecraft:note_block",
                    "count", "1",
                    "schema_version", "1"
                );

            //Add the sequencer to the model
            rootNode.Add("attachments", attatchmentsNode);
            rootNode.Add("item", itemNode);
            rootNode.Add("names", new YamlSequenceNode(songAuthor + "_" + songName));

            using (TextWriter writer = File.CreateText(path))
                outputStream.Save(writer, false);
        }

        public static V ComputeIfAbsent<K, V>(this Dictionary<K, V> dict, K key, Func<K, V> generator)
        {
            bool exists = dict.TryGetValue(key, out var value);
            if (exists)
            {
                return value;
            }
            var generated = generator(key);
            dict.Add(key, generated);
            return generated;
        }
    }
}