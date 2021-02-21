using Microsoft.EntityFrameworkCore.Migrations;

namespace Delineat.Workflow.Core.SqlServer.Migrations
{
    public partial class Customers : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CustomerInfo",
                table: "Jobs",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "OrderRef",
                table: "Jobs",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "QuotationRef",
                table: "Jobs",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Code",
                table: "Customers",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CustomerInfo",
                table: "Jobs");

            migrationBuilder.DropColumn(
                name: "OrderRef",
                table: "Jobs");

            migrationBuilder.DropColumn(
                name: "QuotationRef",
                table: "Jobs");

            migrationBuilder.DropColumn(
                name: "Code",
                table: "Customers");
        }
    }
}
