using BatteryAdvisor.Api;

var builder = WebApplication.CreateBuilder(args);

// Setup services
builder.Services.AddBatteryAdvisorApi();


var app = builder.Build();


app.MapBatteryAdvisorApi();

app.Run();
