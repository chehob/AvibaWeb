using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace AvibaWeb.Migrations
{
    public partial class MCorporatorAccountInitial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CorporatorAccounts",
                columns: table => new
                {
                    CorporatorAccountId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    ITN = table.Column<string>(nullable: true),
                    Description = table.Column<string>(nullable: true),
                    BankName = table.Column<string>(nullable: true),
                    OffBankName = table.Column<string>(nullable: true),
                    BIK = table.Column<string>(nullable: true),
                    CorrespondentAccount = table.Column<string>(nullable: true),
                    IsActive = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CorporatorAccounts", x => x.CorporatorAccountId);
                    table.ForeignKey(
                        name: "FK_CorporatorAccounts_Counterparties_ITN",
                        column: x => x.ITN,
                        principalTable: "Counterparties",
                        principalColumn: "ITN",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CorporatorAccounts_ITN",
                table: "CorporatorAccounts",
                column: "ITN");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CorporatorAccounts");
        }
    }
}
