using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Options;
using MimeKit;
using MimeKit.Text;
using SmartLib.Option;

namespace SmartLib.Services
{
    public class EmailSender(IOptions<SmtpOption> smtpOptions) : IEmailSender
    {
        private readonly SmtpOption _options = smtpOptions.Value;

        public async Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            using var message = new MimeMessage();
            message.From.Add(new MailboxAddress(_options.SenderName, _options.SenderEmail));
            message.To.Add(MailboxAddress.Parse(email));
            message.Subject = subject;
            message.Body = new TextPart(TextFormat.Html)
            {
                Text = htmlMessage
            };

            using var client = new SmtpClient();

            await client.ConnectAsync(_options.Host, _options.Port, SecureSocketOptions.Auto);

            if (!string.IsNullOrWhiteSpace(_options.UserName) &&
                !string.IsNullOrWhiteSpace(_options.Password))
            {
                await client.AuthenticateAsync(_options.UserName, _options.Password);
            }

            await client.SendAsync(message);
            await client.DisconnectAsync(true);
        }
    }
}
