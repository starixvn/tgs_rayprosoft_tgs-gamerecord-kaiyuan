namespace Tgs.GameRecord.KaiyuanService.Core.JobWorkerServices
{
    public record KaiyuanJobWorkerCommand
    {
        public string GameProviderCode { get; init; } = default!;
        public DateTimeOffset SendOn { get; init; }

        public DateTimeOffset ReceiveOn { get; init; }
    }
}
