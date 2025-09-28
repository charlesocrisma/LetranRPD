using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LetranRPD.Migrations
{
    /// <inheritdoc />
    public partial class adddatavariable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "DA_Variable",
                table: "ServiceInformations",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DA_Variable",
                table: "ServiceInformations");
        }
    }
}
