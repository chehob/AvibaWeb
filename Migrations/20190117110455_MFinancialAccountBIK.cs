using Microsoft.EntityFrameworkCore.Migrations;

namespace AvibaWeb.Migrations
{
    public partial class MFinancialAccountBIK : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "BIK",
                table: "FinancialAccounts",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "OffBankName",
                table: "FinancialAccounts",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BIK",
                table: "FinancialAccounts");

            migrationBuilder.DropColumn(
                name: "OffBankName",
                table: "FinancialAccounts");
        }
    }
}
