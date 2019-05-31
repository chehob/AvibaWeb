using Microsoft.EntityFrameworkCore.Migrations;

namespace AvibaWeb.Migrations
{
    public partial class MFinancialAccountOperationForeignKeyFix : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_FinancialAccounts_Organizations_OrganizationId",
                table: "FinancialAccounts");

            migrationBuilder.AlterColumn<int>(
                name: "OrganizationId",
                table: "FinancialAccounts",
                nullable: false,
                oldClrType: typeof(int),
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_FinancialAccounts_Organizations_OrganizationId",
                table: "FinancialAccounts",
                column: "OrganizationId",
                principalTable: "Organizations",
                principalColumn: "OrganizationId",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_FinancialAccounts_Organizations_OrganizationId",
                table: "FinancialAccounts");

            migrationBuilder.AlterColumn<int>(
                name: "OrganizationId",
                table: "FinancialAccounts",
                nullable: true,
                oldClrType: typeof(int));

            migrationBuilder.AddForeignKey(
                name: "FK_FinancialAccounts_Organizations_OrganizationId",
                table: "FinancialAccounts",
                column: "OrganizationId",
                principalTable: "Organizations",
                principalColumn: "OrganizationId",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
