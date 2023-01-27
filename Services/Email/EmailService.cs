using api_authentication_boberto.Models.Config;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;
using MimeKit.Text;

namespace api_authentication_boberto.Services.Email
{
    public class EmailService : IEmailService
    {
        private SmtpConfig smptConfig;
        public EmailService(IOptions<SmtpConfig> smptConfig)
        {
            this.smptConfig = smptConfig.Value;
        }
        public void Send(string to, string subject, string html)
        {
            var email = new MimeMessage();
            email.From.Add(MailboxAddress.Parse(smptConfig.From));
            email.To.Add(MailboxAddress.Parse(to));
            email.Subject = subject;
            email.Body = new TextPart(TextFormat.Html) { Text = html };
            using var smtp = new SmtpClient();
            smtp.Connect(smptConfig.Host, int.Parse(smptConfig.Port), SecureSocketOptions.StartTls);
            smtp.Authenticate(smptConfig.Username, smptConfig.Password);
            smtp.Send(email);
            smtp.Disconnect(true);
        }
    }
}
