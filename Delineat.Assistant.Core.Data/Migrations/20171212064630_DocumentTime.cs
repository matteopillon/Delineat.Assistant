using Microsoft.EntityFrameworkCore.Migrations;
using System;

namespace Delineat.Workflow.Core.SqlServer.Migrations
{
    public partial class DocumentTime : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "EndDate",
                table: "Documents",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ExtimatedBeginDate",
                table: "Documents",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ExtimatedEndDate",
                table: "Documents",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ExtimatedHour",
                table: "Documents",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "StartDate",
                table: "Documents",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "Documents",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "WorkedHour",
                table: "Documents",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EndDate",
                table: "Documents");

            migrationBuilder.DropColumn(
                name: "ExtimatedBeginDate",
                table: "Documents");

            migrationBuilder.DropColumn(
                name: "ExtimatedEndDate",
                table: "Documents");

            migrationBuilder.DropColumn(
                name: "ExtimatedHour",
                table: "Documents");

            migrationBuilder.DropColumn(
                name: "StartDate",
                table: "Documents");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "Documents");

            migrationBuilder.DropColumn(
                name: "WorkedHour",
                table: "Documents");
        }
    }
}
