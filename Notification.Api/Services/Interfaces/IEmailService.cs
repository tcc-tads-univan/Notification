namespace Notification.Api.Services.Interfaces
{
    public interface IEmailService
    {
        Task Send(int userId, string message);
    }
}
