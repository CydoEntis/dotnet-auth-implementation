using System.Net;
using System.Net.Mail;
using AuthImplementation.Infrastructure.Services.Email.Interfaces;

namespace AuthImplementation.Infrastructure.Services.Email
{
    public class MailtrapEmailService : IEmailService
    {
        private readonly string _smtpServer;
        private readonly int _smtpPort;
        private readonly string _smtpUsername;
        private readonly string _smtpPassword;
        private readonly string _fromEmail;

        public MailtrapEmailService(IConfiguration configuration)
        {
            _smtpServer = configuration["Email:SmtpServer"];
            _smtpPort = int.Parse(configuration["Email:SmtpPort"]);
            _smtpUsername = configuration["Email:SmtpUsername"];
            _smtpPassword = configuration["Email:SmtpPassword"];
            _fromEmail = configuration["Email:FromEmail"];
        }

        public async Task SendEmailAsync(string toEmail, string subject, string body)
        {
            using var client = new SmtpClient(_smtpServer, _smtpPort)
            {
                Credentials = new NetworkCredential(_smtpUsername, _smtpPassword),
                EnableSsl = true
            };

            using var mailMessage = new MailMessage
            {
                From = new MailAddress(_fromEmail),
                Subject = subject,
                Body = body,
                IsBodyHtml = true
            };

            mailMessage.To.Add(toEmail);

            await client.SendMailAsync(mailMessage);
        }

        public async Task SendEmailFromTemplateAsync(string toEmail, string subject, string fromName,
            string templateFileName, Dictionary<string, string> templateData)
        {
            var templatePath = Path.Combine(Directory.GetCurrentDirectory(), "Infrastructure", "Services", "Email", "Templates", templateFileName);


            if (!File.Exists(templatePath))
            {
                throw new FileNotFoundException($"Template '{templateFileName}' not found.");
            }

            string htmlContent = await File.ReadAllTextAsync(templatePath);

            foreach (var pair in templateData)
            {
                htmlContent = htmlContent.Replace($"[{pair.Key.ToUpper()}]", pair.Value);
            }

            string plainText = "This email requires HTML support.";
            var fromEmail = _fromEmail; // You can use the configured 'from' email here

            using var client = new SmtpClient(_smtpServer, _smtpPort)
            {
                Credentials = new NetworkCredential(_smtpUsername, _smtpPassword),
                EnableSsl = true
            };

            using var mailMessage = new MailMessage
            {
                From = new MailAddress(fromEmail, fromName),
                Subject = subject,
                Body = htmlContent,
                IsBodyHtml = true
            };

            mailMessage.To.Add(toEmail);

            await client.SendMailAsync(mailMessage);
        }
    }
}
