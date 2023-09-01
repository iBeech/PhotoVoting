using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PhotoVotingApp.Migrations
{
    /// <inheritdoc />
    public partial class UpdatePhotoEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ImageFileName",
                table: "Photos",
                type: "TEXT",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ImageFileName",
                table: "Photos");
        }
    }
}
