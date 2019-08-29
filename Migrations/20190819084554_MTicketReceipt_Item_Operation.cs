using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace AvibaWeb.Migrations
{
    public partial class MTicketReceipt_Item_Operation : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ServiceReceiptItems",
                columns: table => new
                {
                    ServiceReceiptItemId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    ServiceReceiptId = table.Column<int>(nullable: false),
                    SegmentId = table.Column<int>(nullable: true),
                    Amount = table.Column<decimal>(nullable: false),
                    IsCanceled = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ServiceReceiptItems", x => x.ServiceReceiptItemId);
                    table.ForeignKey(
                        name: "FK_ServiceReceiptItems_ServiceReceipts_ServiceReceiptId",
                        column: x => x.ServiceReceiptId,
                        principalTable: "ServiceReceipts",
                        principalColumn: "ServiceReceiptId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ServiceReceiptOperations",
                columns: table => new
                {
                    ServiceReceiptOperationId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    ServiceReceiptId = table.Column<int>(nullable: false),
                    DeskIssuedId = table.Column<string>(nullable: true),
                    DateTime = table.Column<DateTime>(nullable: false),
                    Type = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ServiceReceiptOperations", x => x.ServiceReceiptOperationId);
                    table.ForeignKey(
                        name: "FK_ServiceReceiptOperations_Desks_DeskIssuedId",
                        column: x => x.DeskIssuedId,
                        principalTable: "Desks",
                        principalColumn: "DeskId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ServiceReceiptOperations_ServiceReceipts_ServiceReceiptId",
                        column: x => x.ServiceReceiptId,
                        principalTable: "ServiceReceipts",
                        principalColumn: "ServiceReceiptId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ServiceReceiptItems_ServiceReceiptId",
                table: "ServiceReceiptItems",
                column: "ServiceReceiptId");

            migrationBuilder.CreateIndex(
                name: "IX_ServiceReceiptOperations_DeskIssuedId",
                table: "ServiceReceiptOperations",
                column: "DeskIssuedId");

            migrationBuilder.CreateIndex(
                name: "IX_ServiceReceiptOperations_ServiceReceiptId",
                table: "ServiceReceiptOperations",
                column: "ServiceReceiptId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ServiceReceiptItems");

            migrationBuilder.DropTable(
                name: "ServiceReceiptOperations");
        }
    }
}
