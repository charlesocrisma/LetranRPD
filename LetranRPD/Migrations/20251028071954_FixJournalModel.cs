using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LetranRPD.Migrations
{
    /// <inheritdoc />
    public partial class FixJournalModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Articless_Journalss_JournalModelId",
                table: "Articless");

            migrationBuilder.DropIndex(
                name: "IX_Articless_JournalModelId",
                table: "Articless");

            migrationBuilder.DropColumn(
                name: "JournalModelId",
                table: "Articless");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "JournalModelId",
                table: "Articless",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Articless_JournalModelId",
                table: "Articless",
                column: "JournalModelId");

            migrationBuilder.AddForeignKey(
                name: "FK_Articless_Journalss_JournalModelId",
                table: "Articless",
                column: "JournalModelId",
                principalTable: "Journalss",
                principalColumn: "Id");
        }
    }
}
