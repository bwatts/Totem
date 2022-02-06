using Dream.Hosting;
using Totem.Hosting;

namespace Dream;

public class Startup
{
    public void ConfigureServices(IServiceCollection services)
    {
        services.AddMvc();
        services.AddSignalR();
        services.AddDream();

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

        services
        .AddRouting()
        .AddControllers()
        .AddCommandControllers()
        .AddQueryControllers()
        .AddJsonStringEnumConverter();
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
            endpoints.MapFallbackToFile("index.html");
        });
    }
}
