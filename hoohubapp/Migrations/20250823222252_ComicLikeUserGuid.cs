using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace hoohub.Migrations
{
    /// <inheritdoc />
    public partial class ComicLikeUserGuid : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "UserGuid",
                table: "ComicLikes",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UserGuid",
                table: "ComicLikes");
        }
    }
}
