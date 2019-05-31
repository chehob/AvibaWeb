using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace AvibaWeb.Migrations
{
    public partial class MProviderBingdingId : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProviderBinding_Counterparties_ProviderId",
                table: "ProviderBinding");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ProviderBinding",
                table: "ProviderBinding");

            migrationBuilder.AlterColumn<string>(
                name: "ProviderId",
                table: "ProviderBinding",
                nullable: true,
                oldClrType: typeof(string));

            migrationBuilder.AddColumn<int>(
                name: "ProviderBindingId",
                table: "ProviderBinding",
                nullable: false,
                defaultValue: 0)
                .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            migrationBuilder.AddPrimaryKey(
                name: "PK_ProviderBinding",
                table: "ProviderBinding",
                column: "ProviderBindingId");

            migrationBuilder.CreateIndex(
                name: "IX_ProviderBinding_ProviderId",
                table: "ProviderBinding",
                column: "ProviderId",
                unique: true,
                filter: "[ProviderId] IS NOT NULL");

            migrationBuilder.AddForeignKey(
                name: "FK_ProviderBinding_Counterparties_ProviderId",
                table: "ProviderBinding",
                column: "ProviderId",
                principalTable: "Counterparties",
                principalColumn: "ITN",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProviderBinding_Counterparties_ProviderId",
                table: "ProviderBinding");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ProviderBinding",
                table: "ProviderBinding");

            migrationBuilder.DropIndex(
                name: "IX_ProviderBinding_ProviderId",
                table: "ProviderBinding");

            migrationBuilder.DropColumn(
                name: "ProviderBindingId",
                table: "ProviderBinding");

            migrationBuilder.AlterColumn<string>(
                name: "ProviderId",
                table: "ProviderBinding",
                nullable: false,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_ProviderBinding",
                table: "ProviderBinding",
                column: "ProviderId");

            migrationBuilder.AddForeignKey(
                name: "FK_ProviderBinding_Counterparties_ProviderId",
                table: "ProviderBinding",
                column: "ProviderId",
                principalTable: "Counterparties",
                principalColumn: "ITN",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
