using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace AvibaWeb.Migrations
{
    public partial class MLoanExpenditure_Initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "LoanExpenditures",
                columns: table => new
                {
                    LoanExpenditureId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Description = table.Column<string>(nullable: true),
                    Amount = table.Column<decimal>(type: "Money", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LoanExpenditures", x => x.LoanExpenditureId);
                });

            migrationBuilder.CreateTable(
                name: "LoanExpenditureOperations",
                columns: table => new
                {
                    LoanExpenditureOperationId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    LoanExpenditureId = table.Column<int>(nullable: false),
                    OperationTypeId = table.Column<int>(nullable: false),
                    OperationDateTime = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LoanExpenditureOperations", x => x.LoanExpenditureOperationId);
                    table.ForeignKey(
                        name: "FK_LoanExpenditureOperations_LoanExpenditures_LoanExpenditureId",
                        column: x => x.LoanExpenditureId,
                        principalTable: "LoanExpenditures",
                        principalColumn: "LoanExpenditureId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_LoanExpenditureOperations_LoanExpenditureId",
                table: "LoanExpenditureOperations",
                column: "LoanExpenditureId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "LoanExpenditureOperations");

            migrationBuilder.DropTable(
                name: "LoanExpenditures");
        }
    }
}
