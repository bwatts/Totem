using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Extensions.Hosting;
using Serilog.Extensions.Logging;
using Totem.Subscriptions;

namespace Totem.Hosting;

public static class WebAssemblyHostBuilderExtensions
{
    public static WebAssemblyHostBuilder AddSerilog(this WebAssemblyHostBuilder builder, Action<LoggerConfiguration>? configure = null)
    {
        if(builder is null)
            throw new ArgumentNullException(nameof(builder));

        // Adapted from https://github.com/serilog/serilog-aspnetcore/blob/dev/src/Serilog.AspNetCore/SerilogWebHostBuilderExtensions.cs

        var configuration = new LoggerConfiguration();

        configure?.Invoke(configuration);

        var logger = configuration.CreateLogger();
        builder.Services.AddSingleton(logger);
        builder.Services.AddSingleton<ILoggerFactory>(new SerilogLoggerFactory(logger));

        var diagnosticContext = new DiagnosticContext(logger);
        builder.Services.AddSingleton(diagnosticContext);
        builder.Services.AddSingleton<IDiagnosticContext>(diagnosticContext);

        return builder;
    }

    public static WebAssemblyHostBuilder AddBaseHttpClient(this WebAssemblyHostBuilder builder)
    {
        if(builder is null)
            throw new ArgumentNullException(nameof(builder));

        builder.Services.TryAddScoped(_ => new HttpClient
        {
            BaseAddress = new Uri(builder.HostEnvironment.BaseAddress)
        });

        return builder;
    }

    public static WebAssemblyHostBuilder AddSubscriptionClient(this WebAssemblyHostBuilder builder)
    {
        if(builder is null)
            throw new ArgumentNullException(nameof(builder));

        builder.Services.AddSingleton<ISubscriptionClient>(provider =>
        {
            var baseUrl = new Uri(builder.HostEnvironment.BaseAddress);
            var subscriptionsUrl = new Uri(baseUrl, "_totem/subscriptions");
            var connectionBuilder = new HubConnectionBuilder().WithUrl(subscriptionsUrl, options =>
            {
                options.HttpMessageHandlerFactory = inner => new IncludeCredentialsHandler { InnerHandler = inner };
            });

            return new SubscriptionClient(connectionBuilder.Build());
        });

        return builder;
    }

    public static WebAssemblyHostBuilder AddRootComponent<TComponent>(this WebAssemblyHostBuilder builder, string selector)
        where TComponent : IComponent
    {
        if(builder is null)
            throw new ArgumentNullException(nameof(builder));

        builder.RootComponents.Add<TComponent>(selector);

        return builder;
    }
}
