namespace RaidMonitor.Application.Abstractions.Services;

public interface IEmailService
{
    public Task SendEmailAsync(string toEmail, string subject, string message);
}
