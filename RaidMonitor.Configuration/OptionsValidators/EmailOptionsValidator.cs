using Microsoft.Extensions.Options;
using RaidMonitor.Configuration.Options;

namespace RaidMonitor.Configuration.OptionsValidators;

public class EmailOptionsValidator : IValidateOptions<EmailOptions>
{
    public ValidateOptionsResult Validate(string? name, EmailOptions options)
        => string.IsNullOrWhiteSpace(options.From)
            ? ValidateOptionsResult.Fail($"{nameof(options.From)} is required.")
            : ValidateOptionsResult.Success;
}
