using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace AvibaWeb.Migrations
{
    public partial class MAtolPrintOperationsBinding : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            //migrationBuilder.DropPrimaryKey(
            //    name: "PK_VBookingManagementAtolLuggage",
            //    table: "VBookingManagementAtolLuggage");

            //migrationBuilder.DropPrimaryKey(
            //    name: "PK_VBookingManagementAtolKRS",
            //    table: "VBookingManagementAtolKRS");

            //migrationBuilder.DropColumn(
            //    name: "Id",
            //    table: "VBookingManagementAtolKRS");

            //migrationBuilder.AlterColumn<int>(
            //    name: "SegCount",
            //    table: "VBookingManagementAtolTickets",
            //    nullable: false,
            //    oldClrType: typeof(int),
            //    oldDefaultValue: 0);

            //migrationBuilder.AlterColumn<int>(
            //    name: "AtolServerId",
            //    table: "VBookingManagementAtolLuggage",
            //    nullable: false,
            //    oldClrType: typeof(int))
            //    .OldAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            //migrationBuilder.AddColumn<Guid>(
            //    name: "Id",
            //    table: "VBookingManagementAtolLuggage",
            //    nullable: false,
            //    defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            //migrationBuilder.AlterColumn<int>(
            //    name: "AtolServerId",
            //    table: "VBookingManagementAtolKRS",
            //    nullable: false,
            //    oldClrType: typeof(int))
            //    .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            //migrationBuilder.AlterColumn<decimal>(
            //    name: "Amount",
            //    table: "VBookingManagementAtolKRS",
            //    nullable: false,
            //    oldClrType: typeof(decimal),
            //    oldNullable: true);

            //migrationBuilder.AddPrimaryKey(
            //    name: "PK_VBookingManagementAtolLuggage",
            //    table: "VBookingManagementAtolLuggage",
            //    column: "Id");

            //migrationBuilder.AddPrimaryKey(
            //    name: "PK_VBookingManagementAtolKRS",
            //    table: "VBookingManagementAtolKRS",
            //    column: "AtolServerId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            //migrationBuilder.DropPrimaryKey(
            //    name: "PK_VBookingManagementAtolLuggage",
            //    table: "VBookingManagementAtolLuggage");

            //migrationBuilder.DropPrimaryKey(
            //    name: "PK_VBookingManagementAtolKRS",
            //    table: "VBookingManagementAtolKRS");

            //migrationBuilder.DropColumn(
            //    name: "Id",
            //    table: "VBookingManagementAtolLuggage");

            //migrationBuilder.AlterColumn<int>(
            //    name: "SegCount",
            //    table: "VBookingManagementAtolTickets",
            //    nullable: false,
            //    defaultValue: 0,
            //    oldClrType: typeof(int));

            //migrationBuilder.AlterColumn<int>(
            //    name: "AtolServerId",
            //    table: "VBookingManagementAtolLuggage",
            //    nullable: false,
            //    oldClrType: typeof(int))
            //    .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            //migrationBuilder.AlterColumn<decimal>(
            //    name: "Amount",
            //    table: "VBookingManagementAtolKRS",
            //    nullable: true,
            //    oldClrType: typeof(decimal));

            //migrationBuilder.AlterColumn<int>(
            //    name: "AtolServerId",
            //    table: "VBookingManagementAtolKRS",
            //    nullable: false,
            //    oldClrType: typeof(int))
            //    .OldAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            //migrationBuilder.AddColumn<Guid>(
            //    name: "Id",
            //    table: "VBookingManagementAtolKRS",
            //    nullable: false,
            //    defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            //migrationBuilder.AddPrimaryKey(
            //    name: "PK_VBookingManagementAtolLuggage",
            //    table: "VBookingManagementAtolLuggage",
            //    column: "AtolServerId");

            //migrationBuilder.AddPrimaryKey(
            //    name: "PK_VBookingManagementAtolKRS",
            //    table: "VBookingManagementAtolKRS",
            //    column: "Id");
        }
    }
}
