using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace hoohub.Migrations
{
    /// <inheritdoc />
    public partial class ComicLikes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ComicLikes",
                columns: table => new
                {
                    Id = table.Column<string>(type: "TEXT", nullable: false),
                    ComicId = table.Column<string>(type: "TEXT", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    IpAddress = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ComicLikes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ComicLikes_Comics_ComicId",
                        column: x => x.ComicId,
                        principalTable: "Comics",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_ComicLikes_ComicId",
                table: "ComicLikes",
                column: "ComicId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ComicLikes");
        }
    }
}
