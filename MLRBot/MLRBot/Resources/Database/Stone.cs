using System.ComponentModel.DataAnnotations;

namespace MLRBot.Resources.Database
{
    
    public class Stone
    {
        [Key]
        public ulong UserId { get; set; }
        public int Amount { get; set; }
    }
}
