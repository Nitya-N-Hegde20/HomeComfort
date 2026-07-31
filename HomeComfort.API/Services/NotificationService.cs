using MailKit.Net.Smtp;
using MimeKit;

namespace HomeComfort.API.Services
{
    public class NotificationService
    {
        private readonly IConfiguration _config;

        public NotificationService(IConfiguration configuration)
        {
            _config = configuration;
        }

        public async Task SendMissingProductAlert(string searchTerm)
        {
            var email = new MimeMessage();
            email.From.Add(MailboxAddress.Parse(_config["Gmail:Address"]));
            email.To.Add(MailboxAddress.Parse(_config["Gmail:Address"])); // sending to yourself
            email.Subject = "HomeComfortHub: Product not found";
            email.Body = new TextPart("plain")
            {
                Text = $"A user searched for \"{searchTerm}\" but no matching product was found. Consider adding it."
            };

            using var smtp = new SmtpClient();
            await smtp.ConnectAsync("smtp.gmail.com", 587, MailKit.Security.SecureSocketOptions.StartTls);
            await smtp.AuthenticateAsync(_config["Gmail:Address"], _config["Gmail:AppPassword"]);
            await smtp.SendAsync(email);
            await smtp.DisconnectAsync(true);
        }
    }
}
