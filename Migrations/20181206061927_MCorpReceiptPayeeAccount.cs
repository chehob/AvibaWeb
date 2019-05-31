using Microsoft.EntityFrameworkCore.Migrations;

namespace AvibaWeb.Migrations
{
    public partial class MCorpReceiptPayeeAccount : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CorporatorData");

            migrationBuilder.AddColumn<int>(
                name: "PayeeAccountId",
                table: "CorporatorReceipts",
                nullable: true,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_CorporatorReceipts_PayeeAccountId",
                table: "CorporatorReceipts",
                column: "PayeeAccountId");

            migrationBuilder.AddForeignKey(
                name: "FK_CorporatorReceipts_FinancialAccounts_PayeeAccountId",
                table: "CorporatorReceipts",
                column: "PayeeAccountId",
                principalTable: "FinancialAccounts",
                principalColumn: "FinancialAccountId",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CorporatorReceipts_FinancialAccounts_PayeeAccountId",
                table: "CorporatorReceipts");

            migrationBuilder.DropIndex(
                name: "IX_CorporatorReceipts_PayeeAccountId",
                table: "CorporatorReceipts");

            migrationBuilder.DropColumn(
                name: "PayeeAccountId",
                table: "CorporatorReceipts");

            migrationBuilder.CreateTable(
                name: "CorporatorData",
                columns: table => new
                {
                    CorporatorId = table.Column<string>(nullable: false),
                    OrganizationId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CorporatorData", x => x.CorporatorId);
                    table.ForeignKey(
                        name: "FK_CorporatorData_Counterparties_CorporatorId",
                        column: x => x.CorporatorId,
                        principalTable: "Counterparties",
                        principalColumn: "ITN",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CorporatorData_Organizations_OrganizationId",
                        column: x => x.OrganizationId,
                        principalTable: "Organizations",
                        principalColumn: "OrganizationId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CorporatorData_OrganizationId",
                table: "CorporatorData",
                column: "OrganizationId");
        }
    }
}
