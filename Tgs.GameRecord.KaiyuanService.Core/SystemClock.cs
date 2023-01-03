namespace Tgs.GameRecord.KaiyuanService.Core;

internal class SystemClock : ISystemClock
{
    public DateTimeOffset CurrentTime => DateTimeOffset.UtcNow;
}
