using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Options;
using RaidMonitor.Configuration;
using RaidMonitor.Application.Abstractions.Services;
using RaidMonitor.Configuration.Options;
using Resend;

namespace RaidMonitor.Email;

public class EmailSender(IOptions<EmailOptions> options, IResend resend) : IEmailSender, IEmailService
{
    public async Task SendEmailAsync(string toEmail, string subject, string message)
    {
        var email = new EmailMessage
        {
            From = options.Value.From,
            To = [ toEmail ],
            Subject = subject,
            HtmlBody = message
        };

        await resend.EmailSendAsync(email);
    }
}
