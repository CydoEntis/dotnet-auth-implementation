using AuthImplementation.Infrastructure.Services.Email.Interfaces;
using SendGrid;
using SendGrid.Helpers.Mail;

namespace AuthImplementation.Infrastructure.Services.Email;

public class SendGridService : IEmailService
{
    private readonly IConfiguration _configuration;
    private readonly string _templateFolderPath;

    public SendGridService(IConfiguration configuration)
    {
        _configuration = configuration;
        _templateFolderPath =
            Path.Combine(AppContext.BaseDirectory, "Infrastructure", "Services", "Email", "Templates");
    }

    public async Task SendEmailFromTemplateAsync(
        string toEmail,
        string subject,
        string fromName,
        string templateFileName,
        Dictionary<string, string> templateData
    )
    {
        var templatePath = Path.Combine(_templateFolderPath, templateFileName);

        if (!File.Exists(templatePath))
            throw new FileNotFoundException($"Email template '{templateFileName}' not found.");

        string htmlContent = await File.ReadAllTextAsync(templatePath);

        foreach (var pair in templateData)
        {
            htmlContent = htmlContent.Replace($"[{pair.Key.ToUpper()}]", pair.Value);
        }

        string plainText = "This email requires HTML support.";
        var fromEmail = _configuration["Email:FromEmail"];
        var client = new SendGridClient(_configuration["Email:SendGrid:ApiKey"]);
        var from = new EmailAddress(fromEmail, fromName);
        var to = new EmailAddress(toEmail);
        var msg = MailHelper.CreateSingleEmail(from, to, subject, plainText, htmlContent);

        await client.SendEmailAsync(msg);
    }
}