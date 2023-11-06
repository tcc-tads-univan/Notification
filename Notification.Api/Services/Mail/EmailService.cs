using Microsoft.Extensions.Options;
using Notification.Api.Domain.Entities;
using Notification.Api.Enums;
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
        private readonly IUserRepository _userRepository;

        public EmailService(IOptions<EmailSettings> settings, ILogger<EmailService> logger, IEmailRepository emailRepository, IUserRepository userRepository)
        {
            _settings = settings.Value;
            _logger = logger;
            _emailRepository = emailRepository;
            _userRepository = userRepository;
        }

        public async Task ExecuteEmail(int fromUserId, int toUserId, EmailType emailType) 
        {
            Email email = await BuildEmailContent(fromUserId, toUserId, emailType);
            await SendAsync(email);
        }

        private async Task<Email> BuildEmailContent(int fromUserId, int toUserId, EmailType emailType)
        {
            UserContact from = await _userRepository.GetUserContactInformation(fromUserId);
            UserContact to = await _userRepository.GetUserContactInformation(toUserId);
            var emailContent = await _emailRepository.GetEmailContentById((int)emailType);

            string contentFormated = emailContent.TextContent.Replace("{{name}}", from.Name);
            return new Email(emailContent.Subject, contentFormated, to.Email);
        }
        private async Task SendAsync(Email email)
        {
            var emailClient = new SendGridClient(_settings.ApiKey);

            var fromEmail = new EmailAddress(_settings.FromAddress);
            var destinationEmail = new EmailAddress(email.DestinationEmail);
            var subject = email.Subject;
            var body = email.Content;

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
