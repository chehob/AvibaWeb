using Microsoft.EntityFrameworkCore.Migrations;

namespace AvibaWeb.Migrations
{
    public partial class MOfficeBalanceHistory_BillInfo : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "_2kBill",
                table: "OfficeBalanceHistory",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "_5kBill",
                table: "OfficeBalanceHistory",
                nullable: false,
                defaultValue: 0m);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "_2kBill",
                table: "OfficeBalanceHistory");

            migrationBuilder.DropColumn(
                name: "_5kBill",
                table: "OfficeBalanceHistory");
        }
    }
}
