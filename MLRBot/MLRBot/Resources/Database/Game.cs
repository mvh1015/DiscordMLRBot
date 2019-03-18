using System.ComponentModel.DataAnnotations;

namespace MLRBot.Resources.Database
{
    


    public class Game
    {
        

        [Key]
        public ulong GameID { get; set; }
        public int StateOfGame { get; set; }
        public int NumberOfPitches { get; set; }
    }
}
