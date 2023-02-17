using System.Text.Json;
using Microsoft.Extensions.Options;
using Tgs.GameProviders.Client;
using Tgs.GameRounds.WorkService.Client;
using Tgs.GameRounds.WorkService.Common.Protos;
using Tgs.ServiceUtility;

namespace Tgs.GameRecord.KaiyuanService.Core.ApplicationServices;

internal class GameRecordService : IGetService<GameRecordQuery, GameRecordResult>
{
    private readonly IGameRecordListClient _client;
    private readonly GameRecordInfoOptions _gameRecordInfo;
    private readonly IGameRoundsWorkServiceClient _gameRoundsWorkServiceClient;
    private readonly IUtensil _utensil;

    public GameRecordService(
        IOptions<GameRecordInfoOptions> gameRecordInfoOptions,
        IGameRecordListClient client,
        IGameRoundsWorkServiceClient gameRoundsWorkServiceClient,
        IUtensil utensil)
    {
        _gameRecordInfo = gameRecordInfoOptions.Value;
        _client = client ?? throw new ArgumentNullException(nameof(client));
        _gameRoundsWorkServiceClient = gameRoundsWorkServiceClient ?? throw new ArgumentNullException(nameof(gameRoundsWorkServiceClient));
        _utensil = utensil ?? throw new ArgumentNullException(nameof(utensil));
    }

    public async ValueTask<GameRecordResult> GetAsync(GameRecordQuery query, CancellationToken cancellationToken = default)
    {
        var response = await _client.KYGameRecordListAsync(
            brandCode: query.BrandCode,
            currencyCode: query.CurrencyCode,
            startTime: query.StartTime,
            endTime: query.EndTime,
            cancellationToken: cancellationToken).ConfigureAwait(false);

        if (response == null || !response.Success)
        {
            return new() { Success = false };
        }

        var datas = response.Datas ?? Enumerable.Empty<GameRecordModle>();

        var successCount = 0;
        var failureCount = 0;
        var lastGameRecordTime = default(DateTimeOffset?);

        foreach (var data in datas)
        {
            var success = true;
            var gameProviderId = await _utensil
                .GetGameProviderIdAsync(
                    cancellationToken: cancellationToken)
                .ConfigureAwait(false);

            var playerModel = await _utensil
                .GetPlayerAsync(
                    playerCode: data.PlayerCode,
                    cancellationToken: cancellationToken)
                .ConfigureAwait(false);
            var gameId = await _utensil
                .GetGameIdAsync(
                    gameProviderId: gameProviderId,
                    gameExternalId: data.ExternalGameId,
                    cancellationToken: cancellationToken)
                .ConfigureAwait(false);
            if (playerModel == null || !gameId.HasValue)
            {
                success = false;
            }

            Guid? gameProviderConfigurationId = default;
            if (playerModel != null)
            {
                gameProviderConfigurationId = await _utensil
                    .GetBrandGameProviderConfigurationIdAsync(
                        brandId: playerModel.BrandId,
                        gameProviderId: gameProviderId,
                        cancellationToken: cancellationToken)
                    .ConfigureAwait(false);
                if (!gameProviderConfigurationId.HasValue)
                {
                    success = false;
                }
            }

            if (success)
            {
                await _gameRoundsWorkServiceClient
                .RoundBidding(
                    gameProviderId: gameProviderId,
                    gameProviderCode: _gameRecordInfo.GameProviderCode,
                    externalRoundId: data.ExternalRoundId,
                    gameId: gameId.Value,
                    externalGameId: data.ExternalGameId,
                    playerCode: playerModel!.PlayerCode,
                    walletId: playerModel!.DefaultWalletId,
                    currencyCode: playerModel!.DefaultWalletCurrencyCode,
                    brandId: playerModel!.BrandId,
                    gameProviderConfigurationId: gameProviderConfigurationId!.Value,
                    status: (RoundBiddingStatus)data.Status,
                    clientStatus: (RoundBiddingClientStatus)data.ClientStatus,
                    externalCreatedOn: data.ExternalCreatedOn!.Value,
                    externalClosedOn: data.ExternalClosedOn,
                    bet: data.BetAmount.GetValueOrDefault(0),
                    validBet: data.ValidBetAmount.GetValueOrDefault(0),
                    settle: data.SettleAmount.GetValueOrDefault(0),
                    commission: data.Commission.GetValueOrDefault(0),
                    turnover: data.Turnover.GetValueOrDefault(0),
                    playerWinLoss: data.PlayerWinLoss.GetValueOrDefault(0),
                    additionalData: data.AdditionalData, cancellationToken: cancellationToken)
                .ConfigureAwait(false);
                successCount++;
            }
            else
            {
                await _gameRoundsWorkServiceClient
                .RoundMissing(
                    gameProviderId: gameProviderId,
                    gameProviderCode: _gameRecordInfo.GameProviderCode,
                    externalRoundId: data.ExternalRoundId,
                    gameId: gameId ?? -1,
                    externalGameId: data.ExternalGameId,
                    playerCode: playerModel?.PlayerCode ?? string.Empty,
                    data: JsonSerializer.Serialize(data),
                    cancellationToken: cancellationToken)
                .ConfigureAwait(false);
                failureCount++;
            }

            lastGameRecordTime = data.ExternalClosedOn.HasValue ? data.ExternalClosedOn : data.ExternalCreatedOn;
        };

        return new()
        {
            Success = response.Success,
            LastGameRecordTime = lastGameRecordTime,
            Count = datas.Count(),
            SuccessCount = successCount,
            FailureCount = failureCount
        };
    }
}
