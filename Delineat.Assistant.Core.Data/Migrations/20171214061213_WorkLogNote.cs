using Microsoft.EntityFrameworkCore.Migrations;

namespace Delineat.Workflow.Core.SqlServer.Migrations
{
    public partial class WorkLogNote : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Order",
                table: "WorkLogTypes",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Note",
                table: "WorkLogs",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Order",
                table: "WorkLogTypes");

            migrationBuilder.DropColumn(
                name: "Note",
                table: "WorkLogs");
        }
    }
}
