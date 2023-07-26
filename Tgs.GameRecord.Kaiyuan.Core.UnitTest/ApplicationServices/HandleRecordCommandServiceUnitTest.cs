using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using Google.Protobuf.WellKnownTypes;
using NSubstitute;
using Tgs.GameCommand.Common;
using Tgs.GameInterface.Kaiyuan.Protos;
using Tgs.GameRecord.Kaiyuan.Core.ApplicationServices;
using Tgs.GameRounds.Work.Client;
using Tgs.GameRounds.Work.Common.Protos;

namespace Tgs.GameRecord.Kaiyuan.Core.UnitTest.ApplicationServices;

public class HandleRecordCommandServiceUnitTest
{
    [Fact]
    public async Task Wins()
    {
        var command = new HandleRecordCommand(
            Record: new GameRecordInfo
            {
                ExternalRoundId = "50-1690343439-360242142-4",
                ExternalGameId = "910",
                PlayerCode = "4ucmqplxb029",
                ExternalCreatedOn = DateTimeOffset.Parse("2023-07-26T11:50:39+08:00").ToTimestamp(),
                ExternalClosedOn = DateTimeOffset.Parse("2023-07-26T11:51:02+08:00").ToTimestamp(),
                Bet = 4.00m,
                ValidBet = 4.00m,
                Settle = 8.00m,
                Commission = 0.00m,
                Turnover = 4.00m,
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

        var fakeUtensil = Substitute.For<IUtensil>();
        var fakeClient = Substitute.For<IGameRoundsWorkClient>();


        var gameProviderId = Guid.NewGuid();
        _ = fakeUtensil
             .GetGameProviderIdAsync(
             cancellationToken: Arg.Any<CancellationToken>())
             .Returns(gameProviderId);

        var gameProviderCode = "KY";
        _ = fakeUtensil
           .GetGameProviderCode()
           .Returns(gameProviderCode);

        var player = new PlayerInfo(
            ResellerId: Guid.NewGuid(),
            ResellerCode: "GPI",
            LicenseeId: Guid.NewGuid(),
            LicenseeCode: "GPI",
            BrandId: Guid.NewGuid(),
            BrandCode: "GPI",
            PlayerCode: "test",
            Username: "test",
            DefaultWalletId: Guid.NewGuid(),
            DefaultWalletCurrencyCode: "CNY");
        _ = fakeUtensil
           .GetPlayerAsync(
           playerCode: Arg.Is(command.Record.PlayerCode),
           cancellationToken: Arg.Any<CancellationToken>())
           .Returns(player);

        var gameId = 2300001;
        _ = fakeUtensil
           .GetGameIdAsync(
           gameProviderId: Arg.Is(gameProviderId),
           gameExternalId: Arg.Is(command.Record.ExternalGameId),
           cancellationToken: Arg.Any<CancellationToken>())
           .Returns(gameId);

        Guid? gameProviderConfigurationId = Guid.NewGuid();
        _ = fakeUtensil
            .GetBrandGameProviderConfigurationIdAsync(
            brandId: Arg.Is(player.BrandId),
            gameProviderId: Arg.Is(gameProviderId),
            cancellationToken: Arg.Any<CancellationToken>())
            .Returns(gameProviderConfigurationId);

        var service = new HandleRecordCommandService(
            fakeUtensil,
            fakeClient);

        await service
            .ExecuteAsync(
            command: command)
            .ConfigureAwait(false);

        _ = fakeClient
            .Received(1)
            .RoundBidding(
            gameProviderId: Arg.Is(gameProviderId),
            gameProviderCode: Arg.Is(gameProviderCode),
            externalRoundId: Arg.Is(command.Record.ExternalRoundId),
            gameId: Arg.Is(gameId),
            externalGameId: Arg.Is(command.Record.ExternalGameId),
            playerCode: Arg.Is(command.Record.PlayerCode),
            walletId: Arg.Is(player!.DefaultWalletId),
            currencyCode: Arg.Is(player!.DefaultWalletCurrencyCode),
            brandId: Arg.Is(player!.BrandId),
            gameProviderConfigurationId: Arg.Is(gameProviderConfigurationId.Value),
            status: Arg.Is(RoundStatus.Closed),
            clientStatus: Arg.Is(RoundClientStatus.Win),
            externalCreatedOn: Arg.Is(command.Record.ExternalCreatedOn.ToDateTimeOffset()),
            externalClosedOn: Arg.Is((DateTimeOffset?)command.Record.ExternalClosedOn.ToDateTimeOffset()),
            bet: Arg.Is(command.Record.Bet.ToDecimal().GetValueOrDefault(0)),
            validBet: Arg.Is(command.Record.ValidBet.ToDecimal().GetValueOrDefault(0)),
            settle: Arg.Is(command.Record.Settle.ToDecimal().GetValueOrDefault(0)),
            commission: Arg.Is(0.0m),
            turnover: Arg.Is(command.Record.ValidBet.ToDecimal().GetValueOrDefault(0)),
            playerWinLoss: Arg.Is(command.Record.WinLoss.ToDecimal().GetValueOrDefault(0)),
            additionalData: Arg.Is(command.Record.AdditionalData),
            cancellationToken: Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Loses()
    {
        var command = new HandleRecordCommand(
            Record: new GameRecordInfo
            {
                ExternalRoundId = "50-1690343466-360242382-3",
                ExternalGameId = "910",
                PlayerCode = "4ucmqplxb029",
                ExternalCreatedOn = DateTimeOffset.Parse("2023-07-26T11:51:06+08:00").ToTimestamp(),
                ExternalClosedOn = DateTimeOffset.Parse("2023-07-26T11:51:29+08:00").ToTimestamp(),
                Bet = 8.00m,
                ValidBet = 0.00m,
                Settle = 7.80m,
                Commission = 0.00m,
                Turnover = 0.00m,
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

        var fakeUtensil = Substitute.For<IUtensil>();
        var fakeClient = Substitute.For<IGameRoundsWorkClient>();


        var gameProviderId = Guid.NewGuid();
        _ = fakeUtensil
             .GetGameProviderIdAsync(
             cancellationToken: Arg.Any<CancellationToken>())
             .Returns(gameProviderId);

        var gameProviderCode = "KY";
        _ = fakeUtensil
           .GetGameProviderCode()
           .Returns(gameProviderCode);

        var player = new PlayerInfo(
            ResellerId: Guid.NewGuid(),
            ResellerCode: "GPI",
            LicenseeId: Guid.NewGuid(),
            LicenseeCode: "GPI",
            BrandId: Guid.NewGuid(),
            BrandCode: "GPI",
            PlayerCode: "test",
            Username: "test",
            DefaultWalletId: Guid.NewGuid(),
            DefaultWalletCurrencyCode: "CNY");
        _ = fakeUtensil
           .GetPlayerAsync(
           playerCode: Arg.Is(command.Record.PlayerCode),
           cancellationToken: Arg.Any<CancellationToken>())
           .Returns(player);

        var gameId = 2300001;
        _ = fakeUtensil
           .GetGameIdAsync(
           gameProviderId: Arg.Is(gameProviderId),
           gameExternalId: Arg.Is(command.Record.ExternalGameId),
           cancellationToken: Arg.Any<CancellationToken>())
           .Returns(gameId);

        Guid? gameProviderConfigurationId = Guid.NewGuid();
        _ = fakeUtensil
            .GetBrandGameProviderConfigurationIdAsync(
            brandId: Arg.Is(player.BrandId),
            gameProviderId: Arg.Is(gameProviderId),
            cancellationToken: Arg.Any<CancellationToken>())
            .Returns(gameProviderConfigurationId);

        var service = new HandleRecordCommandService(
            fakeUtensil,
            fakeClient);

        await service
            .ExecuteAsync(
            command: command)
            .ConfigureAwait(false);

        _ = fakeClient
            .Received(1)
            .RoundBidding(
            gameProviderId: Arg.Is(gameProviderId),
            gameProviderCode: Arg.Is(gameProviderCode),
            externalRoundId: Arg.Is(command.Record.ExternalRoundId),
            gameId: Arg.Is(gameId),
            externalGameId: Arg.Is(command.Record.ExternalGameId),
            playerCode: Arg.Is(command.Record.PlayerCode),
            walletId: Arg.Is(player!.DefaultWalletId),
            currencyCode: Arg.Is(player!.DefaultWalletCurrencyCode),
            brandId: Arg.Is(player!.BrandId),
            gameProviderConfigurationId: Arg.Is(gameProviderConfigurationId.Value),
            status: Arg.Is(RoundStatus.Closed),
            clientStatus: Arg.Is(RoundClientStatus.Loss),
            externalCreatedOn: Arg.Is(command.Record.ExternalCreatedOn.ToDateTimeOffset()),
            externalClosedOn: Arg.Is((DateTimeOffset?)command.Record.ExternalClosedOn.ToDateTimeOffset()),
            bet: Arg.Is(command.Record.Bet.ToDecimal().GetValueOrDefault(0)),
            validBet: Arg.Is(command.Record.ValidBet.ToDecimal().GetValueOrDefault(0)),
            settle: Arg.Is(command.Record.Settle.ToDecimal().GetValueOrDefault(0)),
            commission: Arg.Is(0.0m),
            turnover: Arg.Is(command.Record.ValidBet.ToDecimal().GetValueOrDefault(0)),
            playerWinLoss: Arg.Is(command.Record.WinLoss.ToDecimal().GetValueOrDefault(0)),
            additionalData: Arg.Is(command.Record.AdditionalData),
            cancellationToken: Arg.Any<CancellationToken>());
    }
}
