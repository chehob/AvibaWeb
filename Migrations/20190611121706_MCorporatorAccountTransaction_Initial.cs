using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace AvibaWeb.Migrations
{
    public partial class MCorporatorAccountTransaction_Initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CorporatorAccountTransactions",
                columns: table => new
                {
                    ProviderBalanceTransactionId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    AccountId = table.Column<int>(nullable: false),
                    OldBalance = table.Column<decimal>(type: "Money", nullable: false),
                    Amount = table.Column<decimal>(type: "Money", nullable: false),
                    TransactionItemId = table.Column<int>(nullable: false),
                    TypeId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CorporatorAccountTransactions", x => x.ProviderBalanceTransactionId);
                    table.ForeignKey(
                        name: "FK_CorporatorAccountTransactions_CorporatorAccounts_AccountId",
                        column: x => x.AccountId,
                        principalTable: "CorporatorAccounts",
                        principalColumn: "CorporatorAccountId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CorporatorAccountTransactions_AccountId",
                table: "CorporatorAccountTransactions",
                column: "AccountId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CorporatorAccountTransactions");
        }
    }
}
