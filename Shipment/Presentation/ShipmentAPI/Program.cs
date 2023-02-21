using ApplicationShipment.Reposiroty;
using Microsoft.EntityFrameworkCore;
using PersistanceShipment.Context;
using PersistanceShipment.Repositories;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddDbContext<ShipmentDbContext>(options => options.UseSqlServer("Server=localhost,1477;Database=shipmentdb; TrustServerCertificate=True; User = sa; Password=password123*"));
builder.Services.AddScoped<IShipmentRepository, ShipmentRepository>();
builder.Services.AddScoped<IShipmentOutboxRepository, ShipmentOutboxRepository>();
builder.Services.AddControllers();

//builder.Services.AddScoped<IEventConsumer, NotificationShipmentEventConsumer>();
//builder.Services.AddHostedService<NotificationConsumerHostedService>();

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
