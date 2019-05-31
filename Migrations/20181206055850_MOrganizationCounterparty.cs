using Microsoft.EntityFrameworkCore.Migrations;

namespace AvibaWeb.Migrations
{
    public partial class MOrganizationCounterparty : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CounterpartyId",
                table: "Organizations",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Organizations_CounterpartyId",
                table: "Organizations",
                column: "CounterpartyId");

            migrationBuilder.AddForeignKey(
                name: "FK_Organizations_Counterparties_CounterpartyId",
                table: "Organizations",
                column: "CounterpartyId",
                principalTable: "Counterparties",
                principalColumn: "ITN",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Organizations_Counterparties_CounterpartyId",
                table: "Organizations");

            migrationBuilder.DropIndex(
                name: "IX_Organizations_CounterpartyId",
                table: "Organizations");

            migrationBuilder.DropColumn(
                name: "CounterpartyId",
                table: "Organizations");
        }
    }
}
