using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Totem.Http;
using Totem.Http.Commands;
using Totem.Http.Queries;

namespace Totem.Hosting;

public static class ServiceCollectionExtensions
{
    public const string BaseAddressConfigurationKey = "Totem:HttpClient:BaseAddress";

    public static ITotemClientBuilder AddTotemHttpClient(this IServiceCollection services, Uri? baseAddress = null, Action<JsonSerializerOptions>? configureJson = null)
    {
        if(services is null)
            throw new ArgumentNullException(nameof(services));

        services.TryAddSingleton<IClock, UtcClock>();
        services.AddSingleton<IHttpCommandClient, HttpCommandClient>();
        services.AddSingleton<IHttpQueryClient, HttpQueryClient>();
        services.AddHttpClient<IMessageClient, MessageClient>((provider, client) =>
        {
            if(baseAddress is not null)
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
