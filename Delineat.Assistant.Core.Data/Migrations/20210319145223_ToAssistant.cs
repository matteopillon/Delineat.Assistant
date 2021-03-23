using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Delineat.Workflow.Core.SqlServer.Migrations
{
    public partial class ToAssistant : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "CustomerInfo_Completed",
                table: "Jobs",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "CustomerInfo_CompletedByUserId",
                table: "Jobs",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CustomerInfo_EstimatedClosingDate",
                table: "Jobs",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CustomerInfo_Info",
                table: "Jobs",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "CustomerInfo_InvoiceAmount",
                table: "Jobs",
                type: "float",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "CustomerInfo_OrderAmount",
                table: "Jobs",
                type: "float",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CustomerInfo_OrderRef",
                table: "Jobs",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "CustomerInfo_Quotation",
                table: "Jobs",
                type: "float",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CustomerInfo_QuotationRef",
                table: "Jobs",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CustomerInfo_Sent",
                table: "Jobs",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "CustomerInfo_SentByUserId",
                table: "Jobs",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsAbsence",
                table: "Jobs",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "ParentJobId",
                table: "Jobs",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Code",
                table: "Customers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "DayWorkTypes",
                columns: table => new
                {
                    DayWorkTypeId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Code = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DayWorkTypes", x => x.DayWorkTypeId);
                });

            migrationBuilder.CreateTable(
                name: "ExtraFields",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Label = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ValidationExpression = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Type = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExtraFields", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ExtraFieldValues",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    InsertDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdateDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeleteDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ImportSyncId = table.Column<int>(type: "int", nullable: true),
                    ExportSyncId = table.Column<int>(type: "int", nullable: true),
                    NumberValue = table.Column<double>(type: "float", nullable: false),
                    TextValue = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DateTimeValue = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExtraFieldValues", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "HolidayDates",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Day = table.Column<int>(type: "int", nullable: false),
                    Month = table.Column<int>(type: "int", nullable: false),
                    Year = table.Column<int>(type: "int", nullable: false),
                    FormulaId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Minutes = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HolidayDates", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "JobTypes",
                columns: table => new
                {
                    JobTypeId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Color = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_JobTypes", x => x.JobTypeId);
                });

            migrationBuilder.CreateTable(
                name: "Permissions",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Enabled = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Permissions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Roles",
                columns: table => new
                {
                    RoleId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Roles", x => x.RoleId);
                });

            migrationBuilder.CreateTable(
                name: "WeekWorkHours",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    OnMonday = table.Column<double>(type: "float", nullable: false),
                    OnTuesday = table.Column<double>(type: "float", nullable: false),
                    OnWednesday = table.Column<double>(type: "float", nullable: false),
                    OnThursday = table.Column<double>(type: "float", nullable: false),
                    OnFriday = table.Column<double>(type: "float", nullable: false),
                    OnSaturday = table.Column<double>(type: "float", nullable: false),
                    OnSunday = table.Column<double>(type: "float", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WeekWorkHours", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "JobExtraFields",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    JobId = table.Column<int>(type: "int", nullable: true),
                    ExtraFieldId = table.Column<int>(type: "int", nullable: true),
                    InsertDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdateDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeleteDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ImportSyncId = table.Column<int>(type: "int", nullable: true),
                    ExportSyncId = table.Column<int>(type: "int", nullable: true),
                    NumberValue = table.Column<double>(type: "float", nullable: false),
                    TextValue = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DateTimeValue = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_JobExtraFields", x => x.Id);
                    table.ForeignKey(
                        name: "FK_JobExtraFields_ExtraFields_ExtraFieldId",
                        column: x => x.ExtraFieldId,
                        principalTable: "ExtraFields",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_JobExtraFields_Jobs_JobId",
                        column: x => x.JobId,
                        principalTable: "Jobs",
                        principalColumn: "JobId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PermissionRole",
                columns: table => new
                {
                    PermissionsId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    RolesRoleId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PermissionRole", x => new { x.PermissionsId, x.RolesRoleId });
                    table.ForeignKey(
                        name: "FK_PermissionRole_Permissions_PermissionsId",
                        column: x => x.PermissionsId,
                        principalTable: "Permissions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PermissionRole_Roles_RolesRoleId",
                        column: x => x.RolesRoleId,
                        principalTable: "Roles",
                        principalColumn: "RoleId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    UserId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nickname = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FirstName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    HasPaySlip = table.Column<bool>(type: "bit", nullable: false),
                    WeekWorkId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.UserId);
                    table.ForeignKey(
                        name: "FK_Users_WeekWorkHours_WeekWorkId",
                        column: x => x.WeekWorkId,
                        principalTable: "WeekWorkHours",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "DayWorkLogs",
                columns: table => new
                {
                    DayWorkLogId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(type: "int", nullable: true),
                    JobId = table.Column<int>(type: "int", nullable: true),
                    Date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Minutes = table.Column<int>(type: "int", nullable: false),
                    WorkTypeDayWorkTypeId = table.Column<int>(type: "int", nullable: true),
                    Note = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    InsertDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdateDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeleteDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ImportSyncId = table.Column<int>(type: "int", nullable: true),
                    ExportSyncId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DayWorkLogs", x => x.DayWorkLogId);
                    table.ForeignKey(
                        name: "FK_DayWorkLogs_DayWorkTypes_WorkTypeDayWorkTypeId",
                        column: x => x.WorkTypeDayWorkTypeId,
                        principalTable: "DayWorkTypes",
                        principalColumn: "DayWorkTypeId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_DayWorkLogs_Jobs_JobId",
                        column: x => x.JobId,
                        principalTable: "Jobs",
                        principalColumn: "JobId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_DayWorkLogs_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "RoleUser",
                columns: table => new
                {
                    RolesRoleId = table.Column<int>(type: "int", nullable: false),
                    UsersUserId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RoleUser", x => new { x.RolesRoleId, x.UsersUserId });
                    table.ForeignKey(
                        name: "FK_RoleUser_Roles_RolesRoleId",
                        column: x => x.RolesRoleId,
                        principalTable: "Roles",
                        principalColumn: "RoleId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RoleUser_Users_UsersUserId",
                        column: x => x.UsersUserId,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserCredentials",
                columns: table => new
                {
                    CredentialId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Username = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    Password = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PasswordSalt = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MustChangePassword = table.Column<bool>(type: "bit", nullable: false),
                    PasswordExpiration = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UserId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserCredentials", x => x.CredentialId);
                    table.ForeignKey(
                        name: "FK_UserCredentials_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Jobs_CustomerInfo_CompletedByUserId",
                table: "Jobs",
                column: "CustomerInfo_CompletedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Jobs_CustomerInfo_SentByUserId",
                table: "Jobs",
                column: "CustomerInfo_SentByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Jobs_ParentJobId",
                table: "Jobs",
                column: "ParentJobId");

            migrationBuilder.CreateIndex(
                name: "IX_DayWorkLogs_JobId",
                table: "DayWorkLogs",
                column: "JobId");

            migrationBuilder.CreateIndex(
                name: "IX_DayWorkLogs_UserId",
                table: "DayWorkLogs",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_DayWorkLogs_WorkTypeDayWorkTypeId",
                table: "DayWorkLogs",
                column: "WorkTypeDayWorkTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_JobExtraFields_ExtraFieldId",
                table: "JobExtraFields",
                column: "ExtraFieldId");

            migrationBuilder.CreateIndex(
                name: "IX_JobExtraFields_JobId",
                table: "JobExtraFields",
                column: "JobId");

            migrationBuilder.CreateIndex(
                name: "IX_PermissionRole_RolesRoleId",
                table: "PermissionRole",
                column: "RolesRoleId");

            migrationBuilder.CreateIndex(
                name: "IX_RoleUser_UsersUserId",
                table: "RoleUser",
                column: "UsersUserId");

            migrationBuilder.CreateIndex(
                name: "IX_UserCredentials_UserId",
                table: "UserCredentials",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_UserCredentials_Username",
                table: "UserCredentials",
                column: "Username",
                unique: true,
                filter: "[Username] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Users_WeekWorkId",
                table: "Users",
                column: "WeekWorkId");

            migrationBuilder.AddForeignKey(
                name: "FK_Jobs_Jobs_ParentJobId",
                table: "Jobs",
                column: "ParentJobId",
                principalTable: "Jobs",
                principalColumn: "JobId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Jobs_Users_CustomerInfo_CompletedByUserId",
                table: "Jobs",
                column: "CustomerInfo_CompletedByUserId",
                principalTable: "Users",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Jobs_Users_CustomerInfo_SentByUserId",
                table: "Jobs",
                column: "CustomerInfo_SentByUserId",
                principalTable: "Users",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Jobs_Jobs_ParentJobId",
                table: "Jobs");

            migrationBuilder.DropForeignKey(
                name: "FK_Jobs_Users_CustomerInfo_CompletedByUserId",
                table: "Jobs");

            migrationBuilder.DropForeignKey(
                name: "FK_Jobs_Users_CustomerInfo_SentByUserId",
                table: "Jobs");

            migrationBuilder.DropTable(
                name: "DayWorkLogs");

            migrationBuilder.DropTable(
                name: "ExtraFieldValues");

            migrationBuilder.DropTable(
                name: "HolidayDates");

            migrationBuilder.DropTable(
                name: "JobExtraFields");

            migrationBuilder.DropTable(
                name: "JobTypes");

            migrationBuilder.DropTable(
                name: "PermissionRole");

            migrationBuilder.DropTable(
                name: "RoleUser");

            migrationBuilder.DropTable(
                name: "UserCredentials");

            migrationBuilder.DropTable(
                name: "DayWorkTypes");

            migrationBuilder.DropTable(
                name: "ExtraFields");

            migrationBuilder.DropTable(
                name: "Permissions");

            migrationBuilder.DropTable(
                name: "Roles");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropTable(
                name: "WeekWorkHours");

            migrationBuilder.DropIndex(
                name: "IX_Jobs_CustomerInfo_CompletedByUserId",
                table: "Jobs");

            migrationBuilder.DropIndex(
                name: "IX_Jobs_CustomerInfo_SentByUserId",
                table: "Jobs");

            migrationBuilder.DropIndex(
                name: "IX_Jobs_ParentJobId",
                table: "Jobs");

            migrationBuilder.DropColumn(
                name: "CustomerInfo_Completed",
                table: "Jobs");

            migrationBuilder.DropColumn(
                name: "CustomerInfo_CompletedByUserId",
                table: "Jobs");

            migrationBuilder.DropColumn(
                name: "CustomerInfo_EstimatedClosingDate",
                table: "Jobs");

            migrationBuilder.DropColumn(
                name: "CustomerInfo_Info",
                table: "Jobs");

            migrationBuilder.DropColumn(
                name: "CustomerInfo_InvoiceAmount",
                table: "Jobs");

            migrationBuilder.DropColumn(
                name: "CustomerInfo_OrderAmount",
                table: "Jobs");

            migrationBuilder.DropColumn(
                name: "CustomerInfo_OrderRef",
                table: "Jobs");

            migrationBuilder.DropColumn(
                name: "CustomerInfo_Quotation",
                table: "Jobs");

            migrationBuilder.DropColumn(
                name: "CustomerInfo_QuotationRef",
                table: "Jobs");

            migrationBuilder.DropColumn(
                name: "CustomerInfo_Sent",
                table: "Jobs");

            migrationBuilder.DropColumn(
                name: "CustomerInfo_SentByUserId",
                table: "Jobs");

            migrationBuilder.DropColumn(
                name: "IsAbsence",
                table: "Jobs");

            migrationBuilder.DropColumn(
                name: "ParentJobId",
                table: "Jobs");

            migrationBuilder.DropColumn(
                name: "Code",
                table: "Customers");
        }
    }
}
