using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyPortfolioWebsite.Migrations
{
    /// <inheritdoc />
    public partial class AddProfilePortfolioFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "GitHubUrl",
                table: "AppUsers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "JobTitle",
                table: "AppUsers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LinkedInUrl",
                table: "AppUsers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Location",
                table: "AppUsers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Skills",
                table: "AppUsers",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "GitHubUrl",
                table: "AppUsers");

            migrationBuilder.DropColumn(
                name: "JobTitle",
                table: "AppUsers");

            migrationBuilder.DropColumn(
                name: "LinkedInUrl",
                table: "AppUsers");

            migrationBuilder.DropColumn(
                name: "Location",
                table: "AppUsers");

            migrationBuilder.DropColumn(
                name: "Skills",
                table: "AppUsers");
        }
    }
}
