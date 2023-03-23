using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Coder.Migrations
{
    /// <inheritdoc />
    public partial class studentcontestmap : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "StudentContestMap",
                columns: table => new
                {
                    StudentContestId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ContestId = table.Column<int>(type: "int", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    TotalQuestions = table.Column<int>(type: "int", nullable: true),
                    QuestionsAttended = table.Column<int>(type: "int", nullable: true),
                    TotalEarnedScore = table.Column<double>(type: "float", nullable: true),
                    Status = table.Column<int>(type: "int", nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedOn = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StudentContestMap", x => x.StudentContestId);
                    table.ForeignKey(
                        name: "FK_StudentContestMap_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_StudentContestMap_Contest_ContestId",
                        column: x => x.ContestId,
                        principalTable: "Contest",
                        principalColumn: "ContestId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_StudentContestMap_ContestId",
                table: "StudentContestMap",
                column: "ContestId");

            migrationBuilder.CreateIndex(
                name: "IX_StudentContestMap_UserId",
                table: "StudentContestMap",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "StudentContestMap");
        }
    }
}
