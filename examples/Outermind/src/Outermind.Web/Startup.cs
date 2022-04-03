using Outermind.Hosting;
using Totem.Hosting;

namespace Outermind;

public class Startup
{
    public void ConfigureServices(IServiceCollection services)
    {
        services.AddMvc();
        services.AddSignalR();
        services.AddOutermind();

        services
        .AddTotemRuntime(MessagesInfo.Assembly, RuntimeInfo.Assembly)
        .AddTotemWeb()
        .AddHttpCommands(pipeline => pipeline.UseTopic())
        .AddHttpQueries(pipeline => pipeline.UseDispatcher())
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
        .AddInMemoryStorage()
        .AddDiskFileStorage();

        services.AddRouting().AddControllers().AddTotemMvc();
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment environment)
    {
        if(environment.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
            app.UseWebAssemblyDebugging();
        }
        else
        {
            app.UseExceptionHandler("/Error");
        }

        app.UseCors();
        app.UseRouting();
        app.UseStaticFiles();
        app.UseBlazorFrameworkFiles();

        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllers();
            endpoints.MapRuntimeHub();
            endpoints.MapFallbackToFile("index.html");
        });
    }
}
