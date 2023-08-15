using System.Net;
using System.Net.Mail;

namespace ArrestScraper.Cli;

public class EmailHandler
{
    private const string SmtpHost = "smtp.gmail.com";
    private const int SmtpPort = 587;

    public EmailHandler(string emailAddress, string key)
    {
        EmailAddress = emailAddress;
        Key = key;
    }

    public string EmailAddress { get; }
    public string Key { get; }

    public void Send(string to, string subject, string body)
    {
        var smtpClient = new SmtpClient
        {
            Host = SmtpHost,
            Port = SmtpPort,
            EnableSsl = true,
            DeliveryMethod = SmtpDeliveryMethod.Network,
            UseDefaultCredentials = false,
            Credentials = new NetworkCredential(EmailAddress, Key)
        };

        using var message = new MailMessage(EmailAddress, to, subject, body);
        smtpClient.Send(message);
    }
}
