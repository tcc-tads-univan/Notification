using MassTransit;
using Notification.Api.Enums;
using Notification.Api.Services.Mail;
using RabbitMQ.Client;
using SharedContracts;
using SharedContracts.Events;

namespace Notification.Api.BackgroundTasks.Workers
{
    public class InviteRideConsumer : IConsumer<InvitedRideEvent>
    {
        private readonly IEmailService _emailService;
        public InviteRideConsumer(IEmailService emailService)
        {
            _emailService = emailService;
        }
        public async Task Consume(ConsumeContext<InvitedRideEvent> context)
        {
            await _emailService.ExecuteEmail(context.Message.DriverId, context.Message.StudentId, EmailType.INVITE_RIDE);
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
