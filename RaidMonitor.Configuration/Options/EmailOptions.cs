namespace RaidMonitor.Configuration.Options;

public class EmailOptions
{
    public required string BackupEmail { get; set; }

    public required string From { get; set; }

    public required string Subject { get; set; }
}
