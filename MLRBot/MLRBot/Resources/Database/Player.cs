using System.ComponentModel.DataAnnotations;

namespace MLRBot.Resources.Database
{



    public class Player
    {

        [Key]
        public ulong UserId { get; set; }
        public int BatterType { get; set; }
        public int PitcherType { get; set; }
        public int PlateAppearances { get; set; }
        public int AtBats { get; set; }
        public int Singles { get; set; }
        public int Doubles { get; set; }
        public int Triples { get; set; }
        public int HomeRuns { get; set; }
        public int Walks { get; set; }
        public int Strikeouts { get; set; }
        public int AutoKs { get; set; }

        public int PitchesThrown { get; set; }
        public int SinglesAllowed {get; set; }
        public int DoublesAllowed { get; set; }
        public int TriplesAllowed { get; set; }
        public int HomeRunsAllowed { get; set; }
        public int WalksAllowed { get; set; }
        public int StrikeoutsGiven { get; set; }
    }
}
