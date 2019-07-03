using Microsoft.EntityFrameworkCore.Migrations;

namespace AvibaWeb.Migrations
{
    public partial class MCounterparty_ExpenditureSettings : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ExpenditureDeskGroupId",
                table: "Counterparties",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ExpenditureObjectId",
                table: "Counterparties",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Counterparties_ExpenditureDeskGroupId",
                table: "Counterparties",
                column: "ExpenditureDeskGroupId");

            migrationBuilder.CreateIndex(
                name: "IX_Counterparties_ExpenditureObjectId",
                table: "Counterparties",
                column: "ExpenditureObjectId");

            migrationBuilder.AddForeignKey(
                name: "FK_Counterparties_DeskGroups_ExpenditureDeskGroupId",
                table: "Counterparties",
                column: "ExpenditureDeskGroupId",
                principalTable: "DeskGroups",
                principalColumn: "DeskGroupId",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_Counterparties_ExpenditureObjects_ExpenditureObjectId",
                table: "Counterparties",
                column: "ExpenditureObjectId",
                principalTable: "ExpenditureObjects",
                principalColumn: "ExpenditureObjectId",
                onDelete: ReferentialAction.SetNull);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Counterparties_DeskGroups_ExpenditureDeskGroupId",
                table: "Counterparties");

            migrationBuilder.DropForeignKey(
                name: "FK_Counterparties_ExpenditureObjects_ExpenditureObjectId",
                table: "Counterparties");

            migrationBuilder.DropIndex(
                name: "IX_Counterparties_ExpenditureDeskGroupId",
                table: "Counterparties");

            migrationBuilder.DropIndex(
                name: "IX_Counterparties_ExpenditureObjectId",
                table: "Counterparties");

            migrationBuilder.DropColumn(
                name: "ExpenditureDeskGroupId",
                table: "Counterparties");

            migrationBuilder.DropColumn(
                name: "ExpenditureObjectId",
                table: "Counterparties");
        }
    }
}
