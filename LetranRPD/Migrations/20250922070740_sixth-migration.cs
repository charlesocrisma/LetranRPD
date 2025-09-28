using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LetranRPD.Migrations
{
    /// <inheritdoc />
    public partial class sixthmigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Id",
                table: "ServiceType",
                newName: "ServiceId");

            migrationBuilder.AddColumn<string>(
                name: "Author",
                table: "ServiceType",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ContactNumber",
                table: "ServiceType",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ContactPerson",
                table: "ServiceType",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Email",
                table: "ServiceType",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "OtherInformation",
                table: "ServiceType",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ResearchAdviser",
                table: "ServiceType",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Title",
                table: "ServiceType",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "ServiceId",
                table: "ServiceOC",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Author",
                table: "ServiceType");

            migrationBuilder.DropColumn(
                name: "ContactNumber",
                table: "ServiceType");

            migrationBuilder.DropColumn(
                name: "ContactPerson",
                table: "ServiceType");

            migrationBuilder.DropColumn(
                name: "Email",
                table: "ServiceType");

            migrationBuilder.DropColumn(
                name: "OtherInformation",
                table: "ServiceType");

            migrationBuilder.DropColumn(
                name: "ResearchAdviser",
                table: "ServiceType");

            migrationBuilder.DropColumn(
                name: "Title",
                table: "ServiceType");

            migrationBuilder.DropColumn(
                name: "ServiceId",
                table: "ServiceOC");

            migrationBuilder.RenameColumn(
                name: "ServiceId",
                table: "ServiceType",
                newName: "Id");
        }
    }
}
