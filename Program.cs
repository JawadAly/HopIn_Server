using HopIn_Server.Configurations;
using HopIn_Server.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.Configure<DbSettings>(builder.Configuration.GetSection("MongoDBConfigs"));
//services injected
builder.Services.AddSingleton<VehicleService>();
builder.Services.AddSingleton<MessagingService>();
builder.Services.AddSingleton<ChatService>();
builder.Services.AddSingleton<InboxService>();

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
