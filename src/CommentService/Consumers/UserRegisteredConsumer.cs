using CommentService.Data;
using Messaging;
using MassTransit;
using CommentService.Data.Model;

namespace CommentService.Consumers
{
    public class UserRegisteredConsumer(CommentDbContext dbContext) : IConsumer<UserRegistered>
    {
        private readonly CommentDbContext _dbContext = dbContext;

        public async Task Consume(ConsumeContext<UserRegistered> context)
        {
            var msg = context.Message;

            using var transaction = await _dbContext.Database.BeginTransactionAsync();

            try
            {
                var user = new UserInfo()
                {
                    UserId = msg.UserId,
                    UserName = msg.UserName,
                    UserEmail = msg.Email,
                    DateAdded = DateTime.UtcNow
                };

                _dbContext.UsersInfo.Add(user);
                await _dbContext.SaveChangesAsync();


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
