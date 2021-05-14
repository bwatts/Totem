using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Totem.Http;
using Totem.Core;

namespace Totem.Hosting
{
    public static class ServiceCollectionExtensions
    {
        public static ITotemClientBuilder AddTotemClient(this IServiceCollection services)
        {
            if(services == null)
                throw new ArgumentNullException(nameof(services));

            services.TryAddSingleton<IClock, SystemClock>();
            services.AddSingleton<ICorrelationIdAccessor, CorrelationIdAccessor>();

            services
            .AddHttpClient()
            .AddSingleton<ITotemClient, TotemClient>()
            .AddHttpClient<ITotemHttpClient, TotemHttpClient>((provider, client) =>
            {
                var configuration = provider.GetRequiredService<IConfiguration>();

                client.BaseAddress = new Uri(configuration["Totem:Client:BaseAddress"]);
            });

            return new TotemClientBuilder(services);
        }
    }
}