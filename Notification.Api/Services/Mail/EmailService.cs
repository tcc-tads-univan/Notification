using Microsoft.Extensions.Options;
using Notification.Api.Domain.Entities;
using Notification.Api.Repository;
using SendGrid;
using SendGrid.Helpers.Mail;
using System.Net;

namespace Notification.Api.Services.Mail
{
    public class EmailService : IEmailService
    {
        private readonly EmailSettings _settings;
        private readonly ILogger<EmailService> _logger;
        private readonly IEmailRepository _emailRepository;
        public EmailService(IOptions<EmailSettings> settings, ILogger<EmailService> logger, IEmailRepository emailRepository)
        {
            _settings = settings.Value;
            _logger = logger;
            _emailRepository = emailRepository;
        }

        public async Task SendAsync(Email email)
        {
            var emailClient = new SendGridClient(_settings.ApiKey);

            var emailType = await _emailRepository.GetEmailContentById((int)email.EmailType);

            var fromEmail = new EmailAddress(_settings.FromAddress);
            var destinationEmail = new EmailAddress(email.DestinationEmail);
            var subject = emailType.Subject;
            var body = emailType.TextContent;

            var sendGridEmail = MailHelper.CreateSingleEmail(fromEmail, destinationEmail, subject, body, body);
            var response = await emailClient.SendEmailAsync(sendGridEmail);

            if (!EmailSentSuccessfully(response.StatusCode))
            {
                _logger.LogInformation("Error sending email!");
            }
        }

        private bool EmailSentSuccessfully(HttpStatusCode statusCode)
        {
            return statusCode == HttpStatusCode.Accepted ||
                statusCode == HttpStatusCode.OK;
        }
    }
}
