using Microsoft.EntityFrameworkCore.Migrations;

namespace AvibaWeb.Migrations
{
    public partial class MSubagentFeeTransaction_Fix : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Comment",
                table: "SubagentFeeTransactions");

            migrationBuilder.RenameColumn(
                name: "OldAgentFee",
                table: "SubagentFeeTransactions",
                newName: "BalanceOld");

            migrationBuilder.RenameColumn(
                name: "Amount",
                table: "SubagentFeeTransactions",
                newName: "BalanceNew");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "BalanceOld",
                table: "SubagentFeeTransactions",
                newName: "OldAgentFee");

            migrationBuilder.RenameColumn(
                name: "BalanceNew",
                table: "SubagentFeeTransactions",
                newName: "Amount");

            migrationBuilder.AddColumn<string>(
                name: "Comment",
                table: "SubagentFeeTransactions",
                nullable: true);
        }
    }
}
