using Microsoft.Extensions.Options;
using Tgs.GameRecord.Handle.Client;
using Tgs.GameRecord.Handle.Common.Protos;
using Tgs.ServiceUtility;

namespace Tgs.GameRecord.Kaiyuan.Core.ApplicationServices;

internal class HandleRecordCommandService : ICommandService<HandleRecordCommand>
{
    private readonly IGameRecordHandleClient _client;
    private readonly GameRecordInfoOptions _gameRecordInfo;

    public HandleRecordCommandService(
        IOptions<GameRecordInfoOptions> options,
        IGameRecordHandleClient client)
    {
        _gameRecordInfo = options.Value;
        _client = client ?? throw new ArgumentNullException(nameof(client));
    }

    public ValueTask ExecuteAsync(HandleRecordCommand command, CancellationToken cancellationToken = default)
    {
        var wagerState = (command.Record.WinLoss.ToDecimal().GetValueOrDefault(0) > 0) ? WagerStates.Win : WagerStates.Lose;

        return _client
        .MessageAsync(
            gameProviderCode: _gameRecordInfo.GameProviderCode,
            playerCode: command.Record.PlayerCode,
            wagerState: wagerState,
            wagerType: WagerTypes.Normal,
            freeGameState: FreeGameStates.None,
            gambleState: GambleStates.None,
            cashoutState: CashoutStates.None,
            resettleState: ResettleStates.None,
            externalRoundId: command.Record.ExternalRoundId,
            externalWagerId: command.Record.ExternalRoundId,
            externalGameId: command.Record.ExternalGameId,
            externalCreatedOn: command.Record.ExternalCreatedOn.ToDateTimeOffset(),
            externalClosedOn: command.Record.ExternalClosedOn.ToDateTimeOffset(),
            externalPayoffOn: command.Record.ExternalClosedOn.ToDateTimeOffset(),
            externalResettleOn: DateTimeOffset.UnixEpoch,
            bet: command.Record.Bet.ToDecimal().GetValueOrDefault(0),
            validBet: command.Record.ValidBet.ToDecimal().GetValueOrDefault(0),
            settle: command.Record.Settle.ToDecimal().GetValueOrDefault(0),
            commission: command.Record.Commission.ToDecimal().GetValueOrDefault(0),
            turnover: command.Record.Turnover.ToDecimal().GetValueOrDefault(0),
            winLoss: command.Record.WinLoss.ToDecimal().GetValueOrDefault(0),
            additionalData: command.Record.AdditionalData,
            cancellationToken: cancellationToken);
    }
}
