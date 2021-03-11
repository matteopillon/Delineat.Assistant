using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Delineat.Workflow.Core.SqlServer.Migrations
{
    public partial class DayWorkLogToBaseObject : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "DeleteDate",
                table: "DayWorkLogs",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ExportSyncId",
                table: "DayWorkLogs",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ImportSyncId",
                table: "DayWorkLogs",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "InsertDate",
                table: "DayWorkLogs",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdateDate",
                table: "DayWorkLogs",
                type: "datetime2",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DeleteDate",
                table: "DayWorkLogs");

            migrationBuilder.DropColumn(
                name: "ExportSyncId",
                table: "DayWorkLogs");

            migrationBuilder.DropColumn(
                name: "ImportSyncId",
                table: "DayWorkLogs");

            migrationBuilder.DropColumn(
                name: "InsertDate",
                table: "DayWorkLogs");

            migrationBuilder.DropColumn(
                name: "UpdateDate",
                table: "DayWorkLogs");
        }
    }
}
