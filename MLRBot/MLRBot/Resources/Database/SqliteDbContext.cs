using Microsoft.EntityFrameworkCore;

using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;

namespace MLRBot.Resources.Database
{
    public class SqliteDbContext : DbContext
    {
        public DbSet<Stone> Stones { get; set; }
        public DbSet<Participant> Participants { get; set; }
        public DbSet<Game> Games { get; set; }
        public DbSet<Player> Players { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder Options)
        {

            string DbLocation = Assembly.GetEntryAssembly().Location.Replace(@"bin\Debug\netcoreapp2.1\MLRBot.dll", @"Data\");
            Options.UseSqlite($"Data Source={DbLocation}MLRBot.dllDatabase.sqlite");


        }
    }
}
