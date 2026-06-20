using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyPortfolioWebsite.Migrations
{
    /// <inheritdoc />
    public partial class AddEmailVerificationFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AlternateEmailVerificationToken",
                table: "AppUsers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "AlternateEmailVerificationTokenExpires",
                table: "AppUsers",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "AlternateEmailVerifiedAt",
                table: "AppUsers",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "EmailVerifiedAt",
                table: "AppUsers",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsAlternateEmailVerified",
                table: "AppUsers",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsEmailVerified",
                table: "AppUsers",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "PendingAlternateEmail",
                table: "AppUsers",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AlternateEmailVerificationToken",
                table: "AppUsers");

            migrationBuilder.DropColumn(
                name: "AlternateEmailVerificationTokenExpires",
                table: "AppUsers");

            migrationBuilder.DropColumn(
                name: "AlternateEmailVerifiedAt",
                table: "AppUsers");

            migrationBuilder.DropColumn(
                name: "EmailVerifiedAt",
                table: "AppUsers");

            migrationBuilder.DropColumn(
                name: "IsAlternateEmailVerified",
                table: "AppUsers");

            migrationBuilder.DropColumn(
                name: "IsEmailVerified",
                table: "AppUsers");

            migrationBuilder.DropColumn(
                name: "PendingAlternateEmail",
                table: "AppUsers");
        }
    }
}
