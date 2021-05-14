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
            services.AddMvc().AddJsonStringEnumConverter();
            services.AddRouting();

            services
            .AddControllers()
            .AddCommandControllers()
            .AddQueryControllers();

            services
            .AddTotemRuntime()
            .AddTotemAspNetCore()
            .AddFeaturePart(MessagesInfo.Assembly)
            .AddFeaturePart(RuntimeInfo.Assembly)
            .AddCommands(pipeline => pipeline.UseCorrelation().UseHttpUser().UseCommandHandler())
            .AddQueries(pipeline => pipeline.UseCorrelation().UseHttpUser().UseQueryHandler())
            .AddQueueCommands(pipeline => pipeline.UseCorrelation().UseQueueHandler())
            .AddEvents(pipeline => pipeline.UseEventHandlers().UseWorkflows().UseReports())
            .AddWorkflows(pipeline => pipeline.UseRouteHandler())
            .AddReports(pipeline => pipeline.UseRouteHandler())
            .AddCommandHandlersAsServices()
            .AddQueryHandlersAsServices()
            .AddQueueHandlersAsServices()
            .AddEventHandlersAsServices()
            .AddWorkflowsAsServices()
            .AddReportsAsServices()
            .AddInProcessTimelineStore()
            .AddInProcessRouteStore()
            .AddInProcessQueueClient()
            .AddInProcessStorage()
            .AddLocalFileStorage();

            services.AddDream();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment environment)
        {
            app.UseStaticFiles();
            app.UseRouting();
            app.UseEndpoints(route => route.MapControllers());

            if(environment.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
        }
    }
}