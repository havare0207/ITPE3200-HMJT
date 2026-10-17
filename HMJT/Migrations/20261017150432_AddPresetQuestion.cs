using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CodePursuit.Migrations
{
    /// <inheritdoc />
    public partial class AddPresetQuestion : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PresetQuestion",
                columns: table => new
                {
                    PresetQuestionId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Category = table.Column<string>(type: "TEXT", nullable: false),
                    QuestionText = table.Column<string>(type: "TEXT", nullable: false),
                    Difficulty = table.Column<int>(type: "INTEGER", nullable: false),
                    AnswerA = table.Column<string>(type: "TEXT", nullable: false),
                    AnswerB = table.Column<string>(type: "TEXT", nullable: false),
                    AnswerC = table.Column<string>(type: "TEXT", nullable: false),
                    AnswerD = table.Column<string>(type: "TEXT", nullable: false),
                    CorrectAnswer = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PresetQuestion", x => x.PresetQuestionId);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PresetQuestion");
        }
    }
}
