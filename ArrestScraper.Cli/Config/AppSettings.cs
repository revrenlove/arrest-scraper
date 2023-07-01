namespace ArrestScraper.Cli.Config;

public class AppSettings
{
    public List<CountySite> CountySites { get; set; } = new();

    public int FrequencyInMinutes { get; set; }

    public EmailCredentials EmailCredentials { get; set; } = new();

    public string RecipientEmail { get; set; } = default!;

    public List<string> SearchTerms { get; set; } = new();
}
