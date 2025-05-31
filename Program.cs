using HopIn_Server.Configurations;
using HopIn_Server.Services;
using HopIn_Server.Hubs;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<DbSettings>(builder.Configuration.GetSection("MongoDBConfigs"));
builder.Services.AddSingleton<VehicleService>();
builder.Services.AddSingleton<UserService>();


builder.Services.AddSingleton<MessagingService>();
builder.Services.AddSingleton<ChatService>();
builder.Services.AddSingleton<InboxService>();
builder.Services.AddSingleton<RideService>();






builder.Services.AddSignalR();



builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddCors(options =>
{
	options.AddPolicy("AllowAll", policy =>
	{
		policy.AllowAnyOrigin()
			  .AllowAnyMethod()
			  .AllowAnyHeader();
	});
});


var app = builder.Build();



app.MapHub<MessagingHub>("/messaginghub");


app.MapControllers();


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("AllowAll");


app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run("http://0.0.0.0:5000");
