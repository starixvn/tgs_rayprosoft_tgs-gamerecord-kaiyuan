namespace Tgs.GameRecord.KaiyuanService.Core
{
    public interface ISystemClock
    {
        DateTimeOffset CurrentTime { get; }
    }
}
