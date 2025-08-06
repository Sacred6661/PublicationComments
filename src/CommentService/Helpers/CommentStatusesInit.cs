using CommentService.Data;
using CommentService.Data.Model;
using Microsoft.AspNetCore.Identity;
using System.Data;

namespace CommentService.Helpers
{
    public static class CommentStatusesInit
    {
        private static readonly List<string> Statuses = ["Pending", "Accepted", "Rejected"];

        public static async Task SeedStatusesAsync(CommentDbContext dbContext)
        {
            if (!dbContext.CommentStatuses.Any())
            {
                foreach (var status in Statuses)
                {
                    var item = new CommentStatus { Status = status, IsActive = true };
                }
            }
        }
    }
}
