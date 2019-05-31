using Microsoft.EntityFrameworkCore.Migrations;

namespace AvibaWeb.Migrations
{
    public partial class MCorpReceipt_NullableTicketOperation : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "TicketOperationId",
                table: "CorporatorReceiptItems",
                nullable: true,
                oldClrType: typeof(int));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "TicketOperationId",
                table: "CorporatorReceiptItems",
                nullable: false,
                oldClrType: typeof(int),
                oldNullable: true);
        }
    }
}
