using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LetranRPD.Migrations
{
    /// <inheritdoc />
    public partial class fixrelationship2migration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ServiceProgresss_ServiceInformations_ServiceInformationServiceId",
                table: "ServiceProgresss");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ServiceProgresss",
                table: "ServiceProgresss");

            migrationBuilder.DropColumn(
                name: "ServiceId",
                table: "ServiceProgresss");

            migrationBuilder.RenameTable(
                name: "ServiceProgresss",
                newName: "ServiceProgresses");

            migrationBuilder.RenameColumn(
                name: "ServiceInformationServiceId",
                table: "ServiceProgresses",
                newName: "SIServiceId");

            migrationBuilder.RenameIndex(
                name: "IX_ServiceProgresss_ServiceInformationServiceId",
                table: "ServiceProgresses",
                newName: "IX_ServiceProgresses_SIServiceId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ServiceProgresses",
                table: "ServiceProgresses",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ServiceProgresses_ServiceInformations_SIServiceId",
                table: "ServiceProgresses",
                column: "SIServiceId",
                principalTable: "ServiceInformations",
                principalColumn: "ServiceId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ServiceProgresses_ServiceInformations_SIServiceId",
                table: "ServiceProgresses");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ServiceProgresses",
                table: "ServiceProgresses");

            migrationBuilder.RenameTable(
                name: "ServiceProgresses",
                newName: "ServiceProgresss");

            migrationBuilder.RenameColumn(
                name: "SIServiceId",
                table: "ServiceProgresss",
                newName: "ServiceInformationServiceId");

            migrationBuilder.RenameIndex(
                name: "IX_ServiceProgresses_SIServiceId",
                table: "ServiceProgresss",
                newName: "IX_ServiceProgresss_ServiceInformationServiceId");

            migrationBuilder.AddColumn<int>(
                name: "ServiceId",
                table: "ServiceProgresss",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddPrimaryKey(
                name: "PK_ServiceProgresss",
                table: "ServiceProgresss",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ServiceProgresss_ServiceInformations_ServiceInformationServiceId",
                table: "ServiceProgresss",
                column: "ServiceInformationServiceId",
                principalTable: "ServiceInformations",
                principalColumn: "ServiceId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
