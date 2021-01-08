using Microsoft.EntityFrameworkCore.Migrations;

namespace Database.Migrations.Blog
{
    public partial class AddedPostSummary : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "PostSummary",
                table: "Posts",
                type: "TEXT",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PostSummary",
                table: "Posts");
        }
    }
}
