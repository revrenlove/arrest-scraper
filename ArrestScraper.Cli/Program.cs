using ArrestScraper.Cli;
using ArrestScraper.Cli.Config;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

internal class Program
{
    private static async Task Main(string[] args)
    {
        AppSettings appSettings =
            new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .AddUserSecrets<Program>()
                .Build()
                .Get<AppSettings>()!;

        var timespan = TimeSpan.FromMinutes(appSettings.FrequencyInMinutes);
        var timer = new PeriodicTimer(timespan);

        var serviceProvider =
            new ServiceCollection()
                .AddLogging(configure =>
                {
                    configure.AddConsole();
                })
                .AddSingleton<ArrestScraperService>()
                .AddSingleton<EmailHandler>(serviceProvider =>
                {
                    return new(appSettings.EmailCredentials.EmailAddress, appSettings.EmailCredentials.Key);
                })
                .AddSingleton<HttpClient>()
                .AddSingleton(timer)
                .AddSingleton(appSettings)
                .BuildServiceProvider();

        var arrestScraperService = serviceProvider.GetService<ArrestScraperService>()!;

        await arrestScraperService.Execute();
    }
}
