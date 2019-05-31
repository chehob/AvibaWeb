using Microsoft.EntityFrameworkCore.Migrations;

namespace AvibaWeb.Migrations
{
    public partial class MTransitAccountCreditLoanGroupNullable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TransitAccountCredits_LoanGroups_LoanGroupId",
                table: "TransitAccountCredits");

            migrationBuilder.AlterColumn<int>(
                name: "LoanGroupId",
                table: "TransitAccountCredits",
                nullable: true,
                oldClrType: typeof(int));

            migrationBuilder.AddForeignKey(
                name: "FK_TransitAccountCredits_LoanGroups_LoanGroupId",
                table: "TransitAccountCredits",
                column: "LoanGroupId",
                principalTable: "LoanGroups",
                principalColumn: "LoanGroupId",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TransitAccountCredits_LoanGroups_LoanGroupId",
                table: "TransitAccountCredits");

            migrationBuilder.AlterColumn<int>(
                name: "LoanGroupId",
                table: "TransitAccountCredits",
                nullable: false,
                oldClrType: typeof(int),
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_TransitAccountCredits_LoanGroups_LoanGroupId",
                table: "TransitAccountCredits",
                column: "LoanGroupId",
                principalTable: "LoanGroups",
                principalColumn: "LoanGroupId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
