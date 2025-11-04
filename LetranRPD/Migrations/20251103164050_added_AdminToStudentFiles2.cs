using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LetranRPD.Migrations
{
    /// <inheritdoc />
    public partial class added_AdminToStudentFiles2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "adminToStudentFiles",
                table: "ServiceProgresses",
                newName: "AdminToStudentFiles");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "AdminToStudentFiles",
                table: "ServiceProgresses",
                newName: "adminToStudentFiles");
        }
    }
}
