using System.Net.Mail;
using ArrestScraper.Cli.Config;
using Microsoft.Extensions.Logging;

namespace ArrestScraper.Cli;

public class EmailHandler
{
    private readonly string _emailAddress;
    private readonly ILogger _logger;

    private readonly SmtpClient _smtpClient;

    public EmailHandler(
        SmtpClient smtpClient,
        AppSettings appSettings,
        ILogger<EmailHandler> logger)
    {
        _smtpClient = smtpClient;
        _emailAddress = appSettings.EmailCredentials.EmailAddress;
        _logger = logger;
    }

    public void Send(string to, string subject, string body)
    {
        using var message = new MailMessage(_emailAddress, to, subject, body);

        try
        {
            _smtpClient.Send(message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unable to send email.");
        }
    }
}
