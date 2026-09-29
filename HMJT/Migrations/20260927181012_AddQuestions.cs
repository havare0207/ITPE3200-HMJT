using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CodePursuit.Migrations
{
    /// <inheritdoc />
    public partial class AddQuestions : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Questions",
                columns: table => new
                {
                    QuestionId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    QuestionText = table.Column<string>(type: "TEXT", maxLength: 500, nullable: false),
                    Category = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    Difficulty = table.Column<int>(type: "INTEGER", nullable: false),
                    AnswerA = table.Column<string>(type: "TEXT", maxLength: 300, nullable: false),
                    AnswerB = table.Column<string>(type: "TEXT", maxLength: 300, nullable: false),
                    AnswerC = table.Column<string>(type: "TEXT", maxLength: 300, nullable: false),
                    AnswerD = table.Column<string>(type: "TEXT", maxLength: 300, nullable: false),
                    CorrectAnswer = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Questions", x => x.QuestionId);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Questions");
        }
    }
}
