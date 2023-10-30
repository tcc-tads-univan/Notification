using MassTransit;
using Notification.Api.BackgroundTasks.Workers;
using RabbitMQ.Client;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

var config = builder.Configuration;
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Host.ConfigureServices((hostContext, services) =>
{
    services.AddMassTransit(x =>
    {
        x.AddConsumer<InvitedStudentSubscriptionConsumer>(typeof(InvitedStudentSubscriptionConsumerDefinition));
        x.AddConsumer<AcceptedSubscriptionConsumer>(typeof(AcceptedSubscriptionConsumerDefinition));
        x.AddConsumer<CreatedUserConsumer>(typeof(CreatedUserConsumerDefinition));
        x.AddConsumer<DeclinedSubscriptionConsumer>(typeof(DeclinedSubscriptionConsumerDefinition));

        x.SetKebabCaseEndpointNameFormatter();

        x.UsingRabbitMq((context, busFactoryConfigurator) =>
        {
            busFactoryConfigurator.Host(config.GetConnectionString("RabbitMq"));
            busFactoryConfigurator.ConfigureEndpoints(context);
        });
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
