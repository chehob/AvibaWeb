using Microsoft.EntityFrameworkCore.Migrations;

namespace AvibaWeb.Migrations
{
    public partial class MCorpReceiptNullPayeeAccount : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CorporatorReceipts_FinancialAccounts_PayeeAccountId",
                table: "CorporatorReceipts");

            migrationBuilder.AlterColumn<int>(
                name: "PayeeAccountId",
                table: "CorporatorReceipts",
                nullable: true,
                oldClrType: typeof(int));

            migrationBuilder.AddForeignKey(
                name: "FK_CorporatorReceipts_FinancialAccounts_PayeeAccountId",
                table: "CorporatorReceipts",
                column: "PayeeAccountId",
                principalTable: "FinancialAccounts",
                principalColumn: "FinancialAccountId",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CorporatorReceipts_FinancialAccounts_PayeeAccountId",
                table: "CorporatorReceipts");

            migrationBuilder.AlterColumn<int>(
                name: "PayeeAccountId",
                table: "CorporatorReceipts",
                nullable: false,
                oldClrType: typeof(int),
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_CorporatorReceipts_FinancialAccounts_PayeeAccountId",
                table: "CorporatorReceipts",
                column: "PayeeAccountId",
                principalTable: "FinancialAccounts",
                principalColumn: "FinancialAccountId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
