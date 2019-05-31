using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace AvibaWeb.Migrations
{
    public partial class MTransitAccountCreditInitial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TransitAccountCredits",
                columns: table => new
                {
                    TransitAccountCreditId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    AccountAmount = table.Column<decimal>(nullable: false),
                    AddAmount = table.Column<decimal>(nullable: false),
                    TransitAccountId = table.Column<int>(nullable: false),
                    LoanGroupId = table.Column<int>(nullable: false),
                    OperationDateTime = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TransitAccountCredits", x => x.TransitAccountCreditId);
                    table.ForeignKey(
                        name: "FK_TransitAccountCredits_LoanGroups_LoanGroupId",
                        column: x => x.LoanGroupId,
                        principalTable: "LoanGroups",
                        principalColumn: "LoanGroupId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TransitAccountCredits_TransitAccounts_TransitAccountId",
                        column: x => x.TransitAccountId,
                        principalTable: "TransitAccounts",
                        principalColumn: "TransitAccountId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TransitAccountCredits_LoanGroupId",
                table: "TransitAccountCredits",
                column: "LoanGroupId");

            migrationBuilder.CreateIndex(
                name: "IX_TransitAccountCredits_TransitAccountId",
                table: "TransitAccountCredits",
                column: "TransitAccountId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TransitAccountCredits");
        }
    }
}
