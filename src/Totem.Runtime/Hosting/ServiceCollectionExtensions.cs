using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Totem.Core;
using Totem.Map;

namespace Totem.Hosting;

public static partial class ServiceCollectionExtensions
{
    public static RuntimeMap GetRuntimeMap(this IServiceCollection services)
    {
        if(services is null)
            throw new ArgumentNullException(nameof(services));

        var map = (RuntimeMap?) services
            .LastOrDefault(x => x.ServiceType == typeof(RuntimeMap))
            ?.ImplementationInstance;

        return map ?? throw new InvalidOperationException("Expected runtime map to be initialized; call AddTotemRuntime first");
    }

    public static ITotemBuilder AddTotemRuntime(this IServiceCollection services, IEnumerable<Type> mapTypes)
    {
        if(services is null)
            throw new ArgumentNullException(nameof(services));

        if(mapTypes is null)
            throw new ArgumentNullException(nameof(mapTypes));

        services.AddSingleton<IClock, UtcClock>();

        var map = new RuntimeMap(mapTypes);

        services.AddSingleton(map);

        return new TotemBuilder(services, map);
    }

    public static ITotemBuilder AddTotemRuntime(this IServiceCollection services, IEnumerable<Assembly> assemblies)
    {
        if(services is null)
            throw new ArgumentNullException(nameof(services));

        if(assemblies is null)
            throw new ArgumentNullException(nameof(assemblies));

        return services.AddTotemRuntime(assemblies.SelectMany(x => x.GetExportedTypes()));
    }

    public static ITotemBuilder AddTotemRuntime(this IServiceCollection services, params Type[] types) =>
        services.AddTotemRuntime(types.AsEnumerable());

    public static ITotemBuilder AddTotemRuntime(this IServiceCollection services, params Assembly[] assemblies) =>
        services.AddTotemRuntime(assemblies.AsEnumerable());

    class TotemBuilder : ITotemBuilder
    {
        internal TotemBuilder(IServiceCollection services, RuntimeMap map)
        {
            Services = services;
            Map = map;
        }

        public IServiceCollection Services { get; }
        public RuntimeMap Map { get; }
    }
}
