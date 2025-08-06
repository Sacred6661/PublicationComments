using Microsoft.EntityFrameworkCore;
using ProfileService.Data.Models;
using System.Collections.Generic;

namespace ProfileService.Data
{

    public class ProfileDbContext : DbContext
    {
        public ProfileDbContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<UserProfile> UserProfiles { get; set; }

    }
}
