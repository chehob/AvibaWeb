using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace AvibaWeb.Migrations
{
    public partial class MCorporatorAccount_Dates : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "LastPaymentDate",
                table: "CorporatorAccounts",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "LastReceiptDate",
                table: "CorporatorAccounts",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LastPaymentDate",
                table: "CorporatorAccounts");

            migrationBuilder.DropColumn(
                name: "LastReceiptDate",
                table: "CorporatorAccounts");
        }
    }
}
