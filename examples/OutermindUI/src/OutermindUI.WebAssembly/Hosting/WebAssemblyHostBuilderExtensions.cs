using BlazorPro.BlazorSize;
using Majorsoft.Blazor.Components.Common.JsInterop;
using MatBlazor;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Serilog;
using Serilog.Events;
using Totem.Hosting;

namespace OutermindUI.Hosting;

internal static class WebAssemblyHostBuilderExtensions
{
    internal static WebAssemblyHostBuilder ConfigureSerilog(this WebAssemblyHostBuilder builder) =>
        builder.AddSerilog(logger =>
        {
            logger
            .MinimumLevel.Verbose()
            .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
            .MinimumLevel.Override("Microsoft.Hosting.Lifetime", LogEventLevel.Information)
            .MinimumLevel.Override("System", LogEventLevel.Warning)
            .ReadFrom.Configuration(builder.Configuration)
            .Enrich.FromLogContext()
            .Enrich.WithShortTotemTypes()
            .Destructure.ShortIds()
            .WriteTo.BrowserConsole(LogEventLevel.Verbose);
        });

    internal static WebAssemblyHostBuilder ConfigureTotem(this WebAssemblyHostBuilder builder)
    {
        builder.Services
        .AddTotemRuntime(MessagesInfo.Assembly, RuntimeInfo.Assembly)
        .AddLocalCommands(pipeline => pipeline.UseTopic())
        .AddLocalQueries(pipeline => pipeline.UseDispatcher())
        .AddQueueCommands(pipeline => pipeline.UseTopic())
        .AddEvents(pipeline => pipeline.UseHandlers().UseWorkflows().UseReports())
        .AddTopics(pipeline => pipeline.UseWhen())
        .AddWorkflows(pipeline => pipeline.UseWhenCall())
        .AddReports(pipeline => pipeline.UseWhenCall())
        .AddEventHandlersAsServices()
        .AddQueryHandlersAsServices()
        .AddInMemoryQueueClient()
        .AddInMemoryTopicStore()
        .AddInMemoryWorkflowStore()
        .AddInMemoryReportStore()
        .AddInMemoryStorage();

        builder.Services
        .AddTotemHttpClient()
        .AddCommands(pipeline => pipeline.UseRequest())
        .AddQueries(pipeline => pipeline.UseRequest());

        return builder;
    }

    internal static WebAssemblyHostBuilder ConfigureOutermindUI(this WebAssemblyHostBuilder builder)
    {
        builder.AddBaseHttpClient();
        builder.AddSubscriptionClient();
        builder.AddRootComponent<App>("#app");

        builder.Services.AddMatBlazor();
        builder.Services.AddResizeListener();
        builder.Services.AddJsInteropExtensions();
        builder.Services.AddOutermindUI();

        return builder;
    }
}
