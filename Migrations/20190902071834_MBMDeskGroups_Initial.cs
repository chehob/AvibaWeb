using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace AvibaWeb.Migrations
{
    public partial class MBMDeskGroups_Initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "BMDeskGroupId",
                table: "Desks",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "BMDeskGroups",
                columns: table => new
                {
                    BMDeskGroupId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(nullable: true),
                    IsActive = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BMDeskGroups", x => x.BMDeskGroupId);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Desks_BMDeskGroupId",
                table: "Desks",
                column: "BMDeskGroupId");

            migrationBuilder.AddForeignKey(
                name: "FK_Desks_BMDeskGroups_BMDeskGroupId",
                table: "Desks",
                column: "BMDeskGroupId",
                principalTable: "BMDeskGroups",
                principalColumn: "BMDeskGroupId",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Desks_BMDeskGroups_BMDeskGroupId",
                table: "Desks");

            migrationBuilder.DropTable(
                name: "BMDeskGroups");

            migrationBuilder.DropIndex(
                name: "IX_Desks_BMDeskGroupId",
                table: "Desks");

            migrationBuilder.DropColumn(
                name: "BMDeskGroupId",
                table: "Desks");
        }
    }
}
