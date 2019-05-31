using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace AvibaWeb.Migrations
{
    public partial class MKRSCancelRequstInitial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "KRSCancelRequests",
                columns: table => new
                {
                    KRSCancelRequestId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    KRSId = table.Column<int>(nullable: false),
                    CashierId = table.Column<string>(nullable: true, maxLength: 128),
                    Description = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_KRSCancelRequests", x => x.KRSCancelRequestId);
                    table.ForeignKey(
                        name: "FK_KRSCancelRequests_AspNetUsers_CashierId",
                        column: x => x.CashierId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "KRSCancelRequestOperations",
                columns: table => new
                {
                    KRSCancelRequestOperationId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    KRSCancelRequestId = table.Column<int>(nullable: false),
                    OperationTypeId = table.Column<int>(nullable: false),
                    OperationDateTime = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_KRSCancelRequestOperations", x => x.KRSCancelRequestOperationId);
                    table.ForeignKey(
                        name: "FK_KRSCancelRequestOperations_KRSCancelRequests_KRSCancelRequestId",
                        column: x => x.KRSCancelRequestId,
                        principalTable: "KRSCancelRequests",
                        principalColumn: "KRSCancelRequestId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_KRSCancelRequestOperations_KRSCancelRequestId",
                table: "KRSCancelRequestOperations",
                column: "KRSCancelRequestId");

            migrationBuilder.CreateIndex(
                name: "IX_KRSCancelRequests_CashierId",
                table: "KRSCancelRequests",
                column: "CashierId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "KRSCancelRequestOperations");

            migrationBuilder.DropTable(
                name: "KRSCancelRequests");

            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "UserCheckIns",
                nullable: true,
                oldClrType: typeof(string),
                oldMaxLength: 128,
                oldNullable: true);
        }
    }
}
