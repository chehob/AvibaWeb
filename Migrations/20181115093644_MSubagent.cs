using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace AvibaWeb.Migrations
{
    public partial class MSubagent : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "SubagentData",
                columns: table => new
                {
                    SubagentId = table.Column<string>(nullable: false),
                    Balance = table.Column<decimal>(type: "Money", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SubagentData", x => x.SubagentId);
                    table.ForeignKey(
                        name: "FK_SubagentData_Counterparties_SubagentId",
                        column: x => x.SubagentId,
                        principalTable: "Counterparties",
                        principalColumn: "ITN",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SubagentDesk",
                columns: table => new
                {
                    SubagentDeskId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    SubagentId = table.Column<string>(nullable: true),
                    DeskId = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SubagentDesk", x => x.SubagentDeskId);
                    table.ForeignKey(
                        name: "FK_SubagentDesk_Desks_DeskId",
                        column: x => x.DeskId,
                        principalTable: "Desks",
                        principalColumn: "DeskId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SubagentDesk_SubagentData_SubagentId",
                        column: x => x.SubagentId,
                        principalTable: "SubagentData",
                        principalColumn: "SubagentId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SubagentDesk_DeskId",
                table: "SubagentDesk",
                column: "DeskId");

            migrationBuilder.CreateIndex(
                name: "IX_SubagentDesk_SubagentId",
                table: "SubagentDesk",
                column: "SubagentId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SubagentDesk");

            migrationBuilder.DropTable(
                name: "SubagentData");
        }
    }
}
