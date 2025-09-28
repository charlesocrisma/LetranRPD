using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LetranRPD.Migrations
{
    /// <inheritdoc />
    public partial class Forms : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "OtherInformation",
                table: "ServiceInformations",
                newName: "LE_Index");

            migrationBuilder.AddColumn<string>(
                name: "DA_Tool",
                table: "ServiceInformations",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "LE_Pages",
                table: "ServiceInformations",
                type: "int",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DA_Tool",
                table: "ServiceInformations");

            migrationBuilder.DropColumn(
                name: "LE_Pages",
                table: "ServiceInformations");

            migrationBuilder.RenameColumn(
                name: "LE_Index",
                table: "ServiceInformations",
                newName: "OtherInformation");
        }
    }
}
