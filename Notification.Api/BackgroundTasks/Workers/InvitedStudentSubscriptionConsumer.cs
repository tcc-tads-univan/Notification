using MassTransit;
using Notification.Api.BackgroundTasks.Events;
using Notification.Api.Domain.Entities;
using Notification.Api.Enums;
using Notification.Api.Repository;
using Notification.Api.Services.Mail;

namespace Notification.Api.BackgroundTasks.Workers
{
    public class InvitedStudentSubscriptionConsumer : IConsumer<InvitedStudentSubscriptionEvent>
    {
        private readonly IUserRepository _userRepository;
        private readonly IEmailService _emailService;
        public InvitedStudentSubscriptionConsumer(IUserRepository userRepository, IEmailService emailService)
        {
            _userRepository = userRepository;
            _emailService = emailService;
        }
        public async Task Consume(ConsumeContext<InvitedStudentSubscriptionEvent> context)
        {
            var userContact = await _userRepository.GetUserContactInformation(context.Message.userId);

            Email email = new Email
            {
                DestinationEmail = userContact.Email,
                EmailType = EmailType.DRIVER_INVITE_STUDENT_SUB
            };

            await _emailService.SendAsync(email);
        }
    }
}
