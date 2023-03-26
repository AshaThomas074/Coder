using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Coder.Migrations
{
    /// <inheritdoc />
    public partial class updatequestion : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_StudentContestMap_Contest_ContestId",
                table: "StudentContestMap");

            migrationBuilder.DropColumn(
                name: "CompletedCount",
                table: "Question");

            migrationBuilder.DropColumn(
                name: "ProcessedCount",
                table: "Question");

            migrationBuilder.DropColumn(
                name: "StartedCount",
                table: "Question");

            migrationBuilder.AlterColumn<int>(
                name: "ContestId",
                table: "StudentContestMap",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "Status",
                table: "Contest",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "PublishedStatus",
                table: "Contest",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddForeignKey(
                name: "FK_StudentContestMap_Contest_ContestId",
                table: "StudentContestMap",
                column: "ContestId",
                principalTable: "Contest",
                principalColumn: "ContestId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_StudentContestMap_Contest_ContestId",
                table: "StudentContestMap");

            migrationBuilder.AlterColumn<int>(
                name: "ContestId",
                table: "StudentContestMap",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "CompletedCount",
                table: "Question",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "ProcessedCount",
                table: "Question",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "StartedCount",
                table: "Question",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AlterColumn<int>(
                name: "Status",
                table: "Contest",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "PublishedStatus",
                table: "Contest",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_StudentContestMap_Contest_ContestId",
                table: "StudentContestMap",
                column: "ContestId",
                principalTable: "Contest",
                principalColumn: "ContestId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
