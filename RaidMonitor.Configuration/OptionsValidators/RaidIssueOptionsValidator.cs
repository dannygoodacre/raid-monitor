using Microsoft.Extensions.Options;
using RaidMonitor.Configuration.Options;

namespace RaidMonitor.Configuration.OptionsValidators;

public class RaidIssueOptionsValidator : IValidateOptions<RaidIssueOptions>
{
    public ValidateOptionsResult Validate(string? name, RaidIssueOptions options)
    {
        if (options.Keywords.Count == 0)
        {
            return ValidateOptionsResult.Fail($"{nameof(options.Keywords)} is required.");
        }

        if (options.Keywords.Any(string.IsNullOrWhiteSpace))
        {
            return ValidateOptionsResult.Fail($"{nameof(options.Keywords)} must not be null, empty, or whitespace.");
        }

        return ValidateOptionsResult.Success;
    }
}
