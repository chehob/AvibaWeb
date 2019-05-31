using Microsoft.EntityFrameworkCore.Migrations;

namespace AvibaWeb.Migrations
{
    public partial class MCorporatorReceiptItemPassengerAndRoute : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "PassengerName",
                table: "CorporatorReceiptItems",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Route",
                table: "CorporatorReceiptItems",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PassengerName",
                table: "CorporatorReceiptItems");

            migrationBuilder.DropColumn(
                name: "Route",
                table: "CorporatorReceiptItems");
        }
    }
}
