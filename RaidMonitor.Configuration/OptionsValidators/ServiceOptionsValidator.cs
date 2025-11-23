using Microsoft.Extensions.Options;
using RaidMonitor.Configuration.Options;

namespace RaidMonitor.Configuration.OptionsValidators;

public class ServiceOptionsValidator : IValidateOptions<ServiceOptions>
{
    public ValidateOptionsResult Validate(string? name, ServiceOptions options)
        => options.DelayInSeconds <= 0
            ? ValidateOptionsResult.Fail($"{nameof(options.DelayInSeconds)} must be greater than 0.")
            : ValidateOptionsResult.Success;
}
