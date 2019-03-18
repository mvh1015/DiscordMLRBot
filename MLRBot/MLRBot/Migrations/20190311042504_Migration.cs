using Microsoft.EntityFrameworkCore.Migrations;

namespace MLRBot.Migrations
{
    public partial class Migration : Microsoft.EntityFrameworkCore.Migrations.Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Games",
                columns: table => new
                {
                    GameID = table.Column<ulong>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    StateOfGame = table.Column<int>(nullable: false),
                    NumberOfPitches = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Games", x => x.GameID);
                });

            migrationBuilder.CreateTable(
                name: "Participants",
                columns: table => new
                {
                    UserId = table.Column<ulong>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Role = table.Column<int>(nullable: false),
                    Singles = table.Column<int>(nullable: false),
                    Doubles = table.Column<int>(nullable: false),
                    Triples = table.Column<int>(nullable: false),
                    HomeRuns = table.Column<int>(nullable: false),
                    Strikeouts = table.Column<int>(nullable: false),
                    AutoKs = table.Column<int>(nullable: false),
                    Walks = table.Column<int>(nullable: false),
                    GuessNumber = table.Column<int>(nullable: false),
                    NumberOfGuesses = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Participants", x => x.UserId);
                });

            migrationBuilder.CreateTable(
                name: "Players",
                columns: table => new
                {
                    UserId = table.Column<ulong>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    BatterType = table.Column<int>(nullable: false),
                    PitcherType = table.Column<int>(nullable: false),
                    PlateAppearances = table.Column<int>(nullable: false),
                    AtBats = table.Column<int>(nullable: false),
                    Singles = table.Column<int>(nullable: false),
                    Doubles = table.Column<int>(nullable: false),
                    Triples = table.Column<int>(nullable: false),
                    HomeRuns = table.Column<int>(nullable: false),
                    Walks = table.Column<int>(nullable: false),
                    Strikeouts = table.Column<int>(nullable: false),
                    AutoKs = table.Column<int>(nullable: false),
                    PitchesThrown = table.Column<int>(nullable: false),
                    SinglesAllowed = table.Column<int>(nullable: false),
                    DoublesAllowed = table.Column<int>(nullable: false),
                    TriplesAllowed = table.Column<int>(nullable: false),
                    HomeRunsAllowed = table.Column<int>(nullable: false),
                    WalksAllowed = table.Column<int>(nullable: false),
                    StrikeoutsGiven = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Players", x => x.UserId);
                });

            migrationBuilder.CreateTable(
                name: "Stones",
                columns: table => new
                {
                    UserId = table.Column<ulong>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Amount = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Stones", x => x.UserId);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Games");

            migrationBuilder.DropTable(
                name: "Participants");

            migrationBuilder.DropTable(
                name: "Players");

            migrationBuilder.DropTable(
                name: "Stones");
        }
    }
}
