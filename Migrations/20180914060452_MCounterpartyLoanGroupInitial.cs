using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace AvibaWeb.Migrations
{
    public partial class MCounterpartyLoanGroupInitial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "LoanGroups",
                columns: table => new
                {
                    LoanGroupId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Balance = table.Column<decimal>(type: "Money", nullable: false),
                    IsActive = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LoanGroups", x => x.LoanGroupId);
                });

            migrationBuilder.CreateTable(
                name: "CounterpartyLoanGroup",
                columns: table => new
                {
                    CounterpartyId = table.Column<string>(nullable: false),
                    LoanGroupId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CounterpartyLoanGroup", x => new { x.CounterpartyId, x.LoanGroupId });
                    table.ForeignKey(
                        name: "FK_CounterpartyLoanGroup_Counterparties_CounterpartyId",
                        column: x => x.CounterpartyId,
                        principalTable: "Counterparties",
                        principalColumn: "ITN",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CounterpartyLoanGroup_LoanGroups_LoanGroupId",
                        column: x => x.LoanGroupId,
                        principalTable: "LoanGroups",
                        principalColumn: "LoanGroupId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CounterpartyLoanGroup_LoanGroupId",
                table: "CounterpartyLoanGroup",
                column: "LoanGroupId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CounterpartyLoanGroup");

            migrationBuilder.DropTable(
                name: "LoanGroups");
        }
    }
}
