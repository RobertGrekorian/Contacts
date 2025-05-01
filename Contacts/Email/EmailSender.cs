using Contacts.Models;
using Microsoft.Extensions.Options;
using System.Net.Mail;
using System.Net;


public interface IEmailSender
{
    Task SendEmailAsync(string email, string subject, string htmlMessage);
}
    public class EmailSender : IEmailSender
{
    private readonly AuthMessageSenderOptions _options;

    public EmailSender(IOptions<AuthMessageSenderOptions> options)
    {
        _options = options.Value;
    }

    public async Task SendEmailAsync(string email, string subject, string htmlMessage)
    {
        using var client = new SmtpClient(_options.SmtpServer, _options.SmtpPort);
        client.Credentials = new NetworkCredential(_options.SenderEmail, _options.SenderPassword);
        client.EnableSsl = true;

        var mail = new MailMessage
        {
            From = new MailAddress(_options.SenderEmail),
            Subject = subject,
            Body = htmlMessage,
            IsBodyHtml = true
        };
        mail.To.Add(email);

        await client.SendMailAsync(mail);
    }
}