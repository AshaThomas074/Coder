using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Coder.Migrations
{
    /// <inheritdoc />
    public partial class updatecontestmap : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Submission_QuestionContestMap_QuestionContestId",
                table: "Submission");

            migrationBuilder.DropColumn(
                name: "ProcessingCount",
                table: "QuestionContestMap");

            migrationBuilder.AlterColumn<int>(
                name: "QuestionContestId",
                table: "Submission",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "AttendedOn",
                table: "StudentContestMap",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CompletedOn",
                table: "StudentContestMap",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Submission_QuestionContestMap_QuestionContestId",
                table: "Submission",
                column: "QuestionContestId",
                principalTable: "QuestionContestMap",
                principalColumn: "QuestionContestId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Submission_QuestionContestMap_QuestionContestId",
                table: "Submission");

            migrationBuilder.DropColumn(
                name: "AttendedOn",
                table: "StudentContestMap");

            migrationBuilder.DropColumn(
                name: "CompletedOn",
                table: "StudentContestMap");

            migrationBuilder.AlterColumn<int>(
                name: "QuestionContestId",
                table: "Submission",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<int>(
                name: "ProcessingCount",
                table: "QuestionContestMap",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddForeignKey(
                name: "FK_Submission_QuestionContestMap_QuestionContestId",
                table: "Submission",
                column: "QuestionContestId",
                principalTable: "QuestionContestMap",
                principalColumn: "QuestionContestId");
        }
    }
}
