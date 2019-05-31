using Microsoft.EntityFrameworkCore.Migrations;

namespace AvibaWeb.Migrations
{
    public partial class MProviderBalance : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Balance",
                table: "Counterparties");

            migrationBuilder.CreateTable(
                name: "ProviderBinding",
                columns: table => new
                {
                    ProviderId = table.Column<string>(nullable: false),
                    Balance = table.Column<decimal>(type: "Money", nullable: false),
                    Session = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProviderBinding", x => x.ProviderId);
                    table.ForeignKey(
                        name: "FK_ProviderBinding_Counterparties_ProviderId",
                        column: x => x.ProviderId,
                        principalTable: "Counterparties",
                        principalColumn: "ITN",
                        onDelete: ReferentialAction.Cascade);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ProviderBinding");

            migrationBuilder.AddColumn<decimal>(
                name: "Balance",
                table: "Counterparties",
                type: "Money",
                nullable: false,
                defaultValue: 0m);
        }
    }
}
