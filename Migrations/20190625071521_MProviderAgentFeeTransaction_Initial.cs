using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace AvibaWeb.Migrations
{
    public partial class MProviderAgentFeeTransaction_Initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "AgentFee",
                table: "ProviderBalance",
                type: "Money",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.CreateTable(
                name: "ProviderAgentFeeTransactions",
                columns: table => new
                {
                    ProviderAgentFeeTransactionId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    ProviderId = table.Column<string>(nullable: true),
                    OldAgentFee = table.Column<decimal>(type: "Money", nullable: false),
                    Amount = table.Column<decimal>(type: "Money", nullable: false),
                    TransactionDateTime = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProviderAgentFeeTransactions", x => x.ProviderAgentFeeTransactionId);
                    table.ForeignKey(
                        name: "FK_ProviderAgentFeeTransactions_ProviderBalance_ProviderId",
                        column: x => x.ProviderId,
                        principalTable: "ProviderBalance",
                        principalColumn: "ProviderId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ProviderAgentFeeTransactions_ProviderId",
                table: "ProviderAgentFeeTransactions",
                column: "ProviderId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ProviderAgentFeeTransactions");

            migrationBuilder.DropColumn(
                name: "AgentFee",
                table: "ProviderBalance");
        }
    }
}
