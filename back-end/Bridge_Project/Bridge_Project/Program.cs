using Bridge_Project.BacgroundServices;
using Bridge_Project.Data;
using Bridge_Project.Services;
using Bridge_Project.Singleton;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddSingleton<EventTracker>();
builder.Services.AddPersistence(builder.Configuration);
builder.Services.AddServiceLayer();
builder.Services.AddHostedService<EventCheckerService>();
builder.Services.AddHostedService<SourceEventsListenerService>();
builder.Services.AddHostedService<DestinationEventsListenerService>();

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
