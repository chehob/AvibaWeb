using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace AvibaWeb.Migrations
{
    public partial class MTicketCancelInitial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TicketCancelOperations",
                columns: table => new
                {
                    TicketCancelOperationId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    TicketId = table.Column<int>(nullable: false),
                    UserId = table.Column<string>(maxLength: 128, nullable: true),
                    Description = table.Column<string>(nullable: true),
                    OperationTypeId = table.Column<int>(nullable: false),
                    OperationDateTime = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TicketCancelOperations", x => x.TicketCancelOperationId);
                    table.ForeignKey(
                        name: "FK_TicketCancelOperations_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TicketCancelOperations_UserId",
                table: "TicketCancelOperations",
                column: "UserId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TicketCancelOperations");
        }
    }
}
