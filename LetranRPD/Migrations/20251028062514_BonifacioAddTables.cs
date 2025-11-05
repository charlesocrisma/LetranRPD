using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LetranRPD.Migrations
{
    /// <inheritdoc />
    public partial class BonifacioAddTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Articles_Journals_JournalModelId",
                table: "Articles");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Journals",
                table: "Journals");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Articles",
                table: "Articles");

            migrationBuilder.RenameTable(
                name: "Journals",
                newName: "Journalss");

            migrationBuilder.RenameTable(
                name: "Articles",
                newName: "Articless");

            migrationBuilder.RenameIndex(
                name: "IX_Articles_JournalModelId",
                table: "Articless",
                newName: "IX_Articless_JournalModelId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Journalss",
                table: "Journalss",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Articless",
                table: "Articless",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Articless_Journalss_JournalModelId",
                table: "Articless",
                column: "JournalModelId",
                principalTable: "Journalss",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Articless_Journalss_JournalModelId",
                table: "Articless");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Journalss",
                table: "Journalss");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Articless",
                table: "Articless");

            migrationBuilder.RenameTable(
                name: "Journals",
                newName: "Journalss");

            migrationBuilder.RenameTable(
                name: "Articles",
                newName: "Articless");

            migrationBuilder.RenameIndex(
                name: "IX_Articless_JournalModelId",
                table: "Articles",
                newName: "IX_Articles_JournalModelId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Journals",
                table: "Journals",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Articles",
                table: "Articles",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Articles_Journals_JournalModelId",
                table: "Articles",
                column: "JournalModelId",
                principalTable: "Journals",
                principalColumn: "Id");
        }
    }
}
