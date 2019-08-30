using Microsoft.EntityFrameworkCore.Migrations;

namespace AvibaWeb.Migrations
{
    public partial class MSubagentFeeTransaction_Fix2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BalanceNew",
                table: "SubagentFeeTransactions");

            migrationBuilder.RenameColumn(
                name: "BalanceOld",
                table: "SubagentFeeTransactions",
                newName: "Amount");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Amount",
                table: "SubagentFeeTransactions",
                newName: "BalanceOld");

            migrationBuilder.AddColumn<decimal>(
                name: "BalanceNew",
                table: "SubagentFeeTransactions",
                type: "Money",
                nullable: false,
                defaultValue: 0m);
        }
    }
}
