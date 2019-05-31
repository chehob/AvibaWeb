using Microsoft.EntityFrameworkCore.Migrations;

namespace AvibaWeb.Migrations
{
    public partial class KRSCancelRequestManager : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "KRSCancelRequestOperations",
                maxLength: 128,
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_KRSCancelRequestOperations_UserId",
                table: "KRSCancelRequestOperations",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_KRSCancelRequestOperations_AspNetUsers_UserId",
                table: "KRSCancelRequestOperations",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_KRSCancelRequestOperations_AspNetUsers_UserId",
                table: "KRSCancelRequestOperations");

            migrationBuilder.DropIndex(
                name: "IX_KRSCancelRequestOperations_UserId",
                table: "KRSCancelRequestOperations");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "KRSCancelRequestOperations");
        }
    }
}
