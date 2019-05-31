using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace AvibaWeb.Migrations
{
    public partial class MFinancialAccountOperation : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameIndex(
                name: "IX_FinancialAccount_OrganizationId",
                table: "FinancialAccounts",
                newName: "IX_FinancialAccounts_OrganizationId");

            migrationBuilder.AddColumn<string>(
                name: "UserITN",
                table: "AspNetUsers",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UserITN",
                table: "AspNetUsers");
        }
    }
}
