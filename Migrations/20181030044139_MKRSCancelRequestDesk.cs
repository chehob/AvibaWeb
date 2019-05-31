using Microsoft.EntityFrameworkCore.Migrations;

namespace AvibaWeb.Migrations
{
    public partial class MKRSCancelRequestDesk : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "DeskId",
                table: "KRSCancelRequests",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_KRSCancelRequests_DeskId",
                table: "KRSCancelRequests",
                column: "DeskId");

            migrationBuilder.AddForeignKey(
                name: "FK_KRSCancelRequests_Desks_DeskId",
                table: "KRSCancelRequests",
                column: "DeskId",
                principalTable: "Desks",
                principalColumn: "DeskId",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_KRSCancelRequests_Desks_DeskId",
                table: "KRSCancelRequests");

            migrationBuilder.DropIndex(
                name: "IX_KRSCancelRequests_DeskId",
                table: "KRSCancelRequests");

            migrationBuilder.DropColumn(
                name: "DeskId",
                table: "KRSCancelRequests");
        }
    }
}
