using MassTransit;
using Notification.Api.Enums;
using Notification.Api.Services.Mail;
using RabbitMQ.Client;
using SharedContracts;
using SharedContracts.Events;

namespace Notification.Api.BackgroundTasks.Workers
{
    public class AcceptedSubscriptionConsumer : IConsumer<AcceptedSubscriptionEvent>
    {
        private readonly IEmailService _emailService;
        public AcceptedSubscriptionConsumer(IEmailService emailService)
        {
            _emailService = emailService;
        }
        public async Task Consume(ConsumeContext<AcceptedSubscriptionEvent> context)
        {
            await _emailService.ExecuteEmail(context.Message.StudentId, context.Message.DriverId, EmailType.ACCEPTED_SUBSCRIPTION);
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
