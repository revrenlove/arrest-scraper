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

// using System.Net;
// using System.Net.Mail;

// // Email message info
// var toEmailAddress = "Recipient's email address";
// var subjectLine = "Test Subject Line 1002";
// var emailBody = "Test Email Body 1002";

// // Your Email Credentials
// var emailAddress = "Your Email Address";
// var emailPassword = "Your password or app key";

// var smtpHost = "smtp.gmail.com";
// var smtpPort = 587;

// var smtpClient = new SmtpClient
// {
//     Host = smtpHost,
//     Port = smtpPort,
//     EnableSsl = true,
//     DeliveryMethod = SmtpDeliveryMethod.Network,
//     UseDefaultCredentials = false,
//     Credentials = new NetworkCredential(emailAddress, emailPassword)
// };

// using var message = new MailMessage(emailAddress, toEmailAddress, subjectLine, emailBody);
// smtpClient.Send(message);