namespace Tgs.GameRecord.Kaiyuan.Core;

public record GameRecordInfoOptions
{
    public string GameProviderCode { get; set; } = default!;
    public string GameProviderConfiguration { get; set; } = default!;
    public string HangfirePrefix { get; set; } = "gamerecord-ky-hangfire:";
    public string HangfireLocking { get; set; } = "gamerecord-ky-locking:";
    public double IntervalMinutes { get; set; } = 5;
    public double IntervalMinutesMax { get; set; } = 30;
    public double IntervalMinutesMinimum { get; set; } = 1;
}
