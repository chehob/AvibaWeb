using Microsoft.EntityFrameworkCore.Migrations;

namespace AvibaWeb.Migrations
{
    public partial class MFinancialAccountOperationTransfer : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "TransferFinancialAccountId",
                table: "FinancialAccountOperations",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_FinancialAccountOperations_TransferFinancialAccountId",
                table: "FinancialAccountOperations",
                column: "TransferFinancialAccountId");

            migrationBuilder.AddForeignKey(
                name: "FK_FinancialAccountOperations_FinancialAccounts_TransferFinancialAccountId",
                table: "FinancialAccountOperations",
                column: "TransferFinancialAccountId",
                principalTable: "FinancialAccounts",
                principalColumn: "FinancialAccountId",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_FinancialAccountOperations_FinancialAccounts_TransferFinancialAccountId",
                table: "FinancialAccountOperations");

            migrationBuilder.DropIndex(
                name: "IX_FinancialAccountOperations_TransferFinancialAccountId",
                table: "FinancialAccountOperations");

            migrationBuilder.DropColumn(
                name: "TransferFinancialAccountId",
                table: "FinancialAccountOperations");
        }
    }
}
