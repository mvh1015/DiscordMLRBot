using System;
using System.Collections.Generic;
using System.Text;

using Discord.WebSocket;

namespace MLRBot.Resources.Settings
{
    public static class ESettings
    {
        public static string Token;
        public static ulong Owner;
        public static List<ulong> Log;
        public static List<ulong> BatterPitcher;
        public static string Version;
        public static List<ulong> Banned;
        public static string CommandPrefix;

        public static SocketTextChannel PitchingChannel;
        public static SocketTextChannel BattingChannel;
    }
}
