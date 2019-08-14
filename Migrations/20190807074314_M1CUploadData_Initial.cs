using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace AvibaWeb.Migrations
{
    public partial class M1CUploadData_Initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "_1CUploadDataId",
                table: "Organizations",
                nullable: true,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "_1CUploadData",
                columns: table => new
                {
                    _1CUploadDataId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CounterpartyId = table.Column<string>(nullable: true),
                    PrincipalDocumentName = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__1CUploadData", x => x._1CUploadDataId);
                    table.ForeignKey(
                        name: "FK__1CUploadData_Counterparties_CounterpartyId",
                        column: x => x.CounterpartyId,
                        principalTable: "Counterparties",
                        principalColumn: "ITN",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Organizations__1CUploadDataId",
                table: "Organizations",
                column: "_1CUploadDataId");

            migrationBuilder.CreateIndex(
                name: "IX__1CUploadData_CounterpartyId",
                table: "_1CUploadData",
                column: "CounterpartyId");

            migrationBuilder.AddForeignKey(
                name: "FK_Organizations__1CUploadData__1CUploadDataId",
                table: "Organizations",
                column: "_1CUploadDataId",
                principalTable: "_1CUploadData",
                principalColumn: "_1CUploadDataId",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Organizations__1CUploadData__1CUploadDataId",
                table: "Organizations");

            migrationBuilder.DropTable(
                name: "_1CUploadData");

            migrationBuilder.DropIndex(
                name: "IX_Organizations__1CUploadDataId",
                table: "Organizations");

            migrationBuilder.DropColumn(
                name: "_1CUploadDataId",
                table: "Organizations");
        }
    }
}
