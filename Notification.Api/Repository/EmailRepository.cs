using Dapper;
using Notification.Api.Database.Interfaces;
using Notification.Api.Domain.Entities;

namespace Notification.Api.Repository
{
    public class EmailRepository : IEmailRepository
    {
        private readonly IDbConnectionFactory _dbContext;
        public EmailRepository(IDbConnectionFactory dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<EmailContent> GetEmailContentById(int emailTypeId)
        {
            using var connection = await _dbContext.CreateConnection();
            return await connection.QueryFirstOrDefaultAsync<EmailContent>(
                @"SELECT Id, Subject, TextContent FROM EmailContent WHERE Id = @EmailTypeId", new { EmailTypeId = emailTypeId });
        }
    }
}
