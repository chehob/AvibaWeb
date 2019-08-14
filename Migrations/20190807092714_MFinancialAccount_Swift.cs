using Microsoft.EntityFrameworkCore.Migrations;

namespace AvibaWeb.Migrations
{
    public partial class MFinancialAccount_Swift : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Organizations__1CUploadData__1CUploadDataId",
                table: "Organizations");

            migrationBuilder.AddColumn<string>(
                name: "SWIFTBIK",
                table: "FinancialAccounts",
                nullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Organizations__1CUploadData__1CUploadDataId",
                table: "Organizations",
                column: "_1CUploadDataId",
                principalTable: "_1CUploadData",
                principalColumn: "_1CUploadDataId",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Organizations__1CUploadData__1CUploadDataId",
                table: "Organizations");

            migrationBuilder.DropColumn(
                name: "SWIFTBIK",
                table: "FinancialAccounts");

            migrationBuilder.AddForeignKey(
                name: "FK_Organizations__1CUploadData__1CUploadDataId",
                table: "Organizations",
                column: "_1CUploadDataId",
                principalTable: "_1CUploadData",
                principalColumn: "_1CUploadDataId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
