using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Serilog;
using Totem.Hosting;

namespace DreamUI.Hosting
{
    internal static class WebAssemblyHostBuilderExtensions
    {
        internal static WebAssemblyHostBuilder ConfigureSerilog(this WebAssemblyHostBuilder builder) =>
            builder.UseSerilog(logger =>
            {
                logger
                .ReadFrom.Configuration(builder.Configuration)
                .Enrich.FromLogContext()
                .Enrich.WithShortTotemTypes()
                .Destructure.ShortIds()
                .WriteTo.BrowserConsole();

                if(!builder.HostEnvironment.IsDevelopment())
                {
                    logger.MinimumLevel.Warning();
                }
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