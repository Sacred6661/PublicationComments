using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CommentService.Data.Migrations
{
    /// <inheritdoc />
    public partial class DbUpdateTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "UsersInfo",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UserId",
                table: "UsersInfo");
        }
    }
}
