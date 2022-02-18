using Microsoft.EntityFrameworkCore.Migrations;

namespace AvibaWeb.Migrations
{
    public partial class MCounterpartyProxy_Initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsProxy",
                table: "Counterparties",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "ProxyDocument",
                table: "Counterparties",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ProxyName",
                table: "Counterparties",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ProxyPosition",
                table: "Counterparties",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsProxy",
                table: "Counterparties");

            migrationBuilder.DropColumn(
                name: "ProxyDocument",
                table: "Counterparties");

            migrationBuilder.DropColumn(
                name: "ProxyName",
                table: "Counterparties");

            migrationBuilder.DropColumn(
                name: "ProxyPosition",
                table: "Counterparties");
        }
    }
}
