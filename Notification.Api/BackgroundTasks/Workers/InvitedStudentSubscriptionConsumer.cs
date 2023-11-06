using MassTransit;
using Notification.Api.Enums;
using Notification.Api.Services.Mail;
using RabbitMQ.Client;
using SharedContracts.Events;

namespace Notification.Api.BackgroundTasks.Workers
{
    public class InvitedStudentSubscriptionConsumer : IConsumer<InvitedStudentSubscriptionEvent>
    {
        private readonly IEmailService _emailService;
        public InvitedStudentSubscriptionConsumer(IEmailService emailService)
        {
            _emailService = emailService;
        }

        public async Task Consume(ConsumeContext<InvitedStudentSubscriptionEvent> context)
        {
            await _emailService.ExecuteEmail(context.Message.DriverId, context.Message.StudentId, EmailType.DRIVER_INVITE_STUDENT_SUB);
        }
    }

    public class InvitedStudentSubscriptionConsumerDefinition : ConsumerDefinition<InvitedStudentSubscriptionConsumer>
    {
        public InvitedStudentSubscriptionConsumerDefinition()
        {
            EndpointName = "invite-subscription";
        }
        protected override void ConfigureConsumer(IReceiveEndpointConfigurator endpointConfigurator, IConsumerConfigurator<InvitedStudentSubscriptionConsumer> consumerConfigurator, IRegistrationContext context)
        {
            if (endpointConfigurator is IRabbitMqReceiveEndpointConfigurator rabbit)
            {
                rabbit.ConfigureConsumeTopology = false;
                rabbit.Bind(BaseUnivanEvent.exchageName, s =>
                {
                    s.RoutingKey = "InvitedStudentSubscriptionEvent";
                    s.ExchangeType = ExchangeType.Direct;
                });
            }
        }
    }
}
