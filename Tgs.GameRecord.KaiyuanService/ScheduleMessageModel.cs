namespace Tgs.GameRecord.KaiyuanService;

public record ScheduleMessageModel
{
    public string GameProviderCode { get; init; } = default!;
    public DateTimeOffset SendDateTime { get; init; }
}
