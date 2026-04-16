using BatteryAdvisor.Core.Contracts.Services;
using BatteryAdvisor.Core.Services;

namespace BatteryAdvisor.Api;

public static class ApiModuleExtensions
{
    public static IServiceCollection AddBatteryAdvisorApi(this IServiceCollection services)
    {
        services.AddOpenApi();
        services.AddControllers();
        services.AddHttpClient<IHttpClientService, HttpClientService>()
            .ConfigureHttpClient(httpClient =>
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