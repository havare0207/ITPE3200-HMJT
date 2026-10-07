using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CodePursuit.Migrations
{
    /// <inheritdoc />
    public partial class ConnectGameAndQuestion : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "GameId",
                table: "Questions",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Questions_GameId",
                table: "Questions",
                column: "GameId");

            migrationBuilder.AddForeignKey(
                name: "FK_Questions_Games_GameId",
                table: "Questions",
                column: "GameId",
                principalTable: "Games",
                principalColumn: "GameId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Questions_Games_GameId",
                table: "Questions");

            migrationBuilder.DropIndex(
                name: "IX_Questions_GameId",
                table: "Questions");

            migrationBuilder.DropColumn(
                name: "GameId",
                table: "Questions");
        }
    }
}
