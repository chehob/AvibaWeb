using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace AvibaWeb.Migrations
{
    public partial class MMultiPaymentWithFinancialOperation : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_CorporatorAccounts_ITN",
                table: "CorporatorAccounts");

            migrationBuilder.DropColumn(
                name: "CreatedDateTime",
                table: "CorporatorReceiptMultiPayments");

            migrationBuilder.RenameColumn(
                name: "Description",
                table: "CorporatorReceiptMultiPayments",
                newName: "ErrorString");

            migrationBuilder.AddColumn<int>(
                name: "FinancialAccountOperationId",
                table: "CorporatorReceiptMultiPayments",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_CorporatorReceiptMultiPayments_FinancialAccountOperationId",
                table: "CorporatorReceiptMultiPayments",
                column: "FinancialAccountOperationId");

            migrationBuilder.CreateIndex(
                name: "IX_CorporatorAccounts_ITN",
                table: "CorporatorAccounts",
                column: "ITN",
                unique: true,
                filter: "[ITN] IS NOT NULL");

            migrationBuilder.AddForeignKey(
                name: "FK_CorporatorReceiptMultiPayments_FinancialAccountOperations_FinancialAccountOperationId",
                table: "CorporatorReceiptMultiPayments",
                column: "FinancialAccountOperationId",
                principalTable: "FinancialAccountOperations",
                principalColumn: "FinancialAccountOperationId",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CorporatorReceiptMultiPayments_FinancialAccountOperations_FinancialAccountOperationId",
                table: "CorporatorReceiptMultiPayments");

            migrationBuilder.DropIndex(
                name: "IX_CorporatorReceiptMultiPayments_FinancialAccountOperationId",
                table: "CorporatorReceiptMultiPayments");

            migrationBuilder.DropIndex(
                name: "IX_CorporatorAccounts_ITN",
                table: "CorporatorAccounts");

            migrationBuilder.DropColumn(
                name: "FinancialAccountOperationId",
                table: "CorporatorReceiptMultiPayments");

            migrationBuilder.RenameColumn(
                name: "ErrorString",
                table: "CorporatorReceiptMultiPayments",
                newName: "Description");

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedDateTime",
                table: "CorporatorReceiptMultiPayments",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_CorporatorAccounts_ITN",
                table: "CorporatorAccounts",
                column: "ITN");
        }
    }
}
