using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Totem.Events;
using Totem.Features;
using Totem.Features.Default;
using Totem.Reports;
using Totem.Routes;
using Totem.Routes.Dispatch;
using Totem.Workflows;

namespace Totem.Hosting
{
    public static class RouteBuilderExtensions
    {
        public static ITotemBuilder AddWorkflows(this ITotemBuilder builder, Action<IWorkflowPipelineBuilder> declarePipeline)
        {
            if(builder == null)
                throw new ArgumentNullException(nameof(builder));

            if(declarePipeline == null)
                throw new ArgumentNullException(nameof(declarePipeline));

            builder.Services
                .AddSingleton<WorkflowsMiddleware>()
                .AddSingleton<WorkflowMiddleware>()
                .AddSingleton<IWorkflowSettings, WorkflowSettings>()
                .AddSingleton<IWorkflowPipelineBuilder, WorkflowPipelineBuilder>()
                .AddSingleton(provider =>
                {
                    var builder = provider.GetRequiredService<IWorkflowPipelineBuilder>();

                    declarePipeline(builder);

                    return builder.Build();
                })
                .AddSingleton<IWorkflowService>(provider =>
                {
                    var dispatcher = new RouteDispatcher(
                        provider.GetRequiredService<IServiceProvider>(),
                        provider.GetRequiredService<ILoggerFactory>(),
                        provider.GetRequiredService<IWorkflowSettings>(),
                        provider.GetRequiredService<IWorkflowPipeline>());

                    return new WorkflowService(
                        provider.GetRequiredService<FeatureRegistry>(),
                        provider.GetRequiredService<IRouteStore>(),
                        dispatcher);
                });

            return builder;
        }

        public static ITotemBuilder AddReports(this ITotemBuilder builder, Action<IReportPipelineBuilder> declarePipeline)
        {
            if(builder == null)
                throw new ArgumentNullException(nameof(builder));

            if(declarePipeline == null)
                throw new ArgumentNullException(nameof(declarePipeline));

            builder.Services
                .AddSingleton<ReportsMiddleware>()
                .AddSingleton<ReportMiddleware>()
                .AddSingleton<IReportSettings, ReportSettings>()
                .AddSingleton<IReportPipelineBuilder, ReportPipelineBuilder>()
                .AddSingleton(provider =>
                {
                    var builder = provider.GetRequiredService<IReportPipelineBuilder>();

                    declarePipeline(builder);

                    return builder.Build();
                })
                .AddSingleton<IReportService>(provider =>
                {
                    var dispatcher = new RouteDispatcher(
                        provider.GetRequiredService<IServiceProvider>(),
                        provider.GetRequiredService<ILoggerFactory>(),
                        provider.GetRequiredService<IReportSettings>(),
                        provider.GetRequiredService<IReportPipeline>());

                    return new ReportService(
                        provider.GetRequiredService<FeatureRegistry>(),
                        provider.GetRequiredService<IRouteStore>(),
                        dispatcher);
                });

            return builder;
        }

        public static ITotemBuilder AddWorkflowsAsServices(this ITotemBuilder builder)
        {
            if(builder == null)
                throw new ArgumentNullException(nameof(builder));

            var feature = builder.Features.Populate<WorkflowFeature>();

            foreach(var workflow in feature.Workflows)
            {
                builder.Services.AddTransient(workflow, workflow);
            }

            return builder;
        }

        public static ITotemBuilder AddReportsAsServices(this ITotemBuilder builder)
        {
            if(builder == null)
                throw new ArgumentNullException(nameof(builder));

            var feature = builder.Features.Populate<ReportFeature>();

            foreach(var report in feature.Reports)
            {
                builder.Services.AddTransient(report, report);
            }

            return builder;
        }
    }
}