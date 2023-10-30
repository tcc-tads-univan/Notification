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
    public class AcceptedSubscriptionConsumer : IConsumer<AcceptedSubscriptionEvent>
    {
        private readonly IUserRepository _userRepository;
        private readonly IEmailService _emailService;
        public AcceptedSubscriptionConsumer(IUserRepository userRepository, IEmailService emailService)
        {
            _userRepository = userRepository;
            _emailService = emailService;
        }
        public async Task Consume(ConsumeContext<AcceptedSubscriptionEvent> context)
        {
            var userContact = await _userRepository.GetUserContactInformation(context.Message.StudentId);

            Email email = new Email
            {
                DestinationEmail = userContact.Email,
                EmailType = EmailType.ACCEPTED_SUBSCRIPTION
            };

            await _emailService.SendAsync(email);
        }
    }

    public class AcceptedSubscriptionConsumerDefinition : ConsumerDefinition<AcceptedSubscriptionConsumer>
    {
        public AcceptedSubscriptionConsumerDefinition()
        {
            EndpointName = "accepted-subscription";
        }
        protected override void ConfigureConsumer(IReceiveEndpointConfigurator endpointConfigurator, IConsumerConfigurator<AcceptedSubscriptionConsumer> consumerConfigurator, IRegistrationContext context)
        {
            if (endpointConfigurator is IRabbitMqReceiveEndpointConfigurator rabbit)
            {
                rabbit.ConfigureConsumeTopology = false;
                rabbit.Bind(BaseUnivanEvent.exchageName, s =>
                {
                    s.RoutingKey = "AcceptedSubscriptionEvent";
                    s.ExchangeType = ExchangeType.Direct;
                });
            }
        }
    }
}
