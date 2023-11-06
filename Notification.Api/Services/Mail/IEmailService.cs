using Notification.Api.Domain.Entities;
using Notification.Api.Enums;

namespace Notification.Api.Services.Mail
{
    public interface IEmailService
    {
        Task ExecuteEmail(int fromUserId, int toUserId, EmailType emailType);
    }
}
