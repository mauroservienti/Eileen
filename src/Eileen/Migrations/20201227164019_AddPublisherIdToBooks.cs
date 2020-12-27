using Microsoft.EntityFrameworkCore.Migrations;

namespace Eileen.Migrations
{
    public partial class AddPublisherIdToBooks : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "PublisherId",
                table: "Books",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PublisherId",
                table: "Books");
        }
    }
}
