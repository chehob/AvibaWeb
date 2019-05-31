using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace AvibaWeb.Migrations
{
    public partial class MCorporatorFeeRateId : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CorporatorFeeRates_Counterparties_ITN",
                table: "CorporatorFeeRates");

            migrationBuilder.DropPrimaryKey(
                name: "PK_CorporatorFeeRates",
                table: "CorporatorFeeRates");

            migrationBuilder.AlterColumn<string>(
                name: "ITN",
                table: "CorporatorFeeRates",
                nullable: true,
                oldClrType: typeof(string));

            migrationBuilder.AddColumn<int>(
                name: "CorporatorFeeRateId",
                table: "CorporatorFeeRates",
                nullable: false,
                defaultValue: 0)
                .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            migrationBuilder.AddPrimaryKey(
                name: "PK_CorporatorFeeRates",
                table: "CorporatorFeeRates",
                column: "CorporatorFeeRateId");

            migrationBuilder.CreateIndex(
                name: "IX_CorporatorFeeRates_ITN",
                table: "CorporatorFeeRates",
                column: "ITN");

            migrationBuilder.AddForeignKey(
                name: "FK_CorporatorFeeRates_Counterparties_ITN",
                table: "CorporatorFeeRates",
                column: "ITN",
                principalTable: "Counterparties",
                principalColumn: "ITN",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CorporatorFeeRates_Counterparties_ITN",
                table: "CorporatorFeeRates");

            migrationBuilder.DropPrimaryKey(
                name: "PK_CorporatorFeeRates",
                table: "CorporatorFeeRates");

            migrationBuilder.DropIndex(
                name: "IX_CorporatorFeeRates_ITN",
                table: "CorporatorFeeRates");

            migrationBuilder.DropColumn(
                name: "CorporatorFeeRateId",
                table: "CorporatorFeeRates");

            migrationBuilder.AlterColumn<string>(
                name: "ITN",
                table: "CorporatorFeeRates",
                nullable: false,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_CorporatorFeeRates",
                table: "CorporatorFeeRates",
                column: "ITN");

            migrationBuilder.AddForeignKey(
                name: "FK_CorporatorFeeRates_Counterparties_ITN",
                table: "CorporatorFeeRates",
                column: "ITN",
                principalTable: "Counterparties",
                principalColumn: "ITN",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
