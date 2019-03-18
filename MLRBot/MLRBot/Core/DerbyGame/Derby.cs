using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;

using System.Linq;

using MLRBot.Resources.Database;

using MLRBot.Core.Data;
using MLRBot.Resources.Settings;
using Game = MLRBot.Resources.Database.Game;

namespace MLRBot.Core.DerbyGame
{
    public class Derby : ModuleBase<SocketCommandContext>
    {
        enum GameState { InvitingPlayers, WaitingForPitch, WaitingForSwings, AboutToFinish, Concluded }


        [Command("start"), Alias("startderby"), Summary("Starts Homerun Derby Game")]
        public async Task StartDerby()
        {
            if (Context.Channel.Id != ESettings.PitchingChannel.Id)
            {
                await Context.Channel.SendMessageAsync($"{Context.User}, You can only use this command from the designated pitching channel");
                return;
            }

            await ClearGames();

            using (var DbContext = new SqliteDbContext())
            {
                //if the last game exists, and it is not over.
                if (DbContext.Games.Where(x => x.StateOfGame != 4).FirstOrDefault() != null)
                {
                    await Context.Channel.SendMessageAsync($"{Context.User}, you have have to wait for the other game to finish!");
                    return;
                }
            }

            await Data.DerbyData.StartDerby(Context.User.Id);

            await Context.Channel.SendMessageAsync($"You have started a homerun derby. Tell your friends to join in from the designated batting channel!");

            await ESettings.BattingChannel.SendMessageAsync($"{Context.User} has started a game of Derby! Type in " + ESettings.CommandPrefix + "join to Play!");

        }

        [Command("stats"), Alias("starts"), Summary("GetPlayerStats")]
        public async Task Stats(IUser User = null)
        {
            Player p;
            using (var DbContext = new SqliteDbContext())
            {
                if (DbContext.Players.Where(x => x.UserId == User.Id).Count() < 1)
                {
                    await Context.Channel.SendMessageAsync($"Player not Found.");
                    return;
                }
                    
                p = DbContext.Players.Where(X => X.UserId == User.Id).FirstOrDefault();
            }

            //Embed the swings 

            EmbedBuilder Embed = new EmbedBuilder();
            Embed.WithAuthor("Stats for " + User.Username, User.GetAvatarUrl());

            Embed.WithColor(40, 200, 150);

            Embed.AddField("Batting Stats--------------------------------", "||||||||||||||||||||", false);
            Embed.AddField("At Bats: ", p.AtBats, true);
            Embed.AddField("Plate Appearances: ", p.PlateAppearances, true);
            Embed.AddField("Homeruns: ", p.HomeRuns, true);
            Embed.AddField("Triples: ", p.Triples, true);
            Embed.AddField("Doubles: ", p.Doubles, true);
            Embed.AddField("Singles: ", p.Singles, true);
            Embed.AddField("Walks: ", p.Walks, true);
            Embed.AddField("Strikeouts", p.Strikeouts, true);
            Embed.AddField("Pitching Stats--------------------------------", "|||||||||||||||||||", false);
            Embed.AddField("Batters Faced", p.PitchesThrown, true);
            Embed.AddField("Homeruns", p.HomeRunsAllowed, true);
            Embed.AddField("Triples: ", p.TriplesAllowed, true);
            Embed.AddField("Doubles: ", p.DoublesAllowed, true);
            Embed.AddField("Singles: ", p.SinglesAllowed, true);
            Embed.AddField("Walks: ", p.WalksAllowed, true);
            Embed.AddField("Strikeouts", p.StrikeoutsGiven, true);

            await Context.Channel.SendMessageAsync("", false, Embed.Build());
        }

        [Command("clear"), Summary("Clears Database")]
        public async Task ClearGames()
        {
            using (var DbContext = new SqliteDbContext())
            {
                DbContext.Games.RemoveRange(DbContext.Games.Where(x => x.GameID >= 0));
                DbContext.Participants.RemoveRange(DbContext.Participants.Where(x => x.AutoKs >= 0));
                await DbContext.SaveChangesAsync();

            }

            //await Context.Channel.SendMessageAsync($"{Context.User} has cleared all games");
        }


        [Command("join"), Summary("Batters can join")]
        public async Task JoinDerby()
        {
            if (Context.Channel.Id != ESettings.BattingChannel.Id)
            {
                await Context.Channel.SendMessageAsync($"{Context.User}, You can only use this command from the designated batting channel");
                return;
            }

            using (var DbContext = new SqliteDbContext())
            {
                GameState gameState = (GameState)(DbContext.Games.Where(x => x.StateOfGame != 4).Select(x => x.StateOfGame).FirstOrDefault());

                //if the last game exists, and it is in phase 1
                if (!(DbContext.Games.Count() > 0 && gameState == GameState.InvitingPlayers))
                {
                    if (gameState == GameState.WaitingForPitch || gameState == GameState.WaitingForSwings)
                    {
                        await Context.Channel.SendMessageAsync($"Please wait until the current game to finish.");

                        return;
                    } else
                    {
                        await Context.Channel.SendMessageAsync($"No Games have started yet. Press {ESettings.CommandPrefix}start to begin a derby as a pitcher.");
                        return;
                    }
 
                }

                if (DbContext.Participants.Where(x => x.UserId == Context.User.Id).Count() >= 1)
                {
                    await Context.Channel.SendMessageAsync($"{Context.User}, you have have already joined the game!");
                    return;
                }
            }

            await Data.DerbyData.JoinDerby(Context.User.Id);


            await Context.Channel.SendMessageAsync($"{Context.User} has joined the derby!");
            await ESettings.PitchingChannel.SendMessageAsync($"{Context.User} has joined the derby!");
            await ESettings.PitchingChannel.SendMessageAsync($"Use {ESettings.CommandPrefix}pitch # to begin pitching, or wait for more batters!");

        }

        [Command("pitch"), Summary("The Pitch is thrown")]
        public async Task Pitch(int pitch)
        {
            if (Context.Channel.Id != ESettings.PitchingChannel.Id)
            {
                await Context.Channel.SendMessageAsync($"{Context.User}, You can only use this command from the designated pitching channel");
                return;
            }

            using (var DbContext = new SqliteDbContext())
            {
                int role = DbContext.Participants.Where(x => x.UserId == Context.User.Id).Select(x => x.Role).FirstOrDefault();

                if (role != 1)
                {
                    await Context.Channel.SendMessageAsync($"You are not the pitcher, you cannot pitch.");
                    return;
                }

                //ulong pitcher = DbContext.Participants.Where(x => x.Player.UserId == Context.User.Id).FirstOrDefault();

                if (DbContext.Games.Where(x => x.StateOfGame != 4).FirstOrDefault() == null)
                {
                    await Context.Channel.SendMessageAsync($"No Games have started yet. Press {ESettings.CommandPrefix}start to begin a derby as a pitcher.");
                    return;
                }

                GameState gameState = (GameState)(DbContext.Games.Where(x => x.StateOfGame != 4).Select(x => x.StateOfGame).FirstOrDefault());


                //Check if there is more than one player playing
                if (DbContext.Participants.Count() <= 1)
                {
                    await Context.Channel.SendMessageAsync($"Please wait. You need atleast one batter to play this derby game.");
                    return;
                }

                //Check if we are in the right state of the game
                if (gameState == GameState.WaitingForSwings)
                {
                    await Context.Channel.SendMessageAsync($"Please wait. Batters are still swinging.");
                    return;
                }

                

            }

            if (pitch <= 0 || pitch > 1000)
            {
                await Context.Channel.SendMessageAsync($"Your pitch must be within 1-1000");
                return;
            }

            await Data.DerbyData.Pitch(Context.User.Id, pitch);

            await ESettings.BattingChannel.SendMessageAsync($"The pitch is in. Batters can use {ESettings.CommandPrefix}swing # to swing.");
            await Context.Channel.SendMessageAsync("Your pitch is submitted.");
        }

        [Command("swing"), Summary("The Swing is swung")]
        public async Task Swing(int swingNumber)
        {
            if (Context.Channel.Id != ESettings.BattingChannel.Id)
            {
                await Context.Channel.SendMessageAsync($"{Context.User}, You can only use this command from the designated batting channel");
                return;
            }

            using (var DbContext = new SqliteDbContext())
            {
                int role = DbContext.Participants.Where(x => x.UserId == Context.User.Id).Select(x => x.Role).FirstOrDefault();

                if (role != 0)
                {
                    await Context.Channel.SendMessageAsync($"You are not the batter, you cannot swing.");
                    return;
                }


                GameState gameState = (GameState)(DbContext.Games.Where(x => x.StateOfGame != 4).Select(x => x.StateOfGame).FirstOrDefault());

                //Check if in the right state of the game
                if (DbContext.Games.Count() == 0 || gameState == GameState.Concluded || gameState == GameState.AboutToFinish)
                {
                    await Context.Channel.SendMessageAsync($"No Games have started yet. Press {ESettings.CommandPrefix}start to begin a derby as a pitcher.");
                    return;
                }

                //Check if we are in the right state of the game
                if (gameState == GameState.WaitingForPitch)
                {
                    await Context.Channel.SendMessageAsync($"Please wait. Pitcher hasn't submitted a pitch yet");
                    return;
                }



            }

            if (swingNumber <= 0 || swingNumber > 1000)
            {
                await Context.Channel.SendMessageAsync($"Your swing must be within 1-1000");
                return;
            }

            await Data.DerbyData.Swing(Context.User.Id, swingNumber);

            await Context.Channel.SendMessageAsync($"{Context.User}, your swing is submitted.");
            await ESettings.PitchingChannel.SendMessageAsync($"{Context.User.Username}'s swing has been submitted!");

            await Data.DerbyData.CheckIfLastSwing();

            using (var DbContext = new SqliteDbContext())
            {
                Game game = DbContext.Games.Where(x => x.StateOfGame != 4).FirstOrDefault();

                if ((GameState)game.StateOfGame == GameState.AboutToFinish || ((GameState)game.StateOfGame == GameState.WaitingForPitch))
                {
                    //Embed the swings 

                    EmbedBuilder Embed = new EmbedBuilder();
                    Embed.WithAuthor("Results");


                    Embed.WithColor(40, 200, 150);
                    Embed.WithFooter("Pitch " + game.NumberOfPitches + " of 5.");

                    int pitch = DbContext.Participants.Where(x => x.Role == 1).Select(x => x.GuessNumber).FirstOrDefault();
                    ulong pitcherID = DbContext.Participants.Where(x => x.Role == 1).Select(x => x.UserId).FirstOrDefault();

                    Embed.WithDescription("The pitch was: " + pitch + ".");

                    

                    foreach (Participant p in DbContext.Participants)
                    {
                        if (p.UserId == pitcherID)
                            continue;

                        var users = Context.Guild.Users;

                        foreach (SocketGuildUser user in Context.Guild.Users)
                        {
                            if (user.Id == p.UserId)
                            {
                                int max = Math.Max(pitch, p.GuessNumber);
                                int min = Math.Min(pitch, p.GuessNumber);
                                int result = Math.Min(max - min, 1000 - max + min);
                                Embed.WithImageUrl(user.GetAvatarUrl());
                                Embed.AddField(user.Username, "Swing:" + p.GuessNumber + " | Difference: " + result + " | Result: " +
                                                FindSwingOutcome(result, p.UserId, pitcherID, DbContext) + " | OPS: " + FindOPS(p.UserId, DbContext));
                                break;
                            }
                        }
                        
                    }

                    await DbContext.SaveChangesAsync();

                    await Context.Channel.SendMessageAsync("", false, Embed.Build());
                    await ESettings.PitchingChannel.SendMessageAsync("", false, Embed.Build());

                    if ((GameState)game.StateOfGame == GameState.AboutToFinish)
                    {
                        Embed = new EmbedBuilder();
                        Embed.WithAuthor("Final Score");

                        foreach (Participant p in DbContext.Participants)
                        {
                            if (p.UserId == pitcherID)
                                continue;

                            var users = Context.Guild.Users;

                            foreach (SocketGuildUser user in Context.Guild.Users)
                            {
                                if (user.Id == p.UserId)
                                {
                                    Embed.AddField(user.Username, "OPS: " + FindOPS(p.UserId, DbContext));
                                    break;
                                }
                            }

                        }

                        await Context.Channel.SendMessageAsync("", false, Embed.Build());
                        await ESettings.PitchingChannel.SendMessageAsync("", false, Embed.Build());
                        await ClearGames();
                    } else
                    {
                        await Context.Channel.SendMessageAsync($"Please wait for the pitcher to submit their next pitch.");
                        await ESettings.PitchingChannel.SendMessageAsync($"Pitcher, Submit your pitch.");
                    }

                }
            }
            

        }

        string FindSwingOutcome(int difference, ulong id, ulong pid, SqliteDbContext db)
        {
            Player player = db.Players.Where(x => x.UserId == id).FirstOrDefault();
            Player pitcher = db.Players.Where(x => x.UserId == pid).FirstOrDefault();
            Participant participantPlayer = db.Participants.Where(x => x.UserId == id).FirstOrDefault();
            string result = "";

            player.AtBats++;
            participantPlayer.NumberOfGuesses++;
            player.PlateAppearances++;
            pitcher.PitchesThrown++;

            if (difference <= 24)
            {
                participantPlayer.HomeRuns++;
                player.HomeRuns++;
                pitcher.HomeRunsAllowed++;
                result = "Homerun!";
            }
            else if (difference <= 29)
            {
                participantPlayer.Triples++;
                player.Triples++;
                pitcher.TriplesAllowed++;
                result = "Triple!";
            }
            else if (difference <= 64)
            {
                pitcher.DoublesAllowed++;
                player.Doubles++;
                participantPlayer.Doubles++;
                result = "Double!";
            }
            else if (difference <= 137)
            {
                pitcher.SinglesAllowed++;
                player.Singles++;
                participantPlayer.Singles++;
                result = "Single!";
            }
            else if (difference <= 177)
            {
                pitcher.WalksAllowed++;
                player.AtBats--;
                player.Walks++;
                participantPlayer.Walks++;
                result = "Walk!";
            }
            else if (difference <= 284)
            {
                result = "Flyout!";
            }
            else if (difference <= 377)
            {
                pitcher.StrikeoutsGiven++;
                player.Strikeouts++;
                player.Strikeouts++;
                result = "Strikeout!";
            }
            else if (difference <= 399)
            {
                result = "Popout!";
            }
            else if (difference <= 450)
            {
                result = "Groundout to the Right";
            }
            else if (difference <= 500)
            {
                result = "Groundout to the Left";
            }

            db.Players.Update(player);
            db.Players.Update(pitcher);
            

            return result;
        }

        float FindOPS(ulong id, SqliteDbContext db)
        {
            Participant player = db.Participants.Where(x => x.UserId == id).FirstOrDefault();

            float SLG = (float)((player.HomeRuns * 4) + (player.Triples * 3) + (player.Doubles * 2) + (player.Singles) * player.Walks) / (float)player.NumberOfGuesses;

            float OBP = (float)(player.HomeRuns + player.Triples + player.Doubles + player.Singles + player.Walks) / (float)(player.NumberOfGuesses + player.Walks);

            return (float)Math.Round((SLG + OBP),3);
        }


    }
}
