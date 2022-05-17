using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using Tgs.GameProviders.Domains.SettingConfiguration;
using Tgs.GameProviders.QueryServer.Client;
using Tgs.GameProviders.QueryServer.Models;

namespace Tgs.GameRecord.KaiyuanService.Core
{
    internal class KaiyuanService : IKaiyuanService
    {
        private readonly IGameProvidersQueryServerClient _client;
        private readonly GameRecordInfoOptions _gameRecordInfo;
        private readonly IMemoryCache _memoryCache;

        public KaiyuanService(
            IOptions<GameRecordInfoOptions> options,
            IGameProvidersQueryServerClient client,
            IMemoryCache memoryCache
            )
        {
            _gameRecordInfo = options.Value;
            _client = client ?? throw new ArgumentNullException(nameof(client));
            _memoryCache = memoryCache ?? throw new ArgumentNullException(nameof(memoryCache));
        }

        public async ValueTask<Guid?> BrandGetGameProviderConfigurationIdAsync(Guid brandId, Guid gameProviderId)
        {
            var key = $"{_gameRecordInfo.GameProviderCode}:{brandId}:{gameProviderId}";
            if (!_memoryCache.TryGetValue(key, out Guid? gameProviderConfigurationId))
            {
                gameProviderConfigurationId = await _client.BrandGetGameProviderConfigurationIdAsync(brandId, gameProviderId).ConfigureAwait(false);

                if (gameProviderConfigurationId == null)
                {
                    return null;
                }
                _ = _memoryCache.Set(key, gameProviderConfigurationId, TimeSpan.FromDays(1));
            }
            return gameProviderConfigurationId;
        }

        public async ValueTask<int?> GetGameIdAsync(Guid gameProviderId, string gameExternalId)
        {
            var key = $"{_gameRecordInfo.GameProviderCode}:{gameExternalId}";
            if (!_memoryCache.TryGetValue(key, out int? gameId))
            {
                var gameModel = await _client.GameGetAsync(gameProviderId, gameExternalId).ConfigureAwait(false);

                if (gameModel == null)
                {
                    return null;
                }
                gameId = gameModel.GameId;
                _ = _memoryCache.Set(key, gameId, TimeSpan.FromDays(1));
            }
            return gameId;
        }

        public async ValueTask<Guid> GetGameProviderIdAsync()
        {
            if (!_memoryCache.TryGetValue(_gameRecordInfo.GameProviderCode, out Guid? gameProviderId))
            {
                gameProviderId = await _client.GameProviderGetGameProviderIdAsync(
                    _gameRecordInfo.GameProviderCode).ConfigureAwait(false);

                if (gameProviderId.HasValue)
                {
                    _ = _memoryCache.Set(_gameRecordInfo.GameProviderCode, gameProviderId, TimeSpan.FromDays(1));
                }
            }
            return gameProviderId!.Value;
        }

        public async ValueTask<PlayerModel?> GetPlayerAsync(string playerCode)
        {
            var key = $"{_gameRecordInfo.GameProviderCode}:{playerCode}";
            if (!_memoryCache.TryGetValue(key, out PlayerModel? playerModel))
            {
                playerModel = await _client.PlayerGetAsync(playerCode).ConfigureAwait(false);

                if (playerModel == null)
                {
                    return null;
                }
                _ = _memoryCache.Set(key, playerModel, TimeSpan.FromDays(1));
            }
            return playerModel;
        }

        public ValueTask<KYConfiguration> GetSettingConfigurationAsync()
            => _client.SettingGetConfigurationAsync<KYConfiguration>(_gameRecordInfo.GameProviderConfiguration)!;
    }
}
