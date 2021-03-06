using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Delineat.Workflow.Core.SqlServer.Migrations
{
    public partial class AddSubJobItemRelation : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "DeleteDate",
                table: "SubJobs",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ExportSyncId",
                table: "SubJobs",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ImportSyncId",
                table: "SubJobs",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "InsertDate",
                table: "SubJobs",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdateDate",
                table: "SubJobs",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "SubJobId",
                table: "Items",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Items_SubJobId",
                table: "Items",
                column: "SubJobId");

            migrationBuilder.AddForeignKey(
                name: "FK_Items_SubJobs_SubJobId",
                table: "Items",
                column: "SubJobId",
                principalTable: "SubJobs",
                principalColumn: "SubJobId",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Items_SubJobs_SubJobId",
                table: "Items");

            migrationBuilder.DropIndex(
                name: "IX_Items_SubJobId",
                table: "Items");

            migrationBuilder.DropColumn(
                name: "DeleteDate",
                table: "SubJobs");

            migrationBuilder.DropColumn(
                name: "ExportSyncId",
                table: "SubJobs");

            migrationBuilder.DropColumn(
                name: "ImportSyncId",
                table: "SubJobs");

            migrationBuilder.DropColumn(
                name: "InsertDate",
                table: "SubJobs");

            migrationBuilder.DropColumn(
                name: "UpdateDate",
                table: "SubJobs");

            migrationBuilder.DropColumn(
                name: "SubJobId",
                table: "Items");
        }
    }
}
