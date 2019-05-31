using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace AvibaWeb.Migrations
{
    public partial class MCorporatorReceiptOperations : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CorporatorReceiptOperations",
                columns: table => new
                {
                    CorporatorReceiptOperationId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CorporatorReceiptId = table.Column<int>(nullable: false),
                    OperationTypeId = table.Column<int>(nullable: false),
                    OperationDateTime = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CorporatorReceiptOperations", x => x.CorporatorReceiptOperationId);
                    table.ForeignKey(
                        name: "FK_CorporatorReceiptOperations_CorporatorReceipts_CorporatorReceiptId",
                        column: x => x.CorporatorReceiptId,
                        principalTable: "CorporatorReceipts",
                        principalColumn: "CorporatorReceiptId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CorporatorReceiptOperations_CorporatorReceiptId",
                table: "CorporatorReceiptOperations",
                column: "CorporatorReceiptId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CorporatorReceiptOperations");
        }
    }
}
