using Microsoft.OpenApi.Models;
using Microsoft.EntityFrameworkCore;
using System.Reflection;
using Crudtoso_api.Data;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Extensions.Configuration;
using StackExchange.Redis;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Version = "v1",
        Title = "Crudtoso API",
        Description = "An ASP.NET Core Web API for managing the Crudtoso inventory database",
        Contact = new OpenApiContact
        {

            Name = "Said el Kacimi & Sybren Kooistra",
            Url = new Uri("https://github.com/SybrenK")

        },
    });
    // Set the comments path for the Swagger JSON and UI.
    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    options.IncludeXmlComments(xmlPath);
});

//Connecting with Azure SQL server
var connection = String.Empty;
if (builder.Environment.IsDevelopment())
{
    builder.Configuration.AddEnvironmentVariables().AddJsonFile("appsettings.Development.json");
    connection = builder.Configuration.GetConnectionString("AZURE_SQL_CONNECTIONSTRING_SAMPLE");
}
else
{
    connection = Environment.GetEnvironmentVariable("AZURE_SQL_CONNECTIONSTRING");
}
builder.Services.AddDbContext<BikesDbContext>(options =>
    options.UseSqlServer(connection));
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Configure Azure Cache 4 Redis
string _redisConnectionString = Environment.GetEnvironmentVariable("REDIS_CONNECTIONSTRING");
void ConfigureServices(IServiceCollection services)
{
    // Add Redis cache to the dependency injection container
    services.AddSingleton<IConnectionMultiplexer>(_ =>
    {
        // Create Redis cache connection
        return ConnectionMultiplexer.Connect(_redisConnectionString);
    });

}
    app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
