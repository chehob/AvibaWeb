using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace AvibaWeb.Migrations
{
    public partial class MAtolPrintOperationsBinding2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AtolPrintOperationsBindings",
                columns: table => new
                {
                    AtolPrintOperationsBindingId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    PrintOperationId = table.Column<int>(nullable: false),
                    TypeId = table.Column<int>(nullable: false),
                    _1CStatus = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AtolPrintOperationsBindings", x => x.AtolPrintOperationsBindingId);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AtolPrintOperationsBindings");
        }
    }
}
