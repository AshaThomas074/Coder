using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Coder.Migrations
{
    /// <inheritdoc />
    public partial class studentbatch : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "ContestName",
                table: "Contest",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "StudentBatchId",
                table: "AspNetUsers",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "StudentBatch",
                columns: table => new
                {
                    StudentBatchId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    StudentBatchName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Status = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StudentBatch", x => x.StudentBatchId);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_StudentBatchId",
                table: "AspNetUsers",
                column: "StudentBatchId");

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUsers_StudentBatch_StudentBatchId",
                table: "AspNetUsers",
                column: "StudentBatchId",
                principalTable: "StudentBatch",
                principalColumn: "StudentBatchId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUsers_StudentBatch_StudentBatchId",
                table: "AspNetUsers");

            migrationBuilder.DropTable(
                name: "StudentBatch");

            migrationBuilder.DropIndex(
                name: "IX_AspNetUsers_StudentBatchId",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "StudentBatchId",
                table: "AspNetUsers");

            migrationBuilder.AlterColumn<string>(
                name: "ContestName",
                table: "Contest",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");
        }
    }
}
