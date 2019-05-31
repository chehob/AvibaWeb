using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace AvibaWeb.Migrations
{
    public partial class MUserCheckInInitial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "UserCheckIns",
                columns: table => new
                {
                    UserCheckInId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    UserId = table.Column<string>(nullable: true, maxLength: 128),
                    DeskId = table.Column<string>(nullable: true),
                    CheckInDateTime = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserCheckIns", x => x.UserCheckInId);
                    table.ForeignKey(
                        name: "FK_UserCheckIns_Desks_DeskId",
                        column: x => x.DeskId,
                        principalTable: "Desks",
                        principalColumn: "DeskId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_UserCheckIns_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_UserCheckIns_DeskId",
                table: "UserCheckIns",
                column: "DeskId");

            migrationBuilder.CreateIndex(
                name: "IX_UserCheckIns_UserId",
                table: "UserCheckIns",
                column: "UserId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UserCheckIns");
        }
    }
}
