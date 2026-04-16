using BatteryAdvisor.Core.Services;
using BatteryAdvisor.HA.Clients;

namespace BatteryAdvisor.Api;

public static class ApiModuleExtensions
{
    public static IServiceCollection AddBatteryAdvisorApi(this IServiceCollection services)
    {
        services.AddOpenApi();
        services.AddControllers();
        services.AddHttpClient<IHttpClientService, HttpClientService>((serviceProvider, httpClient) =>
        {
            httpClient.Timeout = TimeSpan.FromSeconds(30);
        });
        return services;
    }

    public static WebApplication MapBatteryAdvisorApi(this WebApplication app)
    {
        if (app.Environment.IsDevelopment())
        {
            app.MapOpenApi();
        }

        if (!app.Environment.IsDevelopment())
        {
            app.UseHttpsRedirection();
        }

        app.MapControllers();

        return app;
    }


}