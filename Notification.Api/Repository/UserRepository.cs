using Notification.Api.Domain.Entities;

namespace Notification.Api.Repository
{
    public class UserRepository : IUserRepository
    {
        public Task<string> GetEmailTextBody(int emailType)
        {
            throw new NotImplementedException();
        }

        public Task<UserContact> GetUserContactInformation(int userId)
        {
            throw new NotImplementedException();
        }
    }
}
