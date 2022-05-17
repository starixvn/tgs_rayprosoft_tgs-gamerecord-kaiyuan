using Tgs.GameProviders.Client;
using Tgs.ServiceUtility;

namespace Tgs.GameRecord.KaiyuanService.Core.ApplicationServices
{
    internal class GameRecordService : IGetService<GameRecordQuery, GameRecordResult>
    {
        private readonly IGameRecordListClient _client;
        private readonly IGameRecordMessageSender _messageSender;
        private readonly IKaiyuanService _service;

        public GameRecordService(
            IGameRecordListClient client,
            IGameRecordMessageSender messageSender,
             IKaiyuanService service)
        {
            _client = client ?? throw new ArgumentNullException(nameof(client));
            _messageSender = messageSender ?? throw new ArgumentNullException(nameof(messageSender));
            _service = service ?? throw new ArgumentNullException(nameof(service));
        }

        public async ValueTask<GameRecordResult> GetAsync(GameRecordQuery query, CancellationToken cancellationToken = default)
        {
            var response = await _client.KYGameRecordListAsync(
                query.BrandCode,
                query.CurrencyCode,
                query.StartTime,
                query.EndTime).ConfigureAwait(false);

            if (response == null || !response.Success)
            {
                return new() { Success = false };
            }

            if (response.Success && response.Datas == null)
            {
                return new() { Success = true };
            }

            var successCount = 0;
            var failCount = 0;
            var lastGameRecordTime = default(DateTimeOffset?);

            foreach (var data in response.Datas!)
            {
                var success = true;
                var gameProviderId = await _service.GetGameProviderIdAsync().ConfigureAwait(false);

                var playerModel = await _service.GetPlayerAsync(data.PlayerCode).ConfigureAwait(false);
                var gameId = await _service.GetGameIdAsync(gameProviderId, data.ExternalGameId).ConfigureAwait(false);
                if (playerModel == null || !gameId.HasValue)
                {
                    success = false;
                }

                Guid? gameProviderConfigurationId = default;
                if (playerModel != null)
                {
                    gameProviderConfigurationId = await _service.BrandGetGameProviderConfigurationIdAsync(playerModel.BrandId, gameProviderId).ConfigureAwait(false);
                    if (!gameProviderConfigurationId.HasValue)
                    {
                        success = false;
                    }
                }

                if (success)
                {
                    successCount++;
                }
                else
                {
                    failCount++;
                }

                await _messageSender.SendGameRoundAsync(new GameRoundMessageModel()
                {
                    GameProviderCode = data.GameProviderCode,
                    GameProviderId = data.GameProviderId,
                    ExternalRoundId = data.ExternalRoundId,
                    PlayerCode = (playerModel == null) ? string.Empty : playerModel.PlayerCode,
                    GameId = gameId ?? -1,
                    ExternalGameId = data.ExternalGameId,
                    CurrencyCode = (playerModel == null) ? string.Empty : playerModel.DefaultWalletCurrencyCode,
                    Status = data.Status,
                    GameProviderConfigurationId = gameProviderConfigurationId ?? Guid.Empty,
                    WalletId = (playerModel == null) ? Guid.Empty : playerModel.DefaultWalletId,
                    BrandId = (playerModel == null) ? Guid.Empty : playerModel.BrandId,
                    ExternalCreatedOn = data.ExternalCreatedOn,
                    ExternalClosedOn = data.ExternalClosedOn,
                    BetAmount = data.BetAmount,
                    ValidBetAmount = data.ValidBetAmount,
                    PlayerWinLoss = data.PlayerWinLoss,
                    Commission = data.Commission,
                    Turnover = data.Turnover,
                    SettleAmount = data.SettleAmount,
                    ClientStatus = data.ClientStatus,
                    AdditionalData = data.AdditionalData,
                    Success = success
                }).ConfigureAwait(false);

                if (success)
                {
                    lastGameRecordTime = data.ExternalClosedOn.HasValue ? data.ExternalClosedOn : data.ExternalCreatedOn;
                }
            };

            return new()
            {
                Success = response.Success,
                LastGameRecorTime = lastGameRecordTime,
                Count = response.Datas.Length,
                SuccessCount = successCount,
                FailCount = failCount
            };
        }
    }
}
