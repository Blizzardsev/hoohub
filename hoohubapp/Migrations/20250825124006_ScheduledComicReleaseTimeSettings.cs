using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace hoohub.Migrations
{
    /// <inheritdoc />
    public partial class ScheduledComicReleaseTimeSettings : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<TimeOnly>(
                name: "ScheduledComicReleaseTime",
                table: "Settings",
                type: "time without time zone",
                nullable: false,
                defaultValue: new TimeOnly(12, 0, 0));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ScheduledComicReleaseTime",
                table: "Settings");
        }
    }
}
