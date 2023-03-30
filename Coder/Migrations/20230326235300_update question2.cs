using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Coder.Migrations
{
    /// <inheritdoc />
    public partial class updatequestion2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "SampleInput",
                table: "Question",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SampleOutput",
                table: "Question",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SampleInput",
                table: "Question");

            migrationBuilder.DropColumn(
                name: "SampleOutput",
                table: "Question");
        }
    }
}
