using MassTransit;
using Messaging;
using ProfileService.Data;
using ProfileService.Data.Models;

namespace ProfileService.Consumers
{
    public class UserRegisteredConsumer(ProfileDbContext dbContext) : IConsumer<UserRegistered>
    {
        private readonly ProfileDbContext _dbContext = dbContext;

        public async Task Consume(ConsumeContext<UserRegistered> context)
        {
            var msg = context.Message;

            using var transaction = await _dbContext.Database.BeginTransactionAsync();

            try
            {
                var user = new UserProfile()
                {
                    UserId = msg.UserId,
                    UserName = msg.UserName,
                    Email = msg.Email,
                    DateAdded = DateTime.UtcNow
                };

                _dbContext.UserProfiles.Add(user);
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
