using CommentService.Data;
using MassTransit;
using Messaging;

namespace CommentService.Consumers
{
    public class ProfileEditedConsumer(CommentDbContext dbContext) : IConsumer<ProfileEdited>
    {
        private readonly CommentDbContext _dbContext = dbContext;

        public async Task Consume(ConsumeContext<ProfileEdited> context)
        {
            var msg = context.Message;

            var user = _dbContext.UsersInfo.Where(u => u.UserId.Contains(msg.UserId))?.FirstOrDefault();

            using var transaction = await _dbContext.Database.BeginTransactionAsync();

            try
            {
                if(user != null)
                {
                    if (!string.IsNullOrEmpty(msg.FirstName))
                        user.FirstName = msg.FirstName;
                    if (!string.IsNullOrEmpty(msg.LastName))
                        user.LastName = msg.LastName;

                    user.DateUpdated = DateTime.UtcNow;

                    await _dbContext.SaveChangesAsync();
                }
                
                await transaction.CommitAsync();
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                throw new Exception("Error while saving data to the database", ex);
            }

        }
    }
}
