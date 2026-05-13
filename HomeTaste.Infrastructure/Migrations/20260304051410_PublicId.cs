using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HomeTaste.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class PublicId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "PublicId",
                table: "Meals",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PublicId",
                table: "MealCategories",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PublicId",
                table: "Ingredients",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PublicId",
                table: "Meals");

            migrationBuilder.DropColumn(
                name: "PublicId",
                table: "MealCategories");

            migrationBuilder.DropColumn(
                name: "PublicId",
                table: "Ingredients");
        }
    }
}
