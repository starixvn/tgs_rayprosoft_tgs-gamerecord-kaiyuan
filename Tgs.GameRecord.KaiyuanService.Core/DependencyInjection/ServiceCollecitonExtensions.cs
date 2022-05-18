using System.Reflection;
using Tgs.GameProviders.Client;
using Tgs.GameProviders.QueryServer.Client;
using Tgs.GameRecord.KaiyuanService.Core;
using Tgs.GameRecordSchedules.Client;

namespace Microsoft.Extensions.DependencyInjection;

public static class ServiceCollecitonExtensions
{
    public static IServiceCollection AddKaiyuanServiceCore(
       this IServiceCollection services,
       Action<GameRecordInfoOptions, IServiceProvider> configure,
       Action<GameProvidersQueryServerClientOptions, IServiceProvider> gameProvidersQueryServerClientOptions,
       Action<GameRecordSchedulesClientOptions, IServiceProvider> gameRecordSchedulesClientOptions,
       Action<GameProvidersClientOptions, IServiceProvider> gameProvidersClientOptions)
        => services
        .AddOptions<GameRecordInfoOptions>()
        .Configure(configure)
        .Services
        .RegisterAllServiceUtilityTypes(
            Assembly.GetExecutingAssembly(),
            ServiceLifetime.Transient)
        .Scan(scan =>
        {
            _ = scan.FromCallingAssembly()
            .AddClasses(classes => classes.Where(t => t.Name.EndsWith("repository", StringComparison.OrdinalIgnoreCase)))
            .AsImplementedInterfaces()
            .WithTransientLifetime();
        })
        .AddGameProvidersQueryServerClient(gameProvidersQueryServerClientOptions)
        .AddGameRecordSchedulesClient(gameRecordSchedulesClientOptions)
        .AddGameProvidersClient(gameProvidersClientOptions)
        .AddScoped<ISystemClock, SystemClock>()
        .AddTransient<IKaiyuan, KaiyuanRepository>()
        .Decorate<IKaiyuan>((innerRepository, sp) => ActivatorUtilities.CreateInstance<KaiyuanMemoryCacheRepository>(sp, innerRepository, TimeSpan.FromHours(1)));
}
