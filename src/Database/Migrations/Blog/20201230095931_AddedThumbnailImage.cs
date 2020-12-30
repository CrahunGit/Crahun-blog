using Microsoft.EntityFrameworkCore.Migrations;

namespace Database.Migrations.Blog
{
    public partial class AddedThumbnailImage : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ThumbnailImagePath",
                table: "Posts",
                type: "TEXT",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ThumbnailImagePath",
                table: "Posts");
        }
    }
}
