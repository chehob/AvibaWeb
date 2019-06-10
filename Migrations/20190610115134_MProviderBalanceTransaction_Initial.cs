using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace AvibaWeb.Migrations
{
    public partial class MProviderBalanceTransaction_Initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ProviderBalanceTransaction",
                columns: table => new
                {
                    ProviderBalanceTransactionId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    ProviderId = table.Column<string>(nullable: true),
                    OldBalance = table.Column<decimal>(type: "Money", nullable: false),
                    Amount = table.Column<decimal>(type: "Money", nullable: false),
                    BookingOperationId = table.Column<int>(nullable: false),
                    TypeId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProviderBalanceTransaction", x => x.ProviderBalanceTransactionId);
                    table.ForeignKey(
                        name: "FK_ProviderBalanceTransaction_ProviderBalance_ProviderId",
                        column: x => x.ProviderId,
                        principalTable: "ProviderBalance",
                        principalColumn: "ProviderId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ProviderBalanceTransaction_ProviderId",
                table: "ProviderBalanceTransaction",
                column: "ProviderId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ProviderBalanceTransaction");
        }
    }
}
