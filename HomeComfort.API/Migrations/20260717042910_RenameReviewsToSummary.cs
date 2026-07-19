using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HomeComfort.API.Migrations
{
    /// <inheritdoc />
    public partial class RenameReviewsToSummary : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Reviews",
                table: "Products");

            migrationBuilder.AddColumn<string>(
                name: "ReviewSummary",
                table: "Products",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ReviewSummary",
                table: "Products");

            migrationBuilder.AddColumn<int>(
                name: "Reviews",
                table: "Products",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
