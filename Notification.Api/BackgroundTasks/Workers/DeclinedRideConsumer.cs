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
    public class DeclinedRideConsumer : IConsumer<DeclinedRideEvent>
    {
        private readonly IUserRepository _userRepository;
        private readonly IEmailService _emailService;
        public DeclinedRideConsumer(IUserRepository userRepository, IEmailService emailService)
        {
            _userRepository = userRepository;
            _emailService = emailService;
        }
        public async Task Consume(ConsumeContext<DeclinedRideEvent> context)
        {
            var userContact = await _userRepository.GetUserContactInformation(context.Message.DriverId);

            Email email = new Email
            {
                DestinationEmail = userContact.Email,
                EmailType = EmailType.DECLINED_RIDE
            };

            await _emailService.SendAsync(email);
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
