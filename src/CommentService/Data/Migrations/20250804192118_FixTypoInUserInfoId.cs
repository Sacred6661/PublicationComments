using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CommentService.Data.Migrations
{
    /// <inheritdoc />
    public partial class FixTypoInUserInfoId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "UsedInfoId",
                table: "Comments",
                newName: "UserInfoId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "UserInfoId",
                table: "Comments",
                newName: "UsedInfoId");
        }
    }
}
