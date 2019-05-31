using Microsoft.EntityFrameworkCore.Migrations;

namespace AvibaWeb.Migrations
{
    public partial class MCorpData : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CorporatorData",
                columns: table => new
                {
                    CorporatorId = table.Column<string>(nullable: false),
                    OrganizationId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CorporatorData", x => x.CorporatorId);
                    table.ForeignKey(
                        name: "FK_CorporatorData_Counterparties_CorporatorId",
                        column: x => x.CorporatorId,
                        principalTable: "Counterparties",
                        principalColumn: "ITN",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CorporatorData_Organizations_OrganizationId",
                        column: x => x.OrganizationId,
                        principalTable: "Organizations",
                        principalColumn: "OrganizationId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CorporatorData_OrganizationId",
                table: "CorporatorData",
                column: "OrganizationId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CorporatorData");
        }
    }
}
