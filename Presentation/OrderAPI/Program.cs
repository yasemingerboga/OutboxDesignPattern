using Application;
using Application.Consumers;
using Application.Handlers;
using Application.Repositories;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistance;
using Persistance.Context;
using Persistance.Repositories;
using Quartz;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
//builder.Services.AddMediatR(typeof(CreateOrderCommandHandler).Assembly);
builder.Services.AddApplicationServices();
builder.Services.AddPersistenceServices();
//builder.Services.AddScoped<IEventConsumer, PaymentResponseEventConsumer>();

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
//builder.Services.AddHostedService<Worker>();
//builder.Services.AddHostedService<PaymentResponseEventConsumerHostedService>();

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
