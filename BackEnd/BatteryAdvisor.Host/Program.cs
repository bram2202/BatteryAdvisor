using BatteryAdvisor.Api;
using BatteryAdvisor.Core.ApplicationOptions;

var builder = WebApplication.CreateBuilder(args);

// Setup configuration
builder.Services.Configure<ApplicationOptions>(
    builder.Configuration.GetSection("ApplicationOptions")
);


// Setup services
builder.Services.AddBatteryAdvisorApi();



var app = builder.Build();


app.MapBatteryAdvisorApi();

app.Run();
