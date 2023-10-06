using MassTransit;
using Notification.Api.BackgroundTasks.Events;

namespace Notification.Api.BackgroundTasks.Workers
{
    public class InvitedStudentSubscriptionConsumer : IConsumer<InvitedStudentSubscriptionEvent>
    {
        public Task Consume(ConsumeContext<InvitedStudentSubscriptionEvent> context)
        {
            throw new NotImplementedException();
        }
    }
}
