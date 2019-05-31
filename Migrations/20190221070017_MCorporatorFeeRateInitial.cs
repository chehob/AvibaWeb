using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace AvibaWeb.Migrations
{
    public partial class MCorporatorFeeRateInitial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CorporatorFeeRates",
                columns: table => new
                {
                    ITN = table.Column<string>(nullable: false),
                    TicketTypeId = table.Column<int>(nullable: false),
                    OperationTypeId = table.Column<int>(nullable: false),
                    Rate = table.Column<decimal>(nullable: false),
                    StartDate = table.Column<DateTime>(nullable: false),
                    EndDate = table.Column<DateTime>(nullable: true),
                    AirlineId = table.Column<string>(nullable: true),
                    PerSegment = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CorporatorFeeRates", x => x.ITN);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CorporatorFeeRates");
        }
    }
}
