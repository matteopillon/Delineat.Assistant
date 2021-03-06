using Microsoft.EntityFrameworkCore.Migrations;

namespace Delineat.Workflow.Core.SqlServer.Migrations
{
    public partial class SubJobCustomer : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CustomerId",
                table: "SubJobs",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_SubJobs_CustomerId",
                table: "SubJobs",
                column: "CustomerId");

            migrationBuilder.AddForeignKey(
                name: "FK_SubJobs_Customers_CustomerId",
                table: "SubJobs",
                column: "CustomerId",
                principalTable: "Customers",
                principalColumn: "CustomerId",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SubJobs_Customers_CustomerId",
                table: "SubJobs");

            migrationBuilder.DropIndex(
                name: "IX_SubJobs_CustomerId",
                table: "SubJobs");

            migrationBuilder.DropColumn(
                name: "CustomerId",
                table: "SubJobs");
        }
    }
}
