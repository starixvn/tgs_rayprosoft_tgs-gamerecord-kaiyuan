using Tgs.GameCommand.Common;
using Tgs.GameProviders.Domains.SettingConfiguration;

namespace Tgs.GameRecord.KaiyuanService.Core;

public interface IUtensil
{
    ValueTask<Guid?> GetBrandGameProviderConfigurationIdAsync(
        Guid brandId,
        Guid gameProviderId,
        CancellationToken cancellationToken = default);

    ValueTask<int?> GetGameIdAsync(
        Guid gameProviderId,
        string gameExternalId,
        CancellationToken cancellationToken = default);

    ValueTask<Guid> GetGameProviderIdAsync(
        CancellationToken cancellationToken = default);

    ValueTask<PlayerInfo?> GetPlayerAsync(
        string playerCode,
        CancellationToken cancellationToken = default);

    ValueTask<KYConfiguration> GetSettingConfigurationAsync(
        CancellationToken cancellationToken = default);
}
