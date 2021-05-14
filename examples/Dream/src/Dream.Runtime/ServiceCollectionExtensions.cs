using System;
using Dream.Versions;
using Microsoft.Extensions.DependencyInjection;

namespace Dream
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddDream(this IServiceCollection services)
        {
            if(services == null)
                throw new ArgumentNullException(nameof(services));

            return services
                .AddSingleton<IVersionRepository, VersionRepository>()
                .AddSingleton<IVersionService, VersionService>()
                .AddHttpClient();
        }
    }
}