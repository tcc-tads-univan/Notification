using MassTransit;
using Notification.Api.Domain.Entities;
using Notification.Api.Enums;
using Notification.Api.Repository;
using Notification.Api.Services.Mail;
using RabbitMQ.Client;
using SharedContracts;
using SharedContracts.Events;

namespace Notification.Api.BackgroundTasks.Workers
{
    public class InviteRideConsumer : IConsumer<InvitedRideEvent>
    {
        private readonly IUserRepository _userRepository;
        private readonly IEmailService _emailService;
        public InviteRideConsumer(IUserRepository userRepository, IEmailService emailService)
        {
            _userRepository = userRepository;
            _emailService = emailService;
        }
        public async Task Consume(ConsumeContext<InvitedRideEvent> context)
        {
            var userContact = await _userRepository.GetUserContactInformation(context.Message.StudentId);

            Email email = new Email
            {
                DestinationEmail = userContact.Email,
                EmailType = EmailType.INVITE_RIDE
            };

            await _emailService.SendAsync(email);
        }
    }
    public class InviteRideConsumerDefinition : ConsumerDefinition<InviteRideConsumer>
    {
        public InviteRideConsumerDefinition()
        {
            EndpointName = "invite-ride";
        }
        protected override void ConfigureConsumer(IReceiveEndpointConfigurator endpointConfigurator, IConsumerConfigurator<InviteRideConsumer> consumerConfigurator, IRegistrationContext context)
        {
            if (endpointConfigurator is IRabbitMqReceiveEndpointConfigurator rabbit)
            {
                rabbit.ConfigureConsumeTopology = false;
                rabbit.Bind(BaseCarpoolEvent.exchageName, s =>
                {
                    s.RoutingKey = "InvitedRideEvent";
                    s.ExchangeType = ExchangeType.Direct;
                });
            }
        }
    }
}
