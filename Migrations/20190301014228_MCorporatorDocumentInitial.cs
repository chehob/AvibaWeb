using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace AvibaWeb.Migrations
{
    public partial class MCorporatorDocumentInitial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CorporatorDocuments",
                columns: table => new
                {
                    CorporatorDocumentId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    ITN = table.Column<string>(nullable: true),
                    OrganizationId = table.Column<int>(nullable: false),
                    Document = table.Column<string>(nullable: true),
                    Date = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CorporatorDocuments", x => x.CorporatorDocumentId);
                    table.ForeignKey(
                        name: "FK_CorporatorDocuments_Counterparties_ITN",
                        column: x => x.ITN,
                        principalTable: "Counterparties",
                        principalColumn: "ITN",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CorporatorDocuments_Organizations_OrganizationId",
                        column: x => x.OrganizationId,
                        principalTable: "Organizations",
                        principalColumn: "OrganizationId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CorporatorDocuments_ITN",
                table: "CorporatorDocuments",
                column: "ITN");

            migrationBuilder.CreateIndex(
                name: "IX_CorporatorDocuments_OrganizationId",
                table: "CorporatorDocuments",
                column: "OrganizationId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CorporatorDocuments");
        }
    }
}
