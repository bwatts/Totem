using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Totem.Core;
using Totem.Http;

namespace Totem.Hosting
{
    public static class ServiceCollectionExtensions
    {
        public const string BaseAddressConfigurationKey = "Totem:Client:BaseAddress";

        public static ITotemClientBuilder AddTotemClient(this IServiceCollection services, Uri? baseAddress = null, Action<JsonSerializerOptions>? configureJson = null)
        {
            if(services == null)
                throw new ArgumentNullException(nameof(services));

            services.TryAddSingleton<IClock, SystemClock>();
            services.AddSingleton<ITotemClient, TotemClient>();
            services.AddHttpClient<IMessageClient, MessageClient>((provider, client) =>
            {
                if(baseAddress != null)
                {
                    client.BaseAddress = baseAddress;
                    return;
                }

                var configuration = provider.GetRequiredService<IConfiguration>();
                var configuredBaseAddress = configuration[BaseAddressConfigurationKey];

                if(string.IsNullOrWhiteSpace(configuredBaseAddress) || !Uri.TryCreate(configuredBaseAddress, UriKind.Absolute, out var uri))
                    throw new InvalidOperationException($"Expected an absolute URI for configuration key \"{BaseAddressConfigurationKey}\"");

                client.BaseAddress = uri;
            });

            var jsonOptions = new JsonSerializerOptions(JsonSerializerDefaults.Web)
            {
                WriteIndented = true,
                Converters = { new JsonStringEnumConverter() }
            };

            configureJson?.Invoke(jsonOptions);

            services.AddSingleton(jsonOptions);

            return new TotemClientBuilder(services);
        }
    }
}