using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace AvibaWeb.Migrations
{
    public partial class MCorporatorDocumentFeeItems : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CorporatorDocumentFeeItems",
                columns: table => new
                {
                    CorporatorDocumentFeeItemId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CorporatorDocumentId = table.Column<int>(nullable: false),
                    Name = table.Column<string>(nullable: true),
                    FeeStr = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CorporatorDocumentFeeItems", x => x.CorporatorDocumentFeeItemId);
                    table.ForeignKey(
                        name: "FK_CorporatorDocumentFeeItems_CorporatorDocuments_CorporatorDocumentId",
                        column: x => x.CorporatorDocumentId,
                        principalTable: "CorporatorDocuments",
                        principalColumn: "CorporatorDocumentId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CorporatorDocumentFeeItems_CorporatorDocumentId",
                table: "CorporatorDocumentFeeItems",
                column: "CorporatorDocumentId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CorporatorDocumentFeeItems");
        }
    }
}
