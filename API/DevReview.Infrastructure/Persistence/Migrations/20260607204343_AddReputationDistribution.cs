using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DevReview.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddReputationDistribution : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Score1Count",
                table: "MentorReputations",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Score2Count",
                table: "MentorReputations",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Score3Count",
                table: "MentorReputations",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Score4Count",
                table: "MentorReputations",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Score5Count",
                table: "MentorReputations",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Score1Count",
                table: "MentorReputations");

            migrationBuilder.DropColumn(
                name: "Score2Count",
                table: "MentorReputations");

            migrationBuilder.DropColumn(
                name: "Score3Count",
                table: "MentorReputations");

            migrationBuilder.DropColumn(
                name: "Score4Count",
                table: "MentorReputations");

            migrationBuilder.DropColumn(
                name: "Score5Count",
                table: "MentorReputations");
        }
    }
}
