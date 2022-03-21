using Microsoft.Extensions.DependencyInjection;

namespace Outermind.Hosting;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddOutermind(this IServiceCollection services) =>
        services.AddHttpClient();
}
