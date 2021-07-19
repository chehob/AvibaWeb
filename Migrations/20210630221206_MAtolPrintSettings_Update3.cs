using Microsoft.EntityFrameworkCore.Migrations;

namespace AvibaWeb.Migrations
{
    public partial class MAtolPrintSettings_Update3 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CashierName",
                table: "AtolPrintSettings",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DeskBinding",
                table: "AtolPrintSettings",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsPermanent",
                table: "AtolPrintSettings",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CashierName",
                table: "AtolPrintSettings");

            migrationBuilder.DropColumn(
                name: "DeskBinding",
                table: "AtolPrintSettings");

            migrationBuilder.DropColumn(
                name: "IsPermanent",
                table: "AtolPrintSettings");
        }
    }
}
