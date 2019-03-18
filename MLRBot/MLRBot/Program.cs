using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

using Newtonsoft.Json;

using Discord;
using Discord.WebSocket;
using Discord.Commands;

using MLRBot.Resources.Datatypes;
using MLRBot.Resources.Settings;

namespace MLRBot
{
    class Program
    {
        private DiscordSocketClient client;
        private CommandService Commands;

        static void Main(string[] args)
        => new Program().MainAsync().GetAwaiter().GetResult();

        private async Task MainAsync()
        {
            string JSON = "";
            string SettingsLocation = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location).Replace(@"bin\Debug\netcoreapp2.1", @"Data\Settings.json");

            using (var Stream = new FileStream(SettingsLocation, FileMode.Open, FileAccess.Read))
            using (var ReadSettings = new StreamReader(Stream))
            {
                JSON = ReadSettings.ReadToEnd();
            }

            Setting Settings = JsonConvert.DeserializeObject<Setting>(JSON);
            ESettings.Banned = Settings.banned;
            ESettings.Log = Settings.log;
            ESettings.BatterPitcher = Settings.batterPitcher;
            ESettings.Owner = Settings.owner;
            ESettings.Token = Settings.token;
            ESettings.Version = Settings.version;
            ESettings.CommandPrefix = Settings.commandPrefix;



            client = new DiscordSocketClient(new DiscordSocketConfig
            {
                LogLevel = LogSeverity.Info

            });

            //help HELP heLP
            Commands = new CommandService(new CommandServiceConfig
            {
                CaseSensitiveCommands = true,
                DefaultRunMode = RunMode.Async,
                LogLevel = LogSeverity.Debug
            });

            client.MessageReceived += Client_MessageReceived;
            await Commands.AddModulesAsync(Assembly.GetEntryAssembly(),null);

            client.Ready += Client_Ready;
            client.Log += Client_Log;

            

            await client.LoginAsync(TokenType.Bot, ESettings.Token);
            await client.StartAsync();

            await Task.Delay(-1);
        }

        private async Task Client_Log(LogMessage Message)
        {
            Console.WriteLine($"{DateTime.Now} at {Message.Source}] {Message.Message} ");
            try
            {
                //DiscordSocketClient _client = new DiscordSocketClient()

                SocketGuild Guild = client.Guilds.Where(x => x.Id == ESettings.Log[0]).FirstOrDefault();
                //SocketTextChannel Channel = Guild.Channels.Where(x => x.Id == ESettings.Log[1]).FirstOrDefault() as SocketTextChannel;

                ESettings.BattingChannel = client.GetChannel(ESettings.BatterPitcher[0]) as SocketTextChannel;
                ESettings.PitchingChannel = client.GetChannel(ESettings.BatterPitcher[1]) as SocketTextChannel;

                await ESettings.PitchingChannel.SendMessageAsync($"{DateTime.Now} at {Message.Source}] {Message.Message}");
            } catch { }
            
        }

        private async Task Client_Ready()
        {
            await client.SetGameAsync("Learning How to Breathe", "");
        }

        private async Task Client_MessageReceived(SocketMessage MessageParam)
        {
            var Message = MessageParam as SocketUserMessage;
            var Context = new SocketCommandContext(client, Message);

            if (Context.Message == null || Context.Message.Content == "") return;
            if (Context.User.IsBot) return;

            int ArgPos = 0;

            if (!(Message.HasStringPrefix(ESettings.CommandPrefix, ref ArgPos) || Message.HasMentionPrefix(client.CurrentUser, ref ArgPos))) return;

            var Result = await Commands.ExecuteAsync(Context, ArgPos, null);
            if (!Result.IsSuccess)
            {
                Console.WriteLine($"{DateTime.Now} at Commands] Something went wrong with executing a command. Text: { Context.Message.Content} | Error: {Result.ErrorReason}");

            }
        }

    }
}
