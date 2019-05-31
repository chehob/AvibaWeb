using Microsoft.EntityFrameworkCore.Migrations;

namespace AvibaWeb.Migrations
{
    public partial class MCorporatorFeeRateForeignKeys : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "OrganizationId",
                table: "CorporatorFeeRates",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_CorporatorFeeRates_OrganizationId",
                table: "CorporatorFeeRates",
                column: "OrganizationId");

            migrationBuilder.AddForeignKey(
                name: "FK_CorporatorFeeRates_Counterparties_ITN",
                table: "CorporatorFeeRates",
                column: "ITN",
                principalTable: "Counterparties",
                principalColumn: "ITN",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_CorporatorFeeRates_Organizations_OrganizationId",
                table: "CorporatorFeeRates",
                column: "OrganizationId",
                principalTable: "Organizations",
                principalColumn: "OrganizationId",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CorporatorFeeRates_Counterparties_ITN",
                table: "CorporatorFeeRates");

            migrationBuilder.DropForeignKey(
                name: "FK_CorporatorFeeRates_Organizations_OrganizationId",
                table: "CorporatorFeeRates");

            migrationBuilder.DropIndex(
                name: "IX_CorporatorFeeRates_OrganizationId",
                table: "CorporatorFeeRates");

            migrationBuilder.DropColumn(
                name: "OrganizationId",
                table: "CorporatorFeeRates");
        }
    }
}
