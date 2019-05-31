using Microsoft.EntityFrameworkCore.Migrations;

namespace AvibaWeb.Migrations
{
    public partial class MProviderBalanceUpdate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Balance",
                table: "ProviderBinding");

            migrationBuilder.DropColumn(
                name: "Deposit",
                table: "ProviderBinding");

            migrationBuilder.CreateTable(
                name: "ProviderBalance",
                columns: table => new
                {
                    ProviderId = table.Column<string>(nullable: false),
                    Balance = table.Column<decimal>(type: "Money", nullable: false),
                    Deposit = table.Column<decimal>(type: "Money", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProviderBalance", x => x.ProviderId);
                    table.ForeignKey(
                        name: "FK_ProviderBalance_Counterparties_ProviderId",
                        column: x => x.ProviderId,
                        principalTable: "Counterparties",
                        principalColumn: "ITN",
                        onDelete: ReferentialAction.Cascade);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ProviderBalance");

            migrationBuilder.AddColumn<decimal>(
                name: "Balance",
                table: "ProviderBinding",
                type: "Money",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "Deposit",
                table: "ProviderBinding",
                type: "Money",
                nullable: false,
                defaultValue: 0m);
        }
    }
}
