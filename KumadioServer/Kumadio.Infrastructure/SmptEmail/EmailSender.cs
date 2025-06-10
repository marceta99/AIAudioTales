using Kumadio.Core.Common;
using Kumadio.Core.Common.Interfaces.Base;
using MailKit.Net.Smtp;
using Microsoft.Extensions.Options;
using MimeKit;
using MimeKit.Text;

namespace Kumadio.Infrastructure.SmptEmail
{
    public class EmailSender : IEmailSender
    {
        private readonly SmtpSettings _settings;
        public EmailSender(IOptions<SmtpSettings> options)
        {
            _settings = options.Value;
        }
        public async Task SendEmailAsync(string to, string subject, string htmlBody)
        {
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress("Kumadio", _settings.Username));
            message.To.Add(new MailboxAddress("", to));
            message.Subject = subject;

            message.Body = new TextPart(TextFormat.Html) { Text = htmlBody };

            using (var client = new SmtpClient())
            {
                await client.ConnectAsync(
                    _settings.SmtpHost,
                    _settings.SmtpPort,
                    _settings.EnableSsl ? MailKit.Security.SecureSocketOptions.StartTls : MailKit.Security.SecureSocketOptions.None
                );
                await client.AuthenticateAsync(_settings.Username, _settings.Password);
                await client.SendAsync(message);
                await client.DisconnectAsync(true);
            }
        }
    }
}
