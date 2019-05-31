using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace AvibaWeb.Migrations
{
    public partial class MCorporatorReceipt : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CorporatorReceipts",
                columns: table => new
                {
                    CorporatorReceiptId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    ReceiptNumber = table.Column<int>(nullable: false),
                    CorporatorId = table.Column<string>(nullable: true),
                    Amount = table.Column<decimal>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CorporatorReceipts", x => x.CorporatorReceiptId);
                    table.ForeignKey(
                        name: "FK_CorporatorReceipts_Counterparties_CorporatorId",
                        column: x => x.CorporatorId,
                        principalTable: "Counterparties",
                        principalColumn: "ITN",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CorporatorReceipts_CorporatorId",
                table: "CorporatorReceipts",
                column: "CorporatorId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CorporatorReceipts");
        }
    }
}
