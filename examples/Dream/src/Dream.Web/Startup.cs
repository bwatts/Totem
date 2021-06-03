using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Totem.Hosting;

namespace Dream
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc();
            services.AddSignalR();
            services.AddDream();

            services
            .AddRouting()
            .AddControllers()
            .AddCommandControllers()
            .AddQueryControllers()
            .AddJsonStringEnumConverter();

            services
            .AddTotemRuntime()
            .AddTotemWeb()
            .AddFeaturePart(MessagesInfo.Assembly)
            .AddFeaturePart(RuntimeInfo.Assembly)
            .AddHttpCommands(pipeline => pipeline.UseCommandHandler())
            .AddHttpQueries(pipeline => pipeline.UseQueryHandler())
            .AddQueueCommands(pipeline => pipeline.UseQueueHandler())
            .AddEvents(pipeline => pipeline.UseEventHandlers().UseWorkflows().UseReports())
            .AddWorkflows(pipeline => pipeline.UseRouteHandler())
            .AddReports(pipeline => pipeline.UseRouteHandler())
            .AddHttpCommandHandlersAsServices()
            .AddHttpQueryHandlersAsServices()
            .AddQueueHandlersAsServices()
            .AddEventHandlersAsServices()
            .AddWorkflowsAsServices()
            .AddReportsAsServices()
            .AddInMemoryTimelineStore()
            .AddInMemoryRouteStore()
            .AddInMemoryQueueClient()
            .AddInMemoryStorage()
            .AddDiskFileStorage();
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
}