using MassTransit;
using Notification.Api.Enums;
using Notification.Api.Services.Mail;
using RabbitMQ.Client;
using SharedContracts;
using SharedContracts.Events;

namespace Notification.Api.BackgroundTasks.Workers
{
    public class DeclinedRideConsumer : IConsumer<DeclinedRideEvent>
    {
        private readonly IEmailService _emailService;
        public DeclinedRideConsumer(IEmailService emailService)
        {
            _emailService = emailService;
        }
        public async Task Consume(ConsumeContext<DeclinedRideEvent> context)
        {
            await _emailService.ExecuteEmail(context.Message.StudentId, context.Message.DriverId, EmailType.DECLINED_RIDE);
        }
    }
    public class DeclinedRideConsumerDefinition : ConsumerDefinition<DeclinedRideConsumer>
    {
        public DeclinedRideConsumerDefinition()
        {
            EndpointName = "declined-ride";
        }
        protected override void ConfigureConsumer(IReceiveEndpointConfigurator endpointConfigurator, IConsumerConfigurator<DeclinedRideConsumer> consumerConfigurator, IRegistrationContext context)
        {
            if (endpointConfigurator is IRabbitMqReceiveEndpointConfigurator rabbit)
            {
                rabbit.ConfigureConsumeTopology = false;
                rabbit.Bind(BaseCarpoolEvent.exchageName, s =>
                {
                    s.RoutingKey = "DeclinedRideEvent";
                    s.ExchangeType = ExchangeType.Direct;
                });
            }
        }
    }
}
