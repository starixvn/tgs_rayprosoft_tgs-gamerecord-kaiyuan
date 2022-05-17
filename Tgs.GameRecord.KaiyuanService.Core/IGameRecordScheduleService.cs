using Tgs.GameRecordSchedules.Protos;

namespace Tgs.GameRecord.KaiyuanService.Core
{
    public interface IGameRecordScheduleService
    {
        ValueTask<GameRecordScheduleReply?> GetAsync(string brandCode, string agentId, string currencyCode, CancellationToken cancellationToken = default);

        ScheduleConfig GetConfig(GameRecordScheduleReply gameProviderSchedule);

        ValueTask<GameRecordScheduleReply?> InitAsync(Guid gameProviderId, string brandCode, string agentId, string currencyCode, CancellationToken cancellationToken = default);

        ValueTask<GameRecordScheduleReply?> UpdateAsync(GameRecordScheduleReply gameProviderSchedule, DateTimeOffset? lastGameRecorTime, DateTimeOffset sysTime, CancellationToken cancellationToken = default);
    }
}
