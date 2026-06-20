using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyPortfolioWebsite.Migrations
{
    /// <inheritdoc />
    public partial class AddPublicPortfolioSettings : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsPortfolioPublic",
                table: "AppUsers",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "PortfolioHeadline",
                table: "AppUsers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PublicSlug",
                table: "AppUsers",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsPortfolioPublic",
                table: "AppUsers");

            migrationBuilder.DropColumn(
                name: "PortfolioHeadline",
                table: "AppUsers");

            migrationBuilder.DropColumn(
                name: "PublicSlug",
                table: "AppUsers");
        }
    }
}
