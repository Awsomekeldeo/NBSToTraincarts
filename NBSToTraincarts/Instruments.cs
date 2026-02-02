using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NBSToTraincarts
{
    internal class Instruments
    {
        public static readonly Instrument HARP = new Instrument(0, "Harp", "minecraft:block.note_block.harp");
        public static readonly Instrument BASS = new Instrument(1, "Bass", "minecraft:block.note_block.bass");
        public static readonly Instrument BASS_DRUM = new Instrument(2, "Bass Drum", "minecraft:block.note_block.basedrum");
        public static readonly Instrument SNARE_DRUM = new Instrument(3, "Snare Drum", "minecraft:block.note_block.snare");
        public static readonly Instrument CLICK = new Instrument(4, "Click", "minecraft:block.note_block.hat");
        public static readonly Instrument GUITAR = new Instrument(5, "Guitar", "minecraft:block.note_block.guitar");
        public static readonly Instrument FLUTE = new Instrument(6, "Flute", "minecraft:block.note_block.flute");
        public static readonly Instrument BELL = new Instrument(7, "Bell", "minecraft:block.note_block.bell");
        public static readonly Instrument CHIME = new Instrument(8, "Chime", "minecraft:block.note_block.chime");
        public static readonly Instrument XYLOPHONE = new Instrument(9, "Xylophone", "minecraft:block.note_block.xylophone");
        public static readonly Instrument IRON_XYLOPHONE = new Instrument(10, "Iron Xylophone", "minecraft:block.note_block.iron_xylophone");
        public static readonly Instrument COWBELL = new Instrument(11, "Cowbell", "minecraft:block.note_block.cowbell");
        public static readonly Instrument DIDGERIDOO = new Instrument(12, "Didgeridoo", "minecraft:block.note_block.didgeridoo");
        public static readonly Instrument BIT = new Instrument(13, "Bit", "minecraft:block.note_block.bit");
        public static readonly Instrument BANJO = new Instrument(14, "Banjo", "minecraft:block.note_block.banjo");
        public static readonly Instrument PLING = new Instrument(15, "Pling", "minecraft:block.note_block.pling");

        /**
         * <summary>
         * Returns a vanilla instrument based on it's ID in Note Block Studio.
         * </summary>
         * <exception cref="ArgumentOutOfRangeException">
         * Thrown when the specified id is invalid.
         * </exception>
         * <returns>The instrument cooresponding to the specified ID.</returns>
         */
        public static Instrument GetVanillaInstrumentById(int id)
        {
            switch(id)
            {
                case 0:
                    return Instruments.HARP;
                case 1:
                    return Instruments.BASS;
                case 2:
                    return Instruments.BASS_DRUM;
                case 3:
                    return Instruments.SNARE_DRUM;
                case 4:
                    return Instruments.CLICK;
                case 5:
                    return Instruments.GUITAR;
                case 6:
                    return Instruments.FLUTE;
                case 7:
                    return Instruments.BELL;
                case 8:
                    return Instruments.CHIME;
                case 9:
                    return Instruments.XYLOPHONE;
                case 10:
                    return Instruments.IRON_XYLOPHONE;
                case 11:
                    return Instruments.COWBELL;
                case 12:
                    return Instruments.DIDGERIDOO;
                case 13:
                    return Instruments.BIT;
                case 14:
                    return Instruments.BANJO;
                case 15:
                    return Instruments.PLING;
                default:
                    throw new ArgumentOutOfRangeException("ID " + id + "is outside the valid range.");
            }
        }
    }
}
