namespace Notification.Api.Services.Mail
{
    public class EmailSettings
    {
        public const string EmailSection = "EmailSettings";
        public string FromAddress { get; set; }
        public string ApiKey { get; set; }
    }
}
