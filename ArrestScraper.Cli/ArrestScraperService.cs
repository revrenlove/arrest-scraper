using System.Text.RegularExpressions;
using ArrestScraper.Cli.Config;
using Microsoft.Extensions.Logging;

namespace ArrestScraper.Cli;

public class ArrestScraperService
{
    private readonly PeriodicTimer _timer;
    private readonly AppSettings _appSettings;
    private readonly EmailHandler _emailHandler;
    private readonly HttpClient _httpClient;
    private readonly ILogger _logger;
    private readonly Regex _searchRgx;

    public ArrestScraperService(
        PeriodicTimer timer,
        AppSettings appSettings,
        EmailHandler emailHandler,
        HttpClient httpClient,
        ILogger<ArrestScraperService> logger)
    {
        _timer = timer;
        _appSettings = appSettings;
        _emailHandler = emailHandler;
        _httpClient = httpClient;
        _logger = logger;

        _searchRgx = BuildSearchRegex(_appSettings.SearchTerms);
    }

    public async Task Execute()
    {
        CheckRecords();

        while (await _timer.WaitForNextTickAsync())
        {
            CheckRecords();
        }
    }

    private void CheckRecords()
    {
        _appSettings
            .CountySites
            .ForEach(async countySite => await CheckRecords(countySite));
    }

    private async Task CheckRecords(CountySite countySite)
    {
        var county = countySite.County;
        var url = countySite.Url;

        _logger.LogInformation(
            "{Timestamp} UTC - Checking {County}",
            DateTime.UtcNow.ToLongTimeString(),
            county);

        try
        {
            var response = await _httpClient.GetStringAsync(url);

            var matches = _searchRgx.Matches(response);

            if (matches.Any())
            {
                HandleMatches(matches.Select(m => m.Value), county, url);
            }
            else
            {
                _logger.LogInformation(
                    "{Timestamp} UTC - No records found for {County}",
                    DateTime.UtcNow.ToLongTimeString(),
                    county);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Unable to retrieve records for {County}",
                county);
        }
    }

    private void HandleMatches(IEnumerable<string> matchedTerms, string county, string url)
    {
        var names = string.Join(", ", matchedTerms);

        _logger.LogInformation("{Names} Arrested in {County} County", names, county);

        _emailHandler.Send(_appSettings.RecipientEmail, $"{names} Arrested In {county} County", url);
    }

    private static Regex BuildSearchRegex(List<string> searchTerms)
    {
        var termPatterns = searchTerms.Select(t => $"({t})");

        var searchPattern = string.Join("|", termPatterns);

        var searchRegex = new Regex(searchPattern, RegexOptions.IgnoreCase | RegexOptions.Compiled);

        return searchRegex;
    }
}
