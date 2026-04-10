using BatteryAdvisor.Api;
using BatteryAdvisor.Core.ApplicationOptions;

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



var app = builder.Build();


app.MapBatteryAdvisorApi();

app.Run();
