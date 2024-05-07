using System.Text.Json.Nodes;
using Microsoft.Extensions.Options;
using Tgs.GameRecord.Handle.Client;
using Tgs.GameRecord.Handle.Common.Protos;
using Tgs.ServiceUtility;

namespace Tgs.GameRecord.Kaiyuan.Core.ApplicationServices;

internal class HandleRecordCommandService(
	IOptions<GameRecordInfoOptions> options,
	IGameRecordHandleClient client) : ICommandService<HandleRecordCommand>
{
	private readonly GameRecordInfoOptions _gameRecordInfo = options.Value;

	public ValueTask ExecuteAsync(HandleRecordCommand command, CancellationToken cancellationToken = default)
	{
		var betState = (command.Info.WinLoss.ToDecimal().GetValueOrDefault(0) > 0) ? BetState.Win : BetState.Loss;

		return client.RequestPublishAsyncAsync(
			source: new Handle.Client.Models.GameRecordRequest(
				GameProviderCode: _gameRecordInfo.GameProviderCode,
				PlayerCode: command.Info.PlayerCode,
				ExternalRoundId: command.Info.ExternalWagerId,
				ExternalBetId: command.Info.ExternalWagerId,
				ExternalGameId: command.Info.ExternalGameId,
				ExternalCreatedOn: command.Info.ExternalCreatedOn.ToDateTimeOffset(),
				ExternalClosedOn: command.Info.ExternalClosedOn.ToDateTimeOffset(),
				ExternalPayoffOn: command.Info.ExternalClosedOn.ToDateTimeOffset(),
				ExternalResettleOn: DateTimeOffset.UnixEpoch,
				BetState: betState,
				BetType: BetType.Normal,
				FreeGameState: FreeGameState.None,
				GambleState: GambleState.None,
				CashoutState: CashoutState.None,
				ResettleState: ResettleState.None,
				JackpotState: JackpotState.None,
				Bet: command.Info.Bet.ToDecimal().GetValueOrDefault(0),
				ValidBet: command.Info.ValidBet.ToDecimal().GetValueOrDefault(0),
				Settle: command.Info.Settle.ToDecimal().GetValueOrDefault(0),
				Commission: command.Info.Commission.ToDecimal().GetValueOrDefault(0),
				Turnover: command.Info.ValidBet.ToDecimal().GetValueOrDefault(0),
				WinLoss: command.Info.WinLoss.ToDecimal().GetValueOrDefault(0),
				Jackpot: 0.0M),
			additionalData: JsonNode.Parse(command.Info.AdditionalData)!,
			cancellationToken: cancellationToken);
	}
}
