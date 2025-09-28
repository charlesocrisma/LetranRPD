using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LetranRPD.Migrations
{
    /// <inheritdoc />
    public partial class seventhmigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ServiceOC");

            migrationBuilder.DropTable(
                name: "ServiceType");

            migrationBuilder.CreateTable(
                name: "ServiceInformation",
                columns: table => new
                {
                    ServiceId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Author = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ContactPerson = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ContactNumber = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ResearchAdviser = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    OtherInformation = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ServiceInformation", x => x.ServiceId);
                });

            migrationBuilder.CreateTable(
                name: "ServiceProgress",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ServiceId = table.Column<int>(type: "int", nullable: false),
                    Progress1 = table.Column<bool>(type: "bit", nullable: false),
                    Progress2 = table.Column<bool>(type: "bit", nullable: false),
                    Progress3 = table.Column<bool>(type: "bit", nullable: false),
                    Progress4 = table.Column<bool>(type: "bit", nullable: false),
                    ServiceInformationServiceId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ServiceProgress", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ServiceProgress_ServiceInformation_ServiceInformationServiceId",
                        column: x => x.ServiceInformationServiceId,
                        principalTable: "ServiceInformation",
                        principalColumn: "ServiceId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ServiceProgress_ServiceInformationServiceId",
                table: "ServiceProgress",
                column: "ServiceInformationServiceId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ServiceProgress");

            migrationBuilder.DropTable(
                name: "ServiceInformation");

            migrationBuilder.CreateTable(
                name: "ServiceOC",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Author = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ContactNumber = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ContactPerson = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ResearchAdviser = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ServiceId = table.Column<int>(type: "int", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ServiceOC", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ServiceType",
                columns: table => new
                {
                    ServiceId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Author = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ContactNumber = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ContactPerson = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    OtherInformation = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ResearchAdviser = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ServiceType", x => x.ServiceId);
                });
        }
    }
}
