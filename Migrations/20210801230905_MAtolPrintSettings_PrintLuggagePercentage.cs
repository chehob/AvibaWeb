using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace AvibaWeb.Migrations
{
    public partial class MAtolPrintSettings_PrintLuggagePercentage : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            //migrationBuilder.AlterColumn<DateTime>(
            //    name: "OperationDateTime",
            //    table: "VBookingManagementAtolTickets",
            //    nullable: false,
            //    oldClrType: typeof(DateTime),
            //    oldNullable: true);

            //migrationBuilder.AlterColumn<int>(
            //    name: "AtolServerId",
            //    table: "VBookingManagementAtolTickets",
            //    nullable: false,
            //    oldClrType: typeof(int))
            //    .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            //migrationBuilder.AlterColumn<DateTime>(
            //    name: "OperationDateTime",
            //    table: "VBookingManagementAtolLuggage",
            //    nullable: false,
            //    oldClrType: typeof(DateTime),
            //    oldNullable: true);

            //migrationBuilder.AlterColumn<int>(
            //    name: "AtolServerId",
            //    table: "VBookingManagementAtolLuggage",
            //    nullable: false,
            //    oldClrType: typeof(int))
            //    .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            //migrationBuilder.AlterColumn<DateTime>(
            //    name: "OperationDateTime",
            //    table: "VBookingManagementAtolKRS",
            //    nullable: false,
            //    oldClrType: typeof(DateTime),
            //    oldNullable: true);

            //migrationBuilder.AlterColumn<int>(
            //    name: "AtolServerId",
            //    table: "VBookingManagementAtolKRS",
            //    nullable: false,
            //    oldClrType: typeof(int))
            //    .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            migrationBuilder.AddColumn<int>(
                name: "PrintLuggagePercentage",
                table: "AtolPrintSettings",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PrintLuggagePercentage",
                table: "AtolPrintSettings");

            //migrationBuilder.AlterColumn<DateTime>(
            //    name: "OperationDateTime",
            //    table: "VBookingManagementAtolTickets",
            //    nullable: true,
            //    oldClrType: typeof(DateTime));

            //migrationBuilder.AlterColumn<int>(
            //    name: "AtolServerId",
            //    table: "VBookingManagementAtolTickets",
            //    nullable: false,
            //    oldClrType: typeof(int))
            //    .OldAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            //migrationBuilder.AlterColumn<DateTime>(
            //    name: "OperationDateTime",
            //    table: "VBookingManagementAtolLuggage",
            //    nullable: true,
            //    oldClrType: typeof(DateTime));

            //migrationBuilder.AlterColumn<int>(
            //    name: "AtolServerId",
            //    table: "VBookingManagementAtolLuggage",
            //    nullable: false,
            //    oldClrType: typeof(int))
            //    .OldAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            //migrationBuilder.AlterColumn<DateTime>(
            //    name: "OperationDateTime",
            //    table: "VBookingManagementAtolKRS",
            //    nullable: true,
            //    oldClrType: typeof(DateTime));

            //migrationBuilder.AlterColumn<int>(
            //    name: "AtolServerId",
            //    table: "VBookingManagementAtolKRS",
            //    nullable: false,
            //    oldClrType: typeof(int))
            //    .OldAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);
        }
    }
}
