using Microsoft.EntityFrameworkCore.Migrations;
using System;

namespace Delineat.Workflow.Core.SqlServer.Migrations
{
    public partial class AddDocumentVersionStatus : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "Reply",
                table: "DocumentVersions",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "DocumentVersions",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "StatusSince",
                table: "DocumentVersions",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "WaitingForReply",
                table: "DocumentVersions",
                type: "datetime2",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Reply",
                table: "DocumentVersions");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "DocumentVersions");

            migrationBuilder.DropColumn(
                name: "StatusSince",
                table: "DocumentVersions");

            migrationBuilder.DropColumn(
                name: "WaitingForReply",
                table: "DocumentVersions");
        }
    }
}
