using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace AvibaWeb.Migrations
{
    public partial class MSubagentFeeTransaction_Initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "SubagentFeeTransactions",
                columns: table => new
                {
                    SubagentFeeTransactionId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    SubagentId = table.Column<string>(nullable: true),
                    OldAgentFee = table.Column<decimal>(type: "Money", nullable: false),
                    Amount = table.Column<decimal>(type: "Money", nullable: false),
                    Comment = table.Column<string>(nullable: true),
                    TransactionDateTime = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SubagentFeeTransactions", x => x.SubagentFeeTransactionId);
                    table.ForeignKey(
                        name: "FK_SubagentFeeTransactions_SubagentData_SubagentId",
                        column: x => x.SubagentId,
                        principalTable: "SubagentData",
                        principalColumn: "SubagentId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SubagentFeeTransactions_SubagentId",
                table: "SubagentFeeTransactions",
                column: "SubagentId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SubagentFeeTransactions");
        }
    }
}
