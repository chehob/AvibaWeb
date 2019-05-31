using Microsoft.EntityFrameworkCore.Migrations;

namespace AvibaWeb.Migrations
{
    public partial class MOrganizationFiles : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "SignatureFileName",
                table: "Organizations",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "StampFileName",
                table: "Organizations",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SignatureFileName",
                table: "Organizations");

            migrationBuilder.DropColumn(
                name: "StampFileName",
                table: "Organizations");
        }
    }
}
