using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace AvibaWeb.Migrations
{
    public partial class MOfficeDebitInitial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "OfficeDebits",
                columns: table => new
                {
                    OfficeDebitId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Amount = table.Column<decimal>(nullable: false),
                    Description = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OfficeDebits", x => x.OfficeDebitId);
                });

            migrationBuilder.CreateTable(
                name: "OfficeDebitOperations",
                columns: table => new
                {
                    OfficeDebitOperationId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    OfficeDebitId = table.Column<int>(nullable: false),
                    OperationTypeId = table.Column<int>(nullable: false),
                    OperationDateTime = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OfficeDebitOperations", x => x.OfficeDebitOperationId);
                    table.ForeignKey(
                        name: "FK_OfficeDebitOperations_OfficeDebits_OfficeDebitId",
                        column: x => x.OfficeDebitId,
                        principalTable: "OfficeDebits",
                        principalColumn: "OfficeDebitId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_OfficeDebitOperations_OfficeDebitId",
                table: "OfficeDebitOperations",
                column: "OfficeDebitId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "OfficeDebitOperations");

            migrationBuilder.DropTable(
                name: "OfficeDebits");
        }
    }
}
