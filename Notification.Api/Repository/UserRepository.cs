using Dapper;
using Notification.Api.Database.Interfaces;
using Notification.Api.Domain.Entities;

namespace Notification.Api.Repository
{
    public class UserRepository : IUserRepository
    {
        private readonly IDbConnectionFactory _dbContext;
        public UserRepository(IDbConnectionFactory dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task<UserContact> GetUserContactInformation(int userId)
        {
            using var connection = await _dbContext.CreateConnection();
            return await connection.QueryFirstOrDefaultAsync<UserContact>(
                @"SELECT userId, Email FROM UserContact WHERE userId = @UserId", new { UserId = userId });
        }

        public Task SaveUserContact(UserContact userContact)
        {
            throw new NotImplementedException();
        }
    }
}
