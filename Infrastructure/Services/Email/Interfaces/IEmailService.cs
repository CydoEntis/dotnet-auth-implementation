namespace AuthImplementation.Infrastructure.Services.Email.Interfaces;

public interface IEmailService
{
    Task SendEmailFromTemplateAsync(
        string toEmail,
        string subject,
        string fromName,
        string templateFileName,
        Dictionary<string, string> templateData
    );
}