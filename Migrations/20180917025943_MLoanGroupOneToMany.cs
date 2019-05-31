using Microsoft.EntityFrameworkCore.Migrations;

namespace AvibaWeb.Migrations
{
    public partial class MLoanGroupOneToMany : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CounterpartyLoanGroup");

            migrationBuilder.AddColumn<int>(
                name: "LoanGroupId",
                table: "Counterparties",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Counterparties_LoanGroupId",
                table: "Counterparties",
                column: "LoanGroupId");

            migrationBuilder.AddForeignKey(
                name: "FK_Counterparties_LoanGroups_LoanGroupId",
                table: "Counterparties",
                column: "LoanGroupId",
                principalTable: "LoanGroups",
                principalColumn: "LoanGroupId",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Counterparties_LoanGroups_LoanGroupId",
                table: "Counterparties");

            migrationBuilder.DropIndex(
                name: "IX_Counterparties_LoanGroupId",
                table: "Counterparties");

            migrationBuilder.DropColumn(
                name: "LoanGroupId",
                table: "Counterparties");

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
    }
}
