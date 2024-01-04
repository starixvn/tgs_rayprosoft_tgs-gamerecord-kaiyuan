using Microsoft.Extensions.Logging;
using Tgs.GameInterface.Kaiyuan.Client;
using Tgs.GameInterface.Kaiyuan.Protos;
using Tgs.ServiceUtility;

namespace Tgs.GameRecord.Kaiyuan.Core.ApplicationServices;

internal class PullRecordCommandService(
    ILogger<PullRecordCommandService> logger,
    IGameInterfaceKaiyuanClient client,
    ICommandService<HandleRecordCommand> service) : ICommandService<PullRecordCommand>
{
    public async ValueTask ExecuteAsync(PullRecordCommand command, CancellationToken cancellationToken = default)
    {
        var infos = await client
            .GameWagerAsync(
            apiUrl: command.ApiUrl,
            deskey: command.Deskey,
            md5key: command.Md5key,
            agentId: command.AgentId,
            startTime: command.StartTime,
            endTime: command.EndTime,
            cancellationToken: cancellationToken)
            .ToArrayAsync(
            cancellationToken: cancellationToken)
            .ConfigureAwait(false)
            ?? Enumerable.Empty<GameWagerInfo>();

        foreach (var info in infos)
        {
            try
            {
                await service.ExecuteAsync(
                    command: new HandleRecordCommand(info),
                    cancellationToken: cancellationToken)
                    .ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "LogOn:{logOn} Message: An exception occurred in this game record.({info})", DateTime.Now.ToString("o"), info);
            }
        }
    }
}
