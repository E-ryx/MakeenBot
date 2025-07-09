using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MakeenBot.Migrations
{
    /// <inheritdoc />
    public partial class RemovePhoneNumberAndBaleIdFromStudent : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Reports_Courses_CourseId",
                table: "Reports");

            migrationBuilder.DropIndex(
                name: "IX_Reports_CourseId",
                table: "Reports");

            migrationBuilder.DropColumn(
                name: "BaleId",
                table: "Students");

            migrationBuilder.DropColumn(
                name: "PhoneNumber",
                table: "Students");

            migrationBuilder.DropColumn(
                name: "CourseId",
                table: "Reports");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "BaleId",
                table: "Students",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "PhoneNumber",
                table: "Students",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "CourseId",
                table: "Reports",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Reports_CourseId",
                table: "Reports",
                column: "CourseId");

            migrationBuilder.AddForeignKey(
                name: "FK_Reports_Courses_CourseId",
                table: "Reports",
                column: "CourseId",
                principalTable: "Courses",
                principalColumn: "Id");
        }
    }
}
