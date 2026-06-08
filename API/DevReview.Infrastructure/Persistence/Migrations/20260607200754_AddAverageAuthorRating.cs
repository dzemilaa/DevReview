using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DevReview.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddAverageAuthorRating : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "AverageAuthorRating",
                table: "Users",
                type: "numeric",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<int>(
                name: "TotalAuthorRatings",
                table: "Users",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AverageAuthorRating",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "TotalAuthorRatings",
                table: "Users");
        }
    }
}
