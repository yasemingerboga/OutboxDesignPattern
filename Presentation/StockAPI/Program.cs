using Application.Consumers;
using Application.Handlers;
using Application.Repositories;
using Confluent.Kafka;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Persistance;
using Persistance.Context;
using Persistance.Repositories;
using StockAPI.Consumers;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddHostedService<ConsumerHostedService>();
builder.Services.AddMediatR(typeof(CreateOrderCommandHandler).Assembly);
builder.Services.AddPersistenceServices();

// Add services to the container.
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
//builder.Services.Configure<ConsumerConfig>(builder.Configuration.GetSection(nameof(ConsumerConfig)));
//builder.Services.AddSingleton<IHostedService, OrderCreatedEventConsumer>();
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
