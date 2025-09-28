using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LetranRPD.Migrations
{
    /// <inheritdoc />
    public partial class fixrelationshipmigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ServiceProgress_ServiceInformation_ServiceInformationServiceId",
                table: "ServiceProgress");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ServiceProgress",
                table: "ServiceProgress");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ServiceInformation",
                table: "ServiceInformation");

            migrationBuilder.RenameTable(
                name: "ServiceProgress",
                newName: "ServiceProgresss");

            migrationBuilder.RenameTable(
                name: "ServiceInformation",
                newName: "ServiceInformations");

            migrationBuilder.RenameIndex(
                name: "IX_ServiceProgress_ServiceInformationServiceId",
                table: "ServiceProgresss",
                newName: "IX_ServiceProgresss_ServiceInformationServiceId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ServiceProgresss",
                table: "ServiceProgresss",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ServiceInformations",
                table: "ServiceInformations",
                column: "ServiceId");

            migrationBuilder.AddForeignKey(
                name: "FK_ServiceProgresss_ServiceInformations_ServiceInformationServiceId",
                table: "ServiceProgresss",
                column: "ServiceInformationServiceId",
                principalTable: "ServiceInformations",
                principalColumn: "ServiceId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ServiceProgresss_ServiceInformations_ServiceInformationServiceId",
                table: "ServiceProgresss");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ServiceProgresss",
                table: "ServiceProgresss");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ServiceInformations",
                table: "ServiceInformations");

            migrationBuilder.RenameTable(
                name: "ServiceProgresss",
                newName: "ServiceProgress");

            migrationBuilder.RenameTable(
                name: "ServiceInformations",
                newName: "ServiceInformation");

            migrationBuilder.RenameIndex(
                name: "IX_ServiceProgresss_ServiceInformationServiceId",
                table: "ServiceProgress",
                newName: "IX_ServiceProgress_ServiceInformationServiceId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ServiceProgress",
                table: "ServiceProgress",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ServiceInformation",
                table: "ServiceInformation",
                column: "ServiceId");

            migrationBuilder.AddForeignKey(
                name: "FK_ServiceProgress_ServiceInformation_ServiceInformationServiceId",
                table: "ServiceProgress",
                column: "ServiceInformationServiceId",
                principalTable: "ServiceInformation",
                principalColumn: "ServiceId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
