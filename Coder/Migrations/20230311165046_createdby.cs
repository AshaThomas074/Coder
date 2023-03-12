using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Coder.Migrations
{
    /// <inheritdoc />
    public partial class createdby : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CreatedBy",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Question",
                columns: table => new
                {
                    QuestionId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    QuestionHeading = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    QuestionText = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TestCaseInput1 = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TestCaseOutput1 = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TestCaseInput2 = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TestCaseOutput2 = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TestCaseInput3 = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TestCaseOutput3 = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Answer = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FinalDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Score = table.Column<float>(type: "real", nullable: true),
                    Difficulty = table.Column<int>(type: "int", nullable: false),
                    StartedCount = table.Column<int>(type: "int", nullable: false),
                    ProcessedCount = table.Column<int>(type: "int", nullable: false),
                    CompletedCount = table.Column<int>(type: "int", nullable: false),
                    TeacherId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Question", x => x.QuestionId);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Question");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "AspNetUsers");
        }
    }
}
