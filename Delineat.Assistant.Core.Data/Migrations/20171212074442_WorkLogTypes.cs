using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Delineat.Workflow.Core.SqlServer.Migrations
{
    public partial class WorkLogTypes : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "WorkType",
                table: "WorkLogs");

            migrationBuilder.AddColumn<int>(
                name: "AssignedTo",
                table: "WorkLogs",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "WorkTypeWorkLogTypeId",
                table: "WorkLogs",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "WorkLogTypes",
                columns: table => new
                {
                    WorkLogTypeId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WorkLogTypes", x => x.WorkLogTypeId);
                });

            migrationBuilder.CreateIndex(
                name: "IX_WorkLogs_WorkTypeWorkLogTypeId",
                table: "WorkLogs",
                column: "WorkTypeWorkLogTypeId");

            migrationBuilder.AddForeignKey(
                name: "FK_WorkLogs_WorkLogTypes_WorkTypeWorkLogTypeId",
                table: "WorkLogs",
                column: "WorkTypeWorkLogTypeId",
                principalTable: "WorkLogTypes",
                principalColumn: "WorkLogTypeId",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_WorkLogs_WorkLogTypes_WorkTypeWorkLogTypeId",
                table: "WorkLogs");

            migrationBuilder.DropTable(
                name: "WorkLogTypes");

            migrationBuilder.DropIndex(
                name: "IX_WorkLogs_WorkTypeWorkLogTypeId",
                table: "WorkLogs");

            migrationBuilder.DropColumn(
                name: "AssignedTo",
                table: "WorkLogs");

            migrationBuilder.DropColumn(
                name: "WorkTypeWorkLogTypeId",
                table: "WorkLogs");

            migrationBuilder.AddColumn<int>(
                name: "WorkType",
                table: "WorkLogs",
                nullable: false,
                defaultValue: 0);
        }
    }
}
