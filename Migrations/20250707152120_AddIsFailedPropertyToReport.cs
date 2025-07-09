using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MakeenBot.Migrations
{
    /// <inheritdoc />
    public partial class AddIsFailedPropertyToReport : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsFailed",
                table: "Reports",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsFailed",
                table: "Reports");
        }
    }
}
