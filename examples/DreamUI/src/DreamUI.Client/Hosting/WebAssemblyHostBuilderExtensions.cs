using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Serilog;
using Serilog.Events;
using Totem.Hosting;

namespace DreamUI.Hosting
{
    internal static class WebAssemblyHostBuilderExtensions
    {
        internal static WebAssemblyHostBuilder ConfigureSerilog(this WebAssemblyHostBuilder builder) =>
            builder.UseSerilog(logger =>
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
            .AddTotemRuntime()
            .AddFeaturePart(MessagesInfo.Assembly)
            .AddFeaturePart(RuntimeInfo.Assembly)
            .AddLocalCommands(pipeline => pipeline.UseCommandHandler())
            .AddLocalQueries(pipeline => pipeline.UseQueryHandler())
            .AddQueueCommands(pipeline => pipeline.UseQueueHandler())
            .AddEvents(pipeline => pipeline.UseEventHandlers().UseWorkflows().UseReports())
            .AddWorkflows(pipeline => pipeline.UseRouteHandler())
            .AddReports(pipeline => pipeline.UseRouteHandler())
            .AddLocalCommandHandlersAsServices()
            .AddLocalQueryHandlersAsServices()
            .AddQueueHandlersAsServices()
            .AddEventHandlersAsServices()
            .AddWorkflowsAsServices()
            .AddReportsAsServices()
            .AddInMemoryTimelineStore()
            .AddInMemoryRouteStore()
            .AddInMemoryQueueClient()
            .AddInMemoryStorage();

            builder.Services
            .AddTotemClient()
            .AddCommands(pipeline => pipeline.UseRequest())
            .AddQueries(pipeline => pipeline.UseRequest());

            builder.Services.AddDreamUI().AddDreamWebAssembly();

            return builder;
        }

        internal static WebAssemblyHostBuilder ConfigureDreamUI(this WebAssemblyHostBuilder builder)
        {
            builder.UseBaseHttpClient().UseRootComponent<App>("#app");

            builder.Services.AddDreamUI();

            return builder;
        }
    }
}