using Application.Consumers;
using Application.Repositories;
using Microsoft.EntityFrameworkCore;
using PaymentAPI.Consumers;
using PaymentPersistance.Context;
using PaymentPersistance.Repositories;
using Persistance.Repositories;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddDbContext<PaymentDbContext>(options => options.UseSqlServer("Server=localhost,1455;Database=paymentdb; TrustServerCertificate=True; User = sa; Password=password123*"));
builder.Services.AddScoped<IPaymentRepository, PaymentRepository>();
builder.Services.AddScoped<IPaymentOutboxRepository, PaymentOutboxRepository>();
builder.Services.AddScoped<IPaymentInboxRepository, PaymentInboxRepository>();
//builder.Services.AddPersistenceServicesPayment();
builder.Services.AddScoped<IEventConsumer, PaymentRequestEventConsumer>();
builder.Services.AddHostedService<PaymentConsumerHostedService>();


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
