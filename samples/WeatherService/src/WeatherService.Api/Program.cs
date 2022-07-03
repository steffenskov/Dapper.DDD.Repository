using WeatherService.Domain;
using WeatherService.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddMediatR(typeof(MediatRHookup));
builder.Services.AddInfrastructure("Server=127.0.0.1;Database=WeatherService;User Id=sa;Password=SqlServerPassword#&%¤2019;Encrypt=False;"); // Keep your connectionstring somewhere safe in an actual project, this is just for simplicity

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
	_ = app.UseSwagger();
	_ = app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
