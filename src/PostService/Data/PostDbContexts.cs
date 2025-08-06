using Microsoft.EntityFrameworkCore;
using PostService.Data.Models;

namespace PostService.Data
{

    public class PostDbContexts : DbContext
    {
        public PostDbContexts(DbContextOptions options) : base(options)
        {

        }

        public DbSet<Post> Posts { get; set; }
        public DbSet<UserInfo> UsersInfo { get; set; }

    }
}
