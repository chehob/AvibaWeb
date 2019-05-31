using Microsoft.EntityFrameworkCore.Migrations;

namespace AvibaWeb.Migrations
{
    public partial class MTransitAccountComments : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Comment",
                table: "TransitAccountDebits",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Comment",
                table: "TransitAccountCredits",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Comment",
                table: "TransitAccountDebits");

            migrationBuilder.DropColumn(
                name: "Comment",
                table: "TransitAccountCredits");
        }
    }
}
