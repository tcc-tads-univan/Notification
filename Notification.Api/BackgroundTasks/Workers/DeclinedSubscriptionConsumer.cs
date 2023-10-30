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
    public class DeclinedSubscriptionConsumer : IConsumer<DeclinedSubscriptionEvent>
    {
        private readonly IUserRepository _userRepository;
        private readonly IEmailService _emailService;
        public DeclinedSubscriptionConsumer(IUserRepository userRepository, IEmailService emailService)
        {
            _userRepository = userRepository;
            _emailService = emailService;
        }
        public async Task Consume(ConsumeContext<DeclinedSubscriptionEvent> context)
        {
            var userContact = await _userRepository.GetUserContactInformation(context.Message.StudentId);

            Email email = new Email
            {
                DestinationEmail = userContact.Email,
                EmailType = EmailType.DECLINED_SUBSCRIPTION
            };

            await _emailService.SendAsync(email);
        }
    }

    public class DeclinedSubscriptionConsumerDefinition : ConsumerDefinition<DeclinedSubscriptionConsumer>
    {
        public DeclinedSubscriptionConsumerDefinition()
        {
            EndpointName = "declined-subscription";
        }
        protected override void ConfigureConsumer(IReceiveEndpointConfigurator endpointConfigurator, IConsumerConfigurator<DeclinedSubscriptionConsumer> consumerConfigurator, IRegistrationContext context)
        {
            if (endpointConfigurator is IRabbitMqReceiveEndpointConfigurator rabbit)
            {
                rabbit.ConfigureConsumeTopology = false;
                rabbit.Bind(BaseUnivanEvent.exchageName, s =>
                {
                    s.RoutingKey = "DeclinedSubscriptionEvent";
                    s.ExchangeType = ExchangeType.Direct;
                });
            }
        }
    }
}
