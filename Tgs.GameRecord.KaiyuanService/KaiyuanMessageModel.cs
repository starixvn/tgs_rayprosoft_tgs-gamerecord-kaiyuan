namespace Tgs.GameRecord.KaiyuanService
{
    public record KaiyuanMessageModel
    {
        public string GameProviderCode { get; init; } = default!;
        public DateTimeOffset SendDateTime { get; init; }
    }
}
