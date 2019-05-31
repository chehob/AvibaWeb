using Microsoft.EntityFrameworkCore.Migrations;

namespace AvibaWeb.Migrations
{
    public partial class MCounterpartiesInitial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Counterparties",
                columns: table => new
                {
                    ITN = table.Column<string>(nullable: false),
                    Name = table.Column<string>(nullable: true),
                    CorrespondentAccount = table.Column<string>(nullable: true),
                    KPP = table.Column<string>(nullable: true),
                    BIK = table.Column<string>(nullable: true),
                    OGRN = table.Column<string>(nullable: true),
                    Phone = table.Column<string>(nullable: true),
                    Address = table.Column<string>(nullable: true),
                    Email = table.Column<string>(nullable: true),
                    BankName = table.Column<string>(nullable: true),
                    BankAccount = table.Column<string>(nullable: true),
                    ManagementName = table.Column<string>(nullable: true),
                    ManagementPosition = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Counterparties", x => x.ITN);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Counterparties");
        }
    }
}
