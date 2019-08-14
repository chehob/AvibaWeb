using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace AvibaWeb.Migrations
{
    public partial class M1CProviderDocument_Initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "_1СProviderDocuments",
                columns: table => new
                {
                    _1СProviderDocumentId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    ProviderId = table.Column<string>(nullable: true),
                    OrganizationId = table.Column<int>(nullable: false),
                    DocumentName = table.Column<string>(nullable: true),
                    DocumentNumber = table.Column<string>(nullable: true),
                    DocumentDate = table.Column<DateTime>(nullable: false),
                    IsActive = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__1СProviderDocuments", x => x._1СProviderDocumentId);
                    table.ForeignKey(
                        name: "FK__1СProviderDocuments_Organizations_OrganizationId",
                        column: x => x.OrganizationId,
                        principalTable: "Organizations",
                        principalColumn: "OrganizationId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK__1СProviderDocuments_Counterparties_ProviderId",
                        column: x => x.ProviderId,
                        principalTable: "Counterparties",
                        principalColumn: "ITN",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX__1СProviderDocuments_OrganizationId",
                table: "_1СProviderDocuments",
                column: "OrganizationId");

            migrationBuilder.CreateIndex(
                name: "IX__1СProviderDocuments_ProviderId",
                table: "_1СProviderDocuments",
                column: "ProviderId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "_1СProviderDocuments");
        }
    }
}
