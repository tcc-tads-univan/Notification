using Notification.Api.Domain.Entities;

namespace Notification.Api.Repository
{
    public interface IEmailRepository
    {
        Task<EmailContent> GetEmailContentById(int emailTypeId);
    }
}
