using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Coder.Migrations
{
    /// <inheritdoc />
    public partial class updatesubmission : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Submission",
                columns: table => new
                {
                    SubmissionId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SubmissionContent = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NumberOfTestCasesPassed = table.Column<int>(type: "int", nullable: true),
                    LanguageId = table.Column<int>(type: "int", nullable: true),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    Score = table.Column<double>(type: "float", nullable: true),
                    QuestionContestId = table.Column<int>(type: "int", nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    SubmittedStatus = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Submission", x => x.SubmissionId);
                    table.ForeignKey(
                        name: "FK_Submission_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Submission_Language_LanguageId",
                        column: x => x.LanguageId,
                        principalTable: "Language",
                        principalColumn: "LanguageId");
                    table.ForeignKey(
                        name: "FK_Submission_QuestionContestMap_QuestionContestId",
                        column: x => x.QuestionContestId,
                        principalTable: "QuestionContestMap",
                        principalColumn: "QuestionContestId");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Submission_LanguageId",
                table: "Submission",
                column: "LanguageId");

            migrationBuilder.CreateIndex(
                name: "IX_Submission_QuestionContestId",
                table: "Submission",
                column: "QuestionContestId");

            migrationBuilder.CreateIndex(
                name: "IX_Submission_UserId",
                table: "Submission",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Submission");
        }
    }
}
