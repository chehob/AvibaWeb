using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace AvibaWeb.Migrations
{
    public partial class MTransitAccountCreditOperations : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "OperationDateTime",
                table: "TransitAccountCredits");

            migrationBuilder.CreateTable(
                name: "TransitAccountCreditOperations",
                columns: table => new
                {
                    TransitAccountCreditOperationId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    TransitAccountCreditId = table.Column<int>(nullable: false),
                    OperationTypeId = table.Column<int>(nullable: false),
                    OperationDateTime = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TransitAccountCreditOperations", x => x.TransitAccountCreditOperationId);
                    table.ForeignKey(
                        name: "FK_TransitAccountCreditOperations_TransitAccountCredits_TransitAccountCreditId",
                        column: x => x.TransitAccountCreditId,
                        principalTable: "TransitAccountCredits",
                        principalColumn: "TransitAccountCreditId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TransitAccountCreditOperations_TransitAccountCreditId",
                table: "TransitAccountCreditOperations",
                column: "TransitAccountCreditId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TransitAccountCreditOperations");

            migrationBuilder.AddColumn<DateTime>(
                name: "OperationDateTime",
                table: "TransitAccountCredits",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }
    }
}
