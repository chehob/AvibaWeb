using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace AvibaWeb.Migrations
{
    public partial class MCounterpartyTypesAdd : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "TypeId",
                table: "Counterparties",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "CounterpartyTypes",
                columns: table => new
                {
                    CounterpartyTypeId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Description = table.Column<string>(nullable: true),
                    IsActive = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CounterpartyTypes", x => x.CounterpartyTypeId);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Counterparties_TypeId",
                table: "Counterparties",
                column: "TypeId");

            migrationBuilder.AddForeignKey(
                name: "FK_Counterparties_CounterpartyTypes_TypeId",
                table: "Counterparties",
                column: "TypeId",
                principalTable: "CounterpartyTypes",
                principalColumn: "CounterpartyTypeId",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Counterparties_CounterpartyTypes_TypeId",
                table: "Counterparties");

            migrationBuilder.DropTable(
                name: "CounterpartyTypes");

            migrationBuilder.DropIndex(
                name: "IX_Counterparties_TypeId",
                table: "Counterparties");

            migrationBuilder.DropColumn(
                name: "TypeId",
                table: "Counterparties");
        }
    }
}
