using Tgs.GameRecordSchedules.Protos;

namespace Tgs.GameRecord.KaiyuanService.Core
{
    public interface IGameRecordSchedule
    {
        ValueTask<GameRecordScheduleReply?> GetAsync(string brandCode, string agentId, string currencyCode, CancellationToken cancellationToken = default);

        ScheduleConfig GetConfig(GameRecordScheduleReply gameProviderSchedule);

        ValueTask<GameRecordScheduleReply?> InitAsync(Guid gameProviderId, string brandCode, string agentId, string currencyCode, CancellationToken cancellationToken = default);

        ValueTask<GameRecordScheduleReply?> UpdateAsync(GameRecordScheduleReply gameProviderSchedule, DateTimeOffset? lastGameRecordTime, DateTimeOffset sysTime, CancellationToken cancellationToken = default);
    }
}
