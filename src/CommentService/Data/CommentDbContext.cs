using CommentService.Data.Model;
using Microsoft.EntityFrameworkCore;

namespace CommentService.Data
{
    public class CommentDbContext : DbContext
    {
        public CommentDbContext(DbContextOptions options) : base(options)
        {

        }

        public DbSet<Comment> Comments { get; set; }
        public DbSet<CommentStatus> CommentStatuses { get; set; }
        public DbSet<UserInfo> UsersInfo { get; set; }
    }
}
