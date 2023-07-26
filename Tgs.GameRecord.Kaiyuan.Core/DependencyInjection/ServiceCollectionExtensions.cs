using System.Reflection;
using Tgs.GameCommand.Client;
using Tgs.GameInterface.Kaiyuan.Client;
using Tgs.GameRecord.Kaiyuan.Core;

namespace Microsoft.Extensions.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddGameRecordCore(
       this IServiceCollection services,
       Action<GameRecordInfoOptions, IServiceProvider> gameRecordInfoOptions,
       Action<GameCommandClientOptions, IServiceProvider> gameCommandClientOptions,
       Action<GameInterfaceKaiyuanClientOptions, IServiceProvider> gameInterfaceKaiyuanClientOptions)
        => services
        .AddOptions<GameRecordInfoOptions>()
        .Configure(gameRecordInfoOptions)
        .Services
        .RegisterAllServiceUtilityTypes(
            Assembly.GetExecutingAssembly(),
            ServiceLifetime.Transient)
        .AddGameRoundsWorkClient()
        .AddGameCommandClient(gameCommandClientOptions)
        .AddGameInterfaceKaiyuanClient(gameInterfaceKaiyuanClientOptions)
        .AddTransient<IUtensil>(sp => ActivatorUtilities.CreateInstance<UtensilRepository>(sp, TimeSpan.FromHours(1)))
        .AddTransient<IHangfireUtensil, HangfireUtensil>();
}
