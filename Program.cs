using HopIn_Server.Configurations;
using HopIn_Server.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.Configure<DbSettings>(builder.Configuration.GetSection("MongoDBConfigs"));
//services injected
builder.Services.AddSingleton<VehicleService>();
<<<<<<< HEAD
builder.Services.AddSingleton<UserService>();

=======
builder.Services.AddSingleton<MessagingService>();
builder.Services.AddSingleton<ChatService>();
builder.Services.AddSingleton<InboxService>();
>>>>>>> 4210fb51feebfd3b5d877e6ecd6e7765488326ce

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
