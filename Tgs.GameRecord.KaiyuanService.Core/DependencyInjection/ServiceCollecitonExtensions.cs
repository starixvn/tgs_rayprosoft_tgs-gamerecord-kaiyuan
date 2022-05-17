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
        .Scan(scan =>
        {
            _ = scan.FromCallingAssembly()
            .AddClasses(classes => classes.Where(t => t.Name.EndsWith("service", StringComparison.OrdinalIgnoreCase)))
            .AsImplementedInterfaces()
            .WithTransientLifetime();
        })
        .RegisterAllServiceUtilityTypes(
            Assembly.GetExecutingAssembly(),
            ServiceLifetime.Transient)
        .AddGameProvidersQueryServerClient(gameProvidersQueryServerClientOptions)
        .AddGameRecordSchedulesClient(gameRecordSchedulesClientOptions)
        .AddGameProvidersClient(gameProvidersClientOptions)
        .AddScoped<ISystemClock, SystemClock>()
        .AddScoped<IGameRecordMessageSender, GameRecordMessageSender>();
}
