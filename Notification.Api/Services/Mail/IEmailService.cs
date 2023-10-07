using Notification.Api.Domain.Entities;

namespace Notification.Api.Services.Mail
{
    public interface IEmailService
    {
        Task SendAsync(Email email);
    }
}
