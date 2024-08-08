using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace hoohub.Migrations
{
    /// <inheritdoc />
    public partial class UserLastLoginAddress : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "LastLoginIpAddress",
                table: "Users",
                type: "TEXT",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LastLoginIpAddress",
                table: "Users");
        }
    }
}
