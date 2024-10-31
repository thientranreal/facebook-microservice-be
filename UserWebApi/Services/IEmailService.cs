namespace UserWebApi.Services;

public interface IEmailService
{
    Task SendEmailAsync(string toEmail, string subject, string body);
    Task SendConfirmationEmailAsync(string toEmail, string userName);
}