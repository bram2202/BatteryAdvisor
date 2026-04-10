using BatteryAdvisor.Core.Services;
using BatteryAdvisor.HA.Clients;

namespace BatteryAdvisor.Api;

public static class ApiModuleExtensions
{
    public static IServiceCollection AddBatteryAdvisorApi(this IServiceCollection services)
    {
        services.AddOpenApi();
        services.AddControllers();
        services.AddHttpClient<IHttpClientService, HttpClientService>();
        services.AddScoped<IApiClient, ApiClient>();
        return services;
    }

    public static WebApplication MapBatteryAdvisorApi(this WebApplication app)
    {
        if (app.Environment.IsDevelopment())
        {
            app.MapOpenApi();
        }

        app.UseHttpsRedirection();
        app.MapControllers();

        return app;
    }

 
}