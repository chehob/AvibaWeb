using Microsoft.EntityFrameworkCore.Migrations;

namespace AvibaWeb.Migrations
{
    public partial class MFinancialAccountOperationUserId : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "FinancialAccountOperations",
                maxLength: 128,
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_FinancialAccountOperations_UserId",
                table: "FinancialAccountOperations",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_FinancialAccountOperations_AspNetUsers_UserId",
                table: "FinancialAccountOperations",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_FinancialAccountOperations_AspNetUsers_UserId",
                table: "FinancialAccountOperations");

            migrationBuilder.DropIndex(
                name: "IX_FinancialAccountOperations_UserId",
                table: "FinancialAccountOperations");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "FinancialAccountOperations");
        }
    }
}
