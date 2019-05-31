using Microsoft.EntityFrameworkCore.Migrations;

namespace AvibaWeb.Migrations
{
    public partial class MFinancialAccountCorrAccount : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<decimal>(
                name: "Balance",
                table: "FinancialAccounts",
                type: "Money",
                nullable: false,
                oldClrType: typeof(decimal));

            migrationBuilder.AddColumn<string>(
                name: "CorrespondentAccount",
                table: "FinancialAccounts",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CorrespondentAccount",
                table: "FinancialAccounts");

            migrationBuilder.AlterColumn<decimal>(
                name: "Balance",
                table: "FinancialAccounts",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "Money");
        }
    }
}
