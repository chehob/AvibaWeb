using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace AvibaWeb.Migrations
{
    public partial class MIncomingExpenditures_Initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "IncomingExpenditures",
                columns: table => new
                {
                    IncomingExpenditureId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Amount = table.Column<decimal>(nullable: true),
                    FinancialAccountOperationId = table.Column<int>(nullable: false),
                    IsProcessed = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IncomingExpenditures", x => x.IncomingExpenditureId);
                    table.ForeignKey(
                        name: "FK_IncomingExpenditures_FinancialAccountOperations_FinancialAccountOperationId",
                        column: x => x.FinancialAccountOperationId,
                        principalTable: "FinancialAccountOperations",
                        principalColumn: "FinancialAccountOperationId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_IncomingExpenditures_FinancialAccountOperationId",
                table: "IncomingExpenditures",
                column: "FinancialAccountOperationId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "IncomingExpenditures");
        }
    }
}
