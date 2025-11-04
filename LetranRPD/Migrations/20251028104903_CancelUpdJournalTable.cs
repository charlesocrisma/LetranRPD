using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LetranRPD.Migrations
{
    /// <inheritdoc />
    public partial class CancelUpdJournalTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Description",
                table: "Journalss");

            migrationBuilder.DropColumn(
                name: "EISSN",
                table: "Journalss");

            migrationBuilder.DropColumn(
                name: "ISSN",
                table: "Journalss");

            migrationBuilder.DropColumn(
                name: "Publisher",
                table: "Journalss");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "Journalss",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "EISSN",
                table: "Journalss",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ISSN",
                table: "Journalss",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Publisher",
                table: "Journalss",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
