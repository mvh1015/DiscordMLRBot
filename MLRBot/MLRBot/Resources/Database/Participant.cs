using System.ComponentModel.DataAnnotations;

namespace MLRBot.Resources.Database
{
    


    public class Participant
    {

        [Key]
        public ulong UserId { get; set; }

        public int Role { get; set; }       //pitcher or batter
        public int Singles { get; set; }
        public int Doubles { get; set; }
        public int Triples { get; set; }
        public int HomeRuns { get; set; }
        public int Strikeouts { get; set; }
        public int AutoKs { get; set; }
        public int Walks { get; set; }
        public int GuessNumber { get; set; }    //578 or 410
        public int NumberOfGuesses { get; set; } //number of swings or pitches
    }
}
