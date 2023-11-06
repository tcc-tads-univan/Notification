using Notification.Api.Enums;

namespace Notification.Api.Domain.Entities
{
    public class Email
    {
        public Email(string emailSubject, string emailContent, string destination)
        {
            Content = emailContent;
            Subject = emailSubject;
            DestinationEmail = destination;
        }
        public string DestinationEmail { get; set; }
        public string Content { get; set; }
        public string Subject { get; set; }
    }
}
