using Microsoft.EntityFrameworkCore.Migrations;

namespace AvibaWeb.Migrations
{
    public partial class MFinancialAccountOperation_FactualCounterparty : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "FactualCounterpartyId",
                table: "FinancialAccountOperations",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_FinancialAccountOperations_FactualCounterpartyId",
                table: "FinancialAccountOperations",
                column: "FactualCounterpartyId");

            migrationBuilder.AddForeignKey(
                name: "FK_FinancialAccountOperations_Counterparties_FactualCounterpartyId",
                table: "FinancialAccountOperations",
                column: "FactualCounterpartyId",
                principalTable: "Counterparties",
                principalColumn: "ITN",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_FinancialAccountOperations_Counterparties_FactualCounterpartyId",
                table: "FinancialAccountOperations");

            migrationBuilder.DropIndex(
                name: "IX_FinancialAccountOperations_FactualCounterpartyId",
                table: "FinancialAccountOperations");

            migrationBuilder.DropColumn(
                name: "FactualCounterpartyId",
                table: "FinancialAccountOperations");
        }
    }
}
