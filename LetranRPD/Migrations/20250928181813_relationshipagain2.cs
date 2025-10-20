using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LetranRPD.Migrations
{
    /// <inheritdoc />
    public partial class relationshipagain2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ServiceProgresses_ServiceInformations_SIServiceId",
                table: "ServiceProgresses");

            migrationBuilder.DropIndex(
                name: "IX_ServiceProgresses_SIServiceId",
                table: "ServiceProgresses");

            migrationBuilder.RenameColumn(
                name: "SIServiceId",
                table: "ServiceProgresses",
                newName: "ServiceId");

            migrationBuilder.CreateIndex(
                name: "IX_ServiceProgresses_ServiceId",
                table: "ServiceProgresses",
                column: "ServiceId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_ServiceProgresses_ServiceInformations_ServiceId",
                table: "ServiceProgresses",
                column: "ServiceId",
                principalTable: "ServiceInformations",
                principalColumn: "ServiceId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ServiceProgresses_ServiceInformations_ServiceId",
                table: "ServiceProgresses");

            migrationBuilder.DropIndex(
                name: "IX_ServiceProgresses_ServiceId",
                table: "ServiceProgresses");

            migrationBuilder.RenameColumn(
                name: "ServiceId",
                table: "ServiceProgresses",
                newName: "SIServiceId");

            migrationBuilder.CreateIndex(
                name: "IX_ServiceProgresses_SIServiceId",
                table: "ServiceProgresses",
                column: "SIServiceId");

            migrationBuilder.AddForeignKey(
                name: "FK_ServiceProgresses_ServiceInformations_SIServiceId",
                table: "ServiceProgresses",
                column: "SIServiceId",
                principalTable: "ServiceInformations",
                principalColumn: "ServiceId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
