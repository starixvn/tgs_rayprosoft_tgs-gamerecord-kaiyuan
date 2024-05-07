using System.Text.Json.Nodes;
using Google.Protobuf.WellKnownTypes;
using Microsoft.Extensions.Options;
using NSubstitute;
using Tgs.GameInterface.Kaiyuan.Protos;
using Tgs.GameRecord.Handle.Client;
using Tgs.GameRecord.Handle.Common.Protos;
using Tgs.GameRecord.Kaiyuan.Core.ApplicationServices;

namespace Tgs.GameRecord.Kaiyuan.Core.UnitTest.ApplicationServices;

public class HandleRecordCommandServiceUnitTest
{
	[Fact]
	public async Task Loss()
	{
		var gameRecordInfo = Options.Create(new GameRecordInfoOptions
		{
			GameProviderCode = "KY",
			GameProviderConfiguration = "KYConfiguration",
			HangfirePrefix = "gamerecord-ky-hangfire:",
			HangfireLocking = "gamerecord-ky-locking:",
			IntervalMinutes = 5,
			IntervalMinutesMax = 30,
			IntervalMinutesMinimum = 1
		});
		var fakeClient = Substitute.For<IGameRecordHandleClient>();

		var command = new HandleRecordCommand(
			Info: new GameWagerInfo
			{
				ExternalWagerId = "50-1690343466-360242382-3",
				ExternalGameId = "910",
				PlayerCode = "4ucmqplxb029",
				ExternalCreatedOn = DateTimeOffset.Parse("2023-07-26T11:51:06+08:00").ToTimestamp(),
				ExternalClosedOn = DateTimeOffset.Parse("2023-07-26T11:51:29+08:00").ToTimestamp(),
				Bet = 8.00m,
				ValidBet = 0.00m,
				Settle = 7.80m,
				Commission = 0.00m,
				WinLoss = -0.20m,
				AdditionalData = new JsonObject
				{
					["GameID"] = "50-1690343466-360242382-3",
					["Accounts"] = "72412_4ucmqplxb029",
					["ServerID"] = 9101,
					["KindID"] = 910,
					["TableID"] = 182020001,
					["ChairID"] = 0,
					["UserCount"] = 1,
					["CellScore"] = 0.00m,
					["AllBet"] = 8.00m,
					["Profit"] = -0.20m,
					["Revenue"] = 0,
					["GameStartTime"] = "2023-07-26 11:51:06",
					["GameEndTime"] = "2023-07-26 11:51:29",
					["CardValue"] = "1b3c263b07002",
					["ChannelID"] = 72412,
					["LineCode"] = "72412_GPI"
				}.ToJsonString()
			});

		var service = new HandleRecordCommandService(
			gameRecordInfo,
			fakeClient);

		await service.ExecuteAsync(command: command);

		_ = fakeClient
			.Received(1)
			.RequestPublishAsyncAsync(
			source: Arg.Is(new Handle.Client.Models.GameRecordRequest(
				GameProviderCode: gameRecordInfo.Value.GameProviderCode,
				PlayerCode: command.Info.PlayerCode,
				ExternalRoundId: command.Info.ExternalWagerId,
				ExternalBetId: command.Info.ExternalWagerId,
				ExternalGameId: command.Info.ExternalGameId,
				ExternalCreatedOn: command.Info.ExternalCreatedOn.ToDateTimeOffset(),
				ExternalClosedOn: command.Info.ExternalClosedOn.ToDateTimeOffset(),
				ExternalPayoffOn: command.Info.ExternalClosedOn.ToDateTimeOffset(),
				ExternalResettleOn: DateTimeOffset.UnixEpoch,
				BetState: BetState.Loss,
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
				Jackpot: 0.0M)),
			additionalData: Arg.Any<JsonNode>(),
			cancellationToken: Arg.Any<CancellationToken>());
	}

	[Fact]
	public async Task Win()
	{
		var gameRecordInfo = Options.Create(new GameRecordInfoOptions
		{
			GameProviderCode = "KY",
			GameProviderConfiguration = "KYConfiguration",
			HangfirePrefix = "gamerecord-ky-hangfire:",
			HangfireLocking = "gamerecord-ky-locking:",
			IntervalMinutes = 5,
			IntervalMinutesMax = 30,
			IntervalMinutesMinimum = 1
		});
		var fakeClient = Substitute.For<IGameRecordHandleClient>();

		var command = new HandleRecordCommand(
			Info: new GameWagerInfo
			{
				ExternalWagerId = "50-1690343439-360242142-4",
				ExternalGameId = "910",
				PlayerCode = "4ucmqplxb029",
				ExternalCreatedOn = DateTimeOffset.Parse("2023-07-26T11:50:39+08:00").ToTimestamp(),
				ExternalClosedOn = DateTimeOffset.Parse("2023-07-26T11:51:02+08:00").ToTimestamp(),
				Bet = 4.00m,
				ValidBet = 4.00m,
				Settle = 8.00m,
				Commission = 0.00m,
				WinLoss = 4.00m,
				AdditionalData = new JsonObject
				{
					["GameID"] = "50-1690343439-360242142-4",
					["Accounts"] = "72412_4ucmqplxb029",
					["ServerID"] = 9101,
					["KindID"] = 910,
					["TableID"] = 182020001,
					["ChairID"] = 0,
					["UserCount"] = 1,
					["CellScore"] = 4.00m,
					["AllBet"] = 4.00m,
					["Profit"] = 4.00m,
					["Revenue"] = 0,
					["GameStartTime"] = "2023-07-26 11:50:39",
					["GameEndTime"] = "2023-07-26 11:51:02",
					["CardValue"] = "330b251a17001",
					["ChannelID"] = 72412,
					["LineCode"] = "72412_GPI"
				}.ToJsonString()
			});

		var service = new HandleRecordCommandService(
			gameRecordInfo,
			fakeClient);

		await service.ExecuteAsync(command: command);

		_ = fakeClient
			.Received(1)
			.RequestPublishAsyncAsync(
			source: Arg.Is(new Handle.Client.Models.GameRecordRequest(
				GameProviderCode: gameRecordInfo.Value.GameProviderCode,
				PlayerCode: command.Info.PlayerCode,
				ExternalRoundId: command.Info.ExternalWagerId,
				ExternalBetId: command.Info.ExternalWagerId,
				ExternalGameId: command.Info.ExternalGameId,
				ExternalCreatedOn: command.Info.ExternalCreatedOn.ToDateTimeOffset(),
				ExternalClosedOn: command.Info.ExternalClosedOn.ToDateTimeOffset(),
				ExternalPayoffOn: command.Info.ExternalClosedOn.ToDateTimeOffset(),
				ExternalResettleOn: DateTimeOffset.UnixEpoch,
				BetState: BetState.Win,
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
				Jackpot: 0.0M)),
			additionalData: Arg.Any<JsonNode>(),
			cancellationToken: Arg.Any<CancellationToken>());
	}
}
