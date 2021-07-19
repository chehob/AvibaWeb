using Microsoft.EntityFrameworkCore.Migrations;

namespace AvibaWeb.Migrations
{
    public partial class MAtolPrintSettings_PercentageUpdate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "PrintPercentage",
                table: "AtolPrintSettings",
                newName: "PrintMinPercentage");

            migrationBuilder.AddColumn<int>(
                name: "PrintMaxPercentage",
                table: "AtolPrintSettings",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PrintMaxPercentage",
                table: "AtolPrintSettings");

            migrationBuilder.RenameColumn(
                name: "PrintMinPercentage",
                table: "AtolPrintSettings",
                newName: "PrintPercentage");
        }
    }
}
