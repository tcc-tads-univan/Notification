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
                @"SELECT UserId, Name, Email FROM UserContact WHERE UserId = @UserId", new { UserId = userId });
        }

        public async Task SaveUserContact(UserContact userContact)
        {
            using var connection = await _dbContext.CreateConnection();
            await connection.ExecuteAsync(
                @"INSERT INTO UserContact(UserId,Name, Email) VALUES (@UserId, @Name,@Email)",
                new { UserId = userContact.UserId, Name = userContact.Name, Email = userContact.Email });
        }
    }
}
