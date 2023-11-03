using System.Net;
using System.Net.Mail;
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

        if (appSettings.EmailCredentials is null ||
            string.IsNullOrWhiteSpace(appSettings.EmailCredentials.EmailAddress) ||
            string.IsNullOrWhiteSpace(appSettings.EmailCredentials.Key) ||
            string.IsNullOrWhiteSpace(appSettings.EmailCredentials.Host) ||
            appSettings.EmailCredentials.Port == default)
        {
            Console.WriteLine("Invalid email credentials supplied. Exiting.");

            return;
        }

        var timespan = TimeSpan.FromMinutes(appSettings.FrequencyInMinutes);
        var timer = new PeriodicTimer(timespan);

        var serviceProvider =
            new ServiceCollection()
                .AddLogging(configure =>
                {
                    configure.AddConsole();
                })
                .AddSingleton<ArrestScraperService>()
                .AddSingleton<EmailHandler>()
                .AddSingleton<HttpClient>()
                .AddSingleton(SmtpClientFactory)
                .AddSingleton(timer)
                .AddSingleton(appSettings)
                .BuildServiceProvider();

        var arrestScraperService = serviceProvider.GetService<ArrestScraperService>()!;

        await arrestScraperService.Execute();
    }

    private static Func<IServiceProvider, SmtpClient> SmtpClientFactory = (IServiceProvider serviceProvider) =>
    {
        var emailCredentials =
            serviceProvider
                .GetRequiredService<AppSettings>()
                .EmailCredentials;

        var networkCredential =
            new NetworkCredential(
                emailCredentials.EmailAddress,
                emailCredentials.Key);

        return new()
        {
            Host = emailCredentials.Host,
            Port = emailCredentials.Port,
            EnableSsl = true,
            DeliveryMethod = SmtpDeliveryMethod.Network,
            UseDefaultCredentials = false,
            Credentials = networkCredential,
        };
    };
}
