using Notification.Api.Domain.Entities;

namespace Notification.Api.Repository
{
    public interface IUserRepository
    {
        Task<UserContact> GetUserContactInformation(int userId);
        Task SaveUserContact(UserContact userContact);
    }
}
