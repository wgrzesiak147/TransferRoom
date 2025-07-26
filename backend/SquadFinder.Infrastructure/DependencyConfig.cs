using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SquadFinder.Application.Interfaces;

namespace SquadFinder.Infrastructure
{
    public static class DependencyConfig
    {
        public static IServiceCollection AddFootballApiClient(this IServiceCollection services, IConfiguration configuration)
        {
            var apiFootballConfig = configuration.GetSection("ApiFootball");
            var baseUrl = apiFootballConfig.GetValue<string>("BaseUrl")
                          ?? throw new InvalidOperationException("BaseUrl is not configured");

            var apiKey = apiFootballConfig.GetValue<string>("ApiKey")
                          ?? throw new ArgumentNullException("Api key not configured");

            services.AddHttpClient<IFootballApiService, FootballApiService>(client =>
            {
                client.BaseAddress = new Uri(baseUrl);
                client.DefaultRequestHeaders.Add("x-apisports-key", apiKey);
            });

            return services;
        }
    }
}
