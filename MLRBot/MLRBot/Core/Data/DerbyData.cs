using System;
using System.Linq;
using MLRBot.Resources.Database;

using System.Threading.Tasks;

namespace MLRBot.Core.Data
{
    public static class DerbyData
    {

        public static async Task StartDerby(ulong UserId)
        {

            using (var DbContext = new SqliteDbContext())
            {
                int count = DbContext.Games.Count();
                DbContext.Games.Add(new Game
                {
                    GameID = (ulong)count,
                    StateOfGame = 0,
                    NumberOfPitches = 0

                });

                Player CurrentPlayer = null;

                if (DbContext.Players.Where(x => x.UserId == UserId).Count() < 1)
                {
                    CurrentPlayer = new Player
                    {
                        UserId = UserId,
                        BatterType = 0,
                        PitcherType = 0,
                        PlateAppearances = 0,
                        AtBats = 0,
                        Singles = 0,
                        Doubles = 0,
                        Triples = 0,
                        HomeRuns = 0,
                        Strikeouts = 0,
                        AutoKs = 0,
                        PitchesThrown = 0,
                        SinglesAllowed = 0,
                        DoublesAllowed = 0,
                        TriplesAllowed = 0,
                        HomeRunsAllowed = 0,
                        WalksAllowed = 0,
                        StrikeoutsGiven = 0
                    };

                    //The user doesn't have a row yet, create one for the user.
                    DbContext.Players.Add(CurrentPlayer);
                }
                else
                {
                    CurrentPlayer = DbContext.Players.Where(x => x.UserId == UserId).FirstOrDefault();
                }

                DbContext.Participants.Add(new Participant
                {
                    UserId = UserId,
                    Role = 1,
                    Singles = 0,
                    Doubles = 0,
                    Triples = 0,
                    HomeRuns = 0,
                    Strikeouts = 0,
                    AutoKs = 0,
                    GuessNumber = 0,
                    NumberOfGuesses = 0
                });
                await DbContext.SaveChangesAsync();
            }

        }

        public static async Task JoinDerby(ulong UserId)
        {

            using (var DbContext = new SqliteDbContext())
            {

                Player CurrentPlayer = null;

                if (DbContext.Players.Where(x => x.UserId == UserId).Count() < 1)
                {
                    CurrentPlayer = new Player
                    {
                        UserId = UserId,
                        BatterType = 0,
                        PitcherType = 0,
                        PlateAppearances = 0,
                        AtBats = 0,
                        Singles = 0,
                        Doubles = 0,
                        Triples = 0,
                        HomeRuns = 0,
                        Strikeouts = 0,
                        AutoKs = 0,
                        PitchesThrown = 0,
                        SinglesAllowed = 0,
                        DoublesAllowed = 0,
                        TriplesAllowed = 0,
                        HomeRunsAllowed = 0,
                        WalksAllowed = 0,
                        StrikeoutsGiven = 0
                    };

                    //The user doesn't have a row yet, create one for the user.
                    DbContext.Players.Add(CurrentPlayer);
                }
                else
                {
                    CurrentPlayer = DbContext.Players.Where(x => x.UserId == UserId).FirstOrDefault();
                }

                DbContext.Participants.Add(new Participant
                {
                    UserId = UserId,
                    Role = 0,
                    Singles = 0,
                    Doubles = 0,
                    Triples = 0,
                    HomeRuns = 0,
                    Strikeouts = 0,
                    AutoKs = 0,
                    GuessNumber = 0,
                    NumberOfGuesses = 0
                });
                await DbContext.SaveChangesAsync();
            }

        }

        public static async Task Pitch(ulong UserId, int pitch)
        {

            using (var DbContext = new SqliteDbContext())
            {

                foreach (Participant p in DbContext.Participants)
                {
                    p.GuessNumber = -1;
                }

                Participant CurrentPitcher = DbContext.Participants.Where(x => x.UserId == UserId).FirstOrDefault();
                CurrentPitcher.GuessNumber = pitch;
                DbContext.Participants.Update(CurrentPitcher);

                

                Game game = DbContext.Games.Where(x => x.StateOfGame != 3).FirstOrDefault();
                game.NumberOfPitches++;
                game.StateOfGame = 2;
                DbContext.Games.Update(game);


                await DbContext.SaveChangesAsync();
            }

        }

        public static async Task Swing(ulong UserId, int swing)
        {

            using (var DbContext = new SqliteDbContext())
            {

                Participant CurrentSwinger = DbContext.Participants.Where(x => x.UserId == UserId).FirstOrDefault();
                CurrentSwinger.GuessNumber = swing;
                DbContext.Participants.Update(CurrentSwinger);


                await DbContext.SaveChangesAsync();
            }

        }

        public static async Task CheckIfLastSwing()
        {
            using (var DbContext = new SqliteDbContext())
            {
                foreach (Participant p in DbContext.Participants)
                {
                    if (p.GuessNumber == -1)
                    {
                        return;
                    }
                }

                Game game = DbContext.Games.Where(x => x.StateOfGame != 3).FirstOrDefault();

                if (game.NumberOfPitches >= 5)
                {
                    game.StateOfGame = 3;
                } else
                {
                    game.StateOfGame = 1;
                }

                
                DbContext.Games.Update(game);
                await DbContext.SaveChangesAsync();
                return;
            }
        }


    }
}
