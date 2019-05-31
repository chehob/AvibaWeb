using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace AvibaWeb.Migrations
{
    public partial class MCorporatorReceiptItem : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CorporatorReceiptItems",
                columns: table => new
                {
                    CorporatorReceiptItemId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CorporatorReceiptId = table.Column<int>(nullable: false),
                    Amount = table.Column<decimal>(nullable: false),
                    TicketOperationId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CorporatorReceiptItems", x => x.CorporatorReceiptItemId);
                    table.ForeignKey(
                        name: "FK_CorporatorReceiptItems_CorporatorReceipts_CorporatorReceiptId",
                        column: x => x.CorporatorReceiptId,
                        principalTable: "CorporatorReceipts",
                        principalColumn: "CorporatorReceiptId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CorporatorReceiptItems_CorporatorReceiptId",
                table: "CorporatorReceiptItems",
                column: "CorporatorReceiptId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CorporatorReceiptItems");
        }
    }
}
