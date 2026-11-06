using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CodePursuit.Migrations
{
    /// <inheritdoc />
    public partial class AddNotesAndGamePresetQuestions : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "GamePresetQuestions",
                columns: table => new
                {
                    GamesGameId = table.Column<int>(type: "INTEGER", nullable: false),
                    PresetQuestionsPresetQuestionId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GamePresetQuestions", x => new { x.GamesGameId, x.PresetQuestionsPresetQuestionId });
                    table.ForeignKey(
                        name: "FK_GamePresetQuestions_Games_GamesGameId",
                        column: x => x.GamesGameId,
                        principalTable: "Games",
                        principalColumn: "GameId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_GamePresetQuestions_PresetQuestion_PresetQuestionsPresetQuestionId",
                        column: x => x.PresetQuestionsPresetQuestionId,
                        principalTable: "PresetQuestion",
                        principalColumn: "PresetQuestionId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Notes",
                columns: table => new
                {
                    NoteId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Title = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    Content = table.Column<string>(type: "TEXT", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Notes", x => x.NoteId);
                });

            migrationBuilder.CreateIndex(
                name: "IX_GamePresetQuestions_PresetQuestionsPresetQuestionId",
                table: "GamePresetQuestions",
                column: "PresetQuestionsPresetQuestionId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "GamePresetQuestions");

            migrationBuilder.DropTable(
                name: "Notes");
        }
    }
}
