using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using System;

namespace Delineat.Workflow.Core.SqlServer.Migrations
{
    public partial class WorkLog : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
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

            migrationBuilder.CreateTable(
                name: "WorkLogs",
                columns: table => new
                {
                    WorkLogId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    DeleteDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    EndDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ExportSyncId = table.Column<int>(type: "int", nullable: true),
                    ExtimatedBeginDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ExtimatedEndDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ExtimatedHour = table.Column<int>(type: "int", nullable: false),
                    ImportSyncId = table.Column<int>(type: "int", nullable: true),
                    InsertDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    StartDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false),
                    UpdateDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    WorkType = table.Column<int>(type: "int", nullable: false),
                    WorkedHour = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WorkLogs", x => x.WorkLogId);
                });

            migrationBuilder.CreateTable(
                name: "DocumentsWorkLogs",
                columns: table => new
                {
                    DocumentId = table.Column<int>(type: "int", nullable: false),
                    WorkLogId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DocumentsWorkLogs", x => new { x.DocumentId, x.WorkLogId });
                    table.ForeignKey(
                        name: "FK_DocumentsWorkLogs_Documents_DocumentId",
                        column: x => x.DocumentId,
                        principalTable: "Documents",
                        principalColumn: "DocumentId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DocumentsWorkLogs_WorkLogs_WorkLogId",
                        column: x => x.WorkLogId,
                        principalTable: "WorkLogs",
                        principalColumn: "WorkLogId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ItemsWorkLogs",
                columns: table => new
                {
                    ItemId = table.Column<int>(type: "int", nullable: false),
                    WorkLogId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ItemsWorkLogs", x => new { x.ItemId, x.WorkLogId });
                    table.ForeignKey(
                        name: "FK_ItemsWorkLogs_Items_ItemId",
                        column: x => x.ItemId,
                        principalTable: "Items",
                        principalColumn: "ItemId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ItemsWorkLogs_WorkLogs_WorkLogId",
                        column: x => x.WorkLogId,
                        principalTable: "WorkLogs",
                        principalColumn: "WorkLogId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "JobsWorkLogs",
                columns: table => new
                {
                    JobId = table.Column<int>(type: "int", nullable: false),
                    WorkLogId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_JobsWorkLogs", x => new { x.JobId, x.WorkLogId });
                    table.ForeignKey(
                        name: "FK_JobsWorkLogs_Jobs_JobId",
                        column: x => x.JobId,
                        principalTable: "Jobs",
                        principalColumn: "JobId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_JobsWorkLogs_WorkLogs_WorkLogId",
                        column: x => x.WorkLogId,
                        principalTable: "WorkLogs",
                        principalColumn: "WorkLogId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "NotesWorkLogs",
                columns: table => new
                {
                    NoteId = table.Column<int>(type: "int", nullable: false),
                    WorkLogId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NotesWorkLogs", x => new { x.NoteId, x.WorkLogId });
                    table.ForeignKey(
                        name: "FK_NotesWorkLogs_Notes_NoteId",
                        column: x => x.NoteId,
                        principalTable: "Notes",
                        principalColumn: "NoteId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_NotesWorkLogs_WorkLogs_WorkLogId",
                        column: x => x.WorkLogId,
                        principalTable: "WorkLogs",
                        principalColumn: "WorkLogId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DocumentsWorkLogs_WorkLogId",
                table: "DocumentsWorkLogs",
                column: "WorkLogId");

            migrationBuilder.CreateIndex(
                name: "IX_ItemsWorkLogs_WorkLogId",
                table: "ItemsWorkLogs",
                column: "WorkLogId");

            migrationBuilder.CreateIndex(
                name: "IX_JobsWorkLogs_WorkLogId",
                table: "JobsWorkLogs",
                column: "WorkLogId");

            migrationBuilder.CreateIndex(
                name: "IX_NotesWorkLogs_WorkLogId",
                table: "NotesWorkLogs",
                column: "WorkLogId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DocumentsWorkLogs");

            migrationBuilder.DropTable(
                name: "ItemsWorkLogs");

            migrationBuilder.DropTable(
                name: "JobsWorkLogs");

            migrationBuilder.DropTable(
                name: "NotesWorkLogs");

            migrationBuilder.DropTable(
                name: "WorkLogs");

            migrationBuilder.AddColumn<DateTime>(
                name: "EndDate",
                table: "Items",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ExtimatedBeginDate",
                table: "Items",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ExtimatedEndDate",
                table: "Items",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ExtimatedHour",
                table: "Items",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "StartDate",
                table: "Items",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "Items",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "WorkType",
                table: "Items",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "WorkedHour",
                table: "Items",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "EndDate",
                table: "Documents",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ExtimatedBeginDate",
                table: "Documents",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ExtimatedEndDate",
                table: "Documents",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ExtimatedHour",
                table: "Documents",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "StartDate",
                table: "Documents",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "Documents",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "WorkedHour",
                table: "Documents",
                nullable: false,
                defaultValue: 0);
        }
    }
}
