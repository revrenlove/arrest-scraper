namespace ArrestScraper.Cli.Config;

public class EmailCredentials
{
    public string EmailAddress { get; set; } = default!;
    public string Key { get; set; } = default!;
    public string Host { get; set; } = default!;
    public int Port { get; set; }
}
