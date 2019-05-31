using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace AvibaWeb.Migrations
{
    public partial class MTransitAccountDebitInitial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TransitAccountDebits",
                columns: table => new
                {
                    TransitAccountDebitId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Amount = table.Column<decimal>(nullable: false),
                    TransitAccountId = table.Column<int>(nullable: false),
                    OperationDateTime = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TransitAccountDebits", x => x.TransitAccountDebitId);
                    table.ForeignKey(
                        name: "FK_TransitAccountDebits_TransitAccounts_TransitAccountId",
                        column: x => x.TransitAccountId,
                        principalTable: "TransitAccounts",
                        principalColumn: "TransitAccountId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TransitAccountDebits_TransitAccountId",
                table: "TransitAccountDebits",
                column: "TransitAccountId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TransitAccountDebits");
        }
    }
}
