using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LetranRPD.Migrations
{
    /// <inheritdoc />
    public partial class added_adminToStudentFiles : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "adminToStudentFiles",
                table: "ServiceProgresses",
                type: "nvarchar(max)",
                nullable: true);


        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Articles");

            migrationBuilder.DropTable(
                name: "Journals");

            migrationBuilder.DropColumn(
                name: "adminToStudentFiles",
                table: "ServiceProgresses");
        }
    }
}
