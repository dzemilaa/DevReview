using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DevReview.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class EnsureReviewIsApprovedColumn : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
                """
                ALTER TABLE "Reviews" ADD COLUMN IF NOT EXISTS "IsApproved" boolean NOT NULL DEFAULT false;
                """);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsApproved",
                table: "Reviews");
        }
    }
}
