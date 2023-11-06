using MassTransit;
using Notification.Api.Enums;
using Notification.Api.Services.Mail;
using RabbitMQ.Client;
using SharedContracts;
using SharedContracts.Events;

namespace Notification.Api.BackgroundTasks.Workers
{
    public class DeclinedSubscriptionConsumer : IConsumer<DeclinedSubscriptionEvent>
    {
        private readonly IEmailService _emailService;
        public DeclinedSubscriptionConsumer(IEmailService emailService)
        {
            _emailService = emailService;
        }
        public async Task Consume(ConsumeContext<DeclinedSubscriptionEvent> context)
        {
            await _emailService.ExecuteEmail(context.Message.StudentId, context.Message.DriverId, EmailType.DECLINED_SUBSCRIPTION);
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
