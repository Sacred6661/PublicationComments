using CommentService.Data;
using MassTransit;
using Messaging;
using Microsoft.EntityFrameworkCore;
using CommentService.Data.Model;

namespace CommentService.Consumers
{
    public class CensorCheckedConsumer(CommentDbContext dbContext) : IConsumer<CensorChecked>
    {
        private readonly CommentDbContext _dbContext = dbContext;

        public async Task Consume(ConsumeContext<CensorChecked> context)
        {
            var msg = context.Message;
            var isBanned = msg.HasBannedWords;
            var commentId = msg.CommentId;

            var comment = await _dbContext.Comments.FirstOrDefaultAsync(c => c.Id == commentId);

            if(comment != null)
            {
                if (isBanned)
                    comment.Status = CommentStatusEnum.Rejected;
                else
                    comment.Status = CommentStatusEnum.Approved;

                await _dbContext.SaveChangesAsync();
            }
        }
    }
}
