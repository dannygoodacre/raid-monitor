using Microsoft.Extensions.Options;
using RaidMonitor.Configuration.Options;

namespace RaidMonitor.Configuration.OptionsValidators;

public class EmailOptionsValidator : IValidateOptions<EmailOptions>
{
    public ValidateOptionsResult Validate(string? name, EmailOptions options)
    {
        if (string.IsNullOrWhiteSpace(options.BackupEmail))
        {
            return ValidateOptionsResult.Fail($"{nameof(options.BackupEmail)} is required.");
        }

        if (string.IsNullOrWhiteSpace(options.From))
        {
            return ValidateOptionsResult.Fail($"{nameof(options.From)} is required.");
        }

        if (string.IsNullOrWhiteSpace(options.Subject))
        {
            return ValidateOptionsResult.Fail($"{nameof(options.Subject)} is required.");
        }

        return ValidateOptionsResult.Success;
    }
}
