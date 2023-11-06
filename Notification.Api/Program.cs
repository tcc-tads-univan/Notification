using MassTransit;
using Notification.Api.BackgroundTasks.Workers;
using Notification.Api.Database;
using Notification.Api.Database.Interfaces;
using Notification.Api.Repository;
using Notification.Api.Services.Mail;

var builder = WebApplication.CreateBuilder(args);
// Add services to the container.

var config = builder.Configuration;
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.Configure<EmailSettings>(config.GetSection(EmailSettings.EmailSection));
builder.Services.AddSingleton<IDbConnectionFactory>(_ => new DbConnectionFactory(config.GetConnectionString("NotificationDatabase")));
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IEmailRepository, EmailRepository>();

builder.Services.AddMassTransit(x =>
{
    x.AddConsumer<InvitedStudentSubscriptionConsumer>(typeof(InvitedStudentSubscriptionConsumerDefinition));
    x.AddConsumer<AcceptedSubscriptionConsumer>(typeof(AcceptedSubscriptionConsumerDefinition));
    x.AddConsumer<CreatedUserConsumer>(typeof(CreatedUserConsumerDefinition));
    x.AddConsumer<DeclinedSubscriptionConsumer>(typeof(DeclinedSubscriptionConsumerDefinition));
    x.AddConsumer<InviteRideConsumer>(typeof(InviteRideConsumerDefinition));
    x.AddConsumer<DeclinedRideConsumer>(typeof(DeclinedRideConsumerDefinition));

    x.SetKebabCaseEndpointNameFormatter();

    x.UsingRabbitMq((context, busFactoryConfigurator) =>
    {
        busFactoryConfigurator.Host(config.GetConnectionString("RabbitMq"));
        busFactoryConfigurator.ConfigureEndpoints(context);
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
