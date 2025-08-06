using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CommentService.Data.Migrations
{
    /// <inheritdoc />
    public partial class FixTypo : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "IsActrive",
                table: "UsersInfo",
                newName: "IsActive");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "IsActive",
                table: "UsersInfo",
                newName: "IsActrive");
        }
    }
}
