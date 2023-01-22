using Application.Consumers;
using ApplicationNotification.Consumers;
using ApplicationNotification.Repository;
using Microsoft.EntityFrameworkCore;
using NotificationAPI.Consumers;
using PersistanceNotification.Context;
using PersistanceNotification.Repositories;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddDbContext<NotificationDbContext>(options => options.UseSqlServer("Server=localhost,1466;Database=notificationdb; TrustServerCertificate=True; User = sa; Password=password123*"));
builder.Services.AddScoped<INotificationRepository, NotificationRepository>();
builder.Services.AddScoped<INotificationOutboxRepository, NotificationOutboxRepository>();
//builder.Services.AddPersistenceServicesPayment();
builder.Services.AddScoped<IEventConsumer, NotificationShipmentEventConsumer>();
builder.Services.AddHostedService<NotificationConsumerHostedService>();

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

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
