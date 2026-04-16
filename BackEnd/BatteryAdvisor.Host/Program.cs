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

// Core Services
builder.Services.AddScoped<IHttpClientService, HttpClientService>();
builder.Services.AddSingleton<IWebSocketService, WebSocketService>();
builder.Services.AddSingleton<IHomeAssistantWebSocketResponseService, HomeAssistantWebSocketResponseService>();
builder.Services.AddSingleton<IWebSocketAuthenticationService, WebSocketAuthenticationService>();
builder.Services.AddSingleton<IWebSocketClient, WebSocketClient>();


var app = builder.Build();

app.MapBatteryAdvisorApi();

await app.RunAsync();
