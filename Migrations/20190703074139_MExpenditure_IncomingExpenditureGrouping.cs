using Microsoft.EntityFrameworkCore.Migrations;

namespace AvibaWeb.Migrations
{
    public partial class MExpenditure_IncomingExpenditureGrouping : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "IncomingExpenditureId",
                table: "Expenditures",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Expenditures_IncomingExpenditureId",
                table: "Expenditures",
                column: "IncomingExpenditureId");

            migrationBuilder.AddForeignKey(
                name: "FK_Expenditures_IncomingExpenditures_IncomingExpenditureId",
                table: "Expenditures",
                column: "IncomingExpenditureId",
                principalTable: "IncomingExpenditures",
                principalColumn: "IncomingExpenditureId",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Expenditures_IncomingExpenditures_IncomingExpenditureId",
                table: "Expenditures");

            migrationBuilder.DropIndex(
                name: "IX_Expenditures_IncomingExpenditureId",
                table: "Expenditures");

            migrationBuilder.DropColumn(
                name: "IncomingExpenditureId",
                table: "Expenditures");
        }
    }
}
