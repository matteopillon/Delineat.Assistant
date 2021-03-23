using Microsoft.EntityFrameworkCore.Migrations;

namespace Delineat.Workflow.Core.SqlServer.Migrations
{
    public partial class ExtraFields : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ExtraFieldId",
                table: "ExtraFieldValues",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ExtraFieldValues_ExtraFieldId",
                table: "ExtraFieldValues",
                column: "ExtraFieldId");

            migrationBuilder.AddForeignKey(
                name: "FK_ExtraFieldValues_ExtraFields_ExtraFieldId",
                table: "ExtraFieldValues",
                column: "ExtraFieldId",
                principalTable: "ExtraFields",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ExtraFieldValues_ExtraFields_ExtraFieldId",
                table: "ExtraFieldValues");

            migrationBuilder.DropIndex(
                name: "IX_ExtraFieldValues_ExtraFieldId",
                table: "ExtraFieldValues");

            migrationBuilder.DropColumn(
                name: "ExtraFieldId",
                table: "ExtraFieldValues");
        }
    }
}
