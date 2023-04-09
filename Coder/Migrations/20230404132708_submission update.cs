using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Coder.Migrations
{
    /// <inheritdoc />
    public partial class submissionupdate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "TotalTestCases",
                table: "Submission",
                type: "int",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TotalTestCases",
                table: "Submission");
        }
    }
}
