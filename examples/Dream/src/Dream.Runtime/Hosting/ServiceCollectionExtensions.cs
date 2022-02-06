using Microsoft.Extensions.DependencyInjection;

namespace Dream.Hosting;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddDream(this IServiceCollection services) =>
        services.AddHttpClient();
}
