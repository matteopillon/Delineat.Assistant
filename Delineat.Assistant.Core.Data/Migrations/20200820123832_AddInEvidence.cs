using Microsoft.EntityFrameworkCore.Migrations;

namespace Delineat.Workflow.Core.SqlServer.Migrations
{
    public partial class AddInEvidence : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "InEvidence",
                table: "DocumentVersions",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "InEvidence",
                table: "DocumentVersions");
        }
    }
}
