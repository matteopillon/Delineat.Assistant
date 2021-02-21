using Microsoft.EntityFrameworkCore.Migrations;
using System;

namespace Delineat.Workflow.Core.SqlServer.Migrations
{
    public partial class TraceWorkAndRelativePath : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "EndDate",
                table: "Items",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ExtimatedBeginDate",
                table: "Items",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ExtimatedEndDate",
                table: "Items",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ExtimatedHour",
                table: "Items",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "StartDate",
                table: "Items",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "Items",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "WorkType",
                table: "Items",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "WorkedHour",
                table: "Items",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "RelativePath",
                table: "DocumentVersions",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EndDate",
                table: "Items");

            migrationBuilder.DropColumn(
                name: "ExtimatedBeginDate",
                table: "Items");

            migrationBuilder.DropColumn(
                name: "ExtimatedEndDate",
                table: "Items");

            migrationBuilder.DropColumn(
                name: "ExtimatedHour",
                table: "Items");

            migrationBuilder.DropColumn(
                name: "StartDate",
                table: "Items");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "Items");

            migrationBuilder.DropColumn(
                name: "WorkType",
                table: "Items");

            migrationBuilder.DropColumn(
                name: "WorkedHour",
                table: "Items");

            migrationBuilder.DropColumn(
                name: "RelativePath",
                table: "DocumentVersions");
        }
    }
}
