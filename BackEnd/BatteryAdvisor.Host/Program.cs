using BatteryAdvisor.Api;
using BatteryAdvisor.Core.ApplicationOptions;
using BatteryAdvisor.Core.Services;
using BatteryAdvisor.HA.Clients;

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

builder.Services.AddScoped<IRestClient, RestClient>();
builder.Services.AddScoped<IHttpClientService, HttpClientService>();

builder.Services.AddSingleton<IWebSocketClient, WebSocketClient>();

var app = builder.Build();


app.MapBatteryAdvisorApi();

await app.RunAsync();
