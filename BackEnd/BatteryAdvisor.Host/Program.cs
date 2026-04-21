using BatteryAdvisor.Api;
using BatteryAdvisor.Core.ApplicationOptions;
using BatteryAdvisor.Core.Contracts.Services;
using BatteryAdvisor.Core.Services;
using BatteryAdvisor.HA.Contracts.Clients;
using BatteryAdvisor.HA.Contracts.Helpers;
using BatteryAdvisor.HA.Contracts.Services;
using BatteryAdvisor.HA.Clients;
using BatteryAdvisor.HA.Helpers;
using BatteryAdvisor.HA.Services;
using BatteryAdvisor.Core.Database;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration
    .AddJsonFile("config/appsettings.json", optional: false, reloadOnChange: true)
    .AddJsonFile($"config/appsettings.{builder.Environment.EnvironmentName}.json", optional: true, reloadOnChange: true)
    .AddJsonFile("config/appsettings.Local.json", optional: true, reloadOnChange: true);

// Setup configuration
builder.Services.Configure<ApplicationOptions>(
    builder.Configuration.GetSection("ApplicationOptions")
);


// Setup services
builder.Services.AddBatteryAdvisorApi();

// Scoped ==> new instance per request
// Singleton ==> same instance for entire application lifetime

// HA Services
builder.Services.AddScoped<IRestClient, RestClient>();
builder.Services.AddSingleton<IWebSocketMessageHelper, WebSocketMessageHelper>();
builder.Services.AddScoped<IStatisticsService, StatisticsService>();

// Core Services
builder.Services.AddScoped<IDatabaseService, DatabaseService>();
builder.Services.AddScoped<IConfigurationService, ConfigurationService>();
builder.Services.AddScoped<IHttpClientService, HttpClientService>();
builder.Services.AddSingleton<IWebSocketService, WebSocketService>();
builder.Services.AddSingleton<IHomeAssistantWebSocketResponseService, HomeAssistantWebSocketResponseService>();
builder.Services.AddSingleton<IWebSocketAuthenticationService, WebSocketAuthenticationService>();
builder.Services.AddSingleton<IWebSocketClient, WebSocketClient>();

// Database
builder.Services.AddDbContext<BatteryAdvisorContext>(options =>
{
    var databaseName = builder.Configuration.GetValue<string>("DatabaseName") ?? "BatteryAdvisor.db";
    var dbPath = Path.Combine(builder.Environment.ContentRootPath, databaseName);
    var connectionString = $"Data Source={dbPath}";

    options.UseSqlite(connectionString);
}
);


var app = builder.Build();

app.MapBatteryAdvisorApi();

await app.RunAsync();
