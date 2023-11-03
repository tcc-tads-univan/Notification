using MassTransit;
using Notification.Api.Domain.Entities;
using Notification.Api.Repository;
using Notification.Api.Services.Mail;
using RabbitMQ.Client;
using SharedContracts;
using SharedContracts.Events;

namespace Notification.Api.BackgroundTasks.Workers
{
    public class CreatedUserConsumer : IConsumer<CreatedUserEvent>
    {
        private readonly IUserRepository _userRepository;
        public CreatedUserConsumer(IUserRepository userRepository, IEmailService emailService)
        {
            _userRepository = userRepository;
        }
        public async Task Consume(ConsumeContext<CreatedUserEvent> context)
        {
            var userContact = new UserContact()
            {
                UserId = context.Message.UserId,
                Email = context.Message.Email,
                Name = context.Message.Name
            };
            await _userRepository.SaveUserContact(userContact);
        }
    }
    public class CreatedUserConsumerDefinition : ConsumerDefinition<CreatedUserConsumer>
    {
        public CreatedUserConsumerDefinition()
        {
            EndpointName = "created-user";
        }
        protected override void ConfigureConsumer(IReceiveEndpointConfigurator endpointConfigurator, IConsumerConfigurator<CreatedUserConsumer> consumerConfigurator, IRegistrationContext context)
        {
            if (endpointConfigurator is IRabbitMqReceiveEndpointConfigurator rabbit)
            {
                rabbit.ConfigureConsumeTopology = false;
                rabbit.Bind(BaseUnivanEvent.exchageName, s =>
                {
                    s.RoutingKey = "CreatedUserEvent";
                    s.ExchangeType = ExchangeType.Direct;
                });
            }
        }
    }
}
