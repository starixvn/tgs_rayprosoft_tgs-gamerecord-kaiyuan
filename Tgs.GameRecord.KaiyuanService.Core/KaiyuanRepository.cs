using Microsoft.Extensions.Options;
using Tgs.GameProviders.Domains.SettingConfiguration;
using Tgs.GameProviders.QueryServer.Client;
using Tgs.GameProviders.QueryServer.Models;

namespace Tgs.GameRecord.KaiyuanService.Core
{
    internal class KaiyuanRepository : IKaiyuan
    {
        private readonly IGameProvidersQueryServerClient _client;
        private readonly GameRecordInfoOptions _gameRecordInfo;

        public KaiyuanRepository(
            IGameProvidersQueryServerClient client,
            IOptions<GameRecordInfoOptions> options)
        {
            _client = client ?? throw new ArgumentNullException(nameof(client));
            _gameRecordInfo = options.Value;
        }

        public ValueTask<Guid?> BrandGetGameProviderConfigurationIdAsync(Guid brandId, Guid gameProviderId)
            => _client.BrandGetGameProviderConfigurationIdAsync(brandId, gameProviderId);

        public async ValueTask<int?> GetGameIdAsync(Guid gameProviderId, string gameExternalId)
        {
            var gameModel = await _client.GameGetAsync(gameProviderId, gameExternalId).ConfigureAwait(false);
            return gameModel?.GameId;
        }

        public async ValueTask<Guid> GetGameProviderIdAsync()
        {
            var gameProviderId = await _client.GameProviderGetGameProviderIdAsync(_gameRecordInfo.GameProviderCode).ConfigureAwait(false);
            return gameProviderId!.Value;
        }

        public ValueTask<PlayerModel?> GetPlayerAsync(string playerCode)
            => _client.PlayerGetAsync(playerCode);

        public ValueTask<KYConfiguration> GetSettingConfigurationAsync()
            => _client.SettingGetConfigurationAsync<KYConfiguration>(_gameRecordInfo.GameProviderConfiguration)!;
    }
}
