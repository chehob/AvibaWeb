using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace AvibaWeb.Migrations
{
    public partial class MFinancialAccountOperationInitial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "FinancialAccountOperations",
                columns: table => new
                {
                    FinancialAccountOperationId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Amount = table.Column<decimal>(nullable: false),
                    FinancialAccountId = table.Column<int>(nullable: false),
                    CounterpartyId = table.Column<string>(nullable: true),
                    OperationDateTime = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FinancialAccountOperations", x => x.FinancialAccountOperationId);
                    table.ForeignKey(
                        name: "FK_FinancialAccountOperations_Counterparties_CounterpartyId",
                        column: x => x.CounterpartyId,
                        principalTable: "Counterparties",
                        principalColumn: "ITN",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_FinancialAccountOperations_FinancialAccounts_FinancialAccountId",
                        column: x => x.FinancialAccountId,
                        principalTable: "FinancialAccounts",
                        principalColumn: "FinancialAccountId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_FinancialAccountOperations_CounterpartyId",
                table: "FinancialAccountOperations",
                column: "CounterpartyId");

            migrationBuilder.CreateIndex(
                name: "IX_FinancialAccountOperations_FinancialAccountId",
                table: "FinancialAccountOperations",
                column: "FinancialAccountId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FinancialAccountOperations");
        }
    }
}
