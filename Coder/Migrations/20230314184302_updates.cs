using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Coder.Migrations
{
    /// <inheritdoc />
    public partial class updates : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_QuestionContestMap_AspNetUsers_UserId",
                table: "QuestionContestMap");

            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "QuestionContestMap",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AddForeignKey(
                name: "FK_QuestionContestMap_AspNetUsers_UserId",
                table: "QuestionContestMap",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_QuestionContestMap_AspNetUsers_UserId",
                table: "QuestionContestMap");

            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "QuestionContestMap",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_QuestionContestMap_AspNetUsers_UserId",
                table: "QuestionContestMap",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
