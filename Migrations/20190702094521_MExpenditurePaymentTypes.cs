using Microsoft.EntityFrameworkCore.Migrations;

namespace AvibaWeb.Migrations
{
    public partial class MExpenditurePaymentTypes : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "PaymentTypeId",
                table: "Expenditures",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PaymentTypeId",
                table: "Expenditures");
        }
    }
}
