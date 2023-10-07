using Notification.Api.Enums;

namespace Notification.Api.Domain.Entities
{
    public class Email
    {
        public EmailType EmailType { get; set; }
        public string DestinationEmail { get; set; }
    }
}
