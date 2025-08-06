using MassTransit;
using Messaging;
using Microsoft.AspNetCore.Identity;
using PostService.Data;
using PostService.Data.Models;

namespace PostService.Consumers
{
    public class UserRegisteredConsumer(PostDbContexts dbContext) : IConsumer<UserRegistered>
    {
        private readonly PostDbContexts _dbContext = dbContext;

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
            catch(Exception ex)
            {
                await transaction.RollbackAsync();
                throw new Exception("Error while saving data to the database", ex);
            }

        }
    }
}
