using System;
using System.Linq;
using System.Reflection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Totem.Commands;
using Totem.Events;
using Totem.Features;
using Totem.Features.Default;
using Totem.Files;
using Totem.InProcess;
using Totem.Queries;
using Totem.Queues;
using Totem.Reports;
using Totem.Routes;
using Totem.Routes.Dispatch;
using Totem.Core;
using Totem.Workflows;

namespace Totem.Hosting
{
    public static class TotemBuilderExtensions
    {
        public static ITotemBuilder ConfigureFeatures(this ITotemBuilder builder, Action<FeatureRegistry> configure)
        {
            if(builder == null)
                throw new ArgumentNullException(nameof(builder));

            if(configure == null)
                throw new ArgumentNullException(nameof(configure));

            configure(builder.Features);

            return builder;
        }

        public static ITotemBuilder AddFeaturePart(this ITotemBuilder builder, FeaturePart part)
        {
            if(builder == null)
                throw new ArgumentNullException(nameof(builder));

            if(part == null)
                throw new ArgumentNullException(nameof(part));

            builder.Features.Parts.Add(part);

            return builder;
        }

        public static ITotemBuilder AddFeaturePart(this ITotemBuilder builder, Assembly assembly) =>
            builder.AddFeaturePart(new AssemblyPart(assembly));

        public static ITotemBuilder AddCommands(this ITotemBuilder builder, Action<ICommandPipelineBuilder> declarePipeline)
        {
            if(builder == null)
                throw new ArgumentNullException(nameof(builder));

            if(declarePipeline == null)
                throw new ArgumentNullException(nameof(declarePipeline));

            builder.Services
                .AddSingleton<CommandHandlerMiddleware>()
                .AddSingleton<CommandCorrelationMiddleware>()
                .AddSingleton<ICommandContextFactory, CommandContextFactory>()
                .AddTransient<ICommandPipelineBuilder, CommandPipelineBuilder>()
                .AddSingleton(provider =>
                 {
                     var pipelineBuilder = provider.GetRequiredService<ICommandPipelineBuilder>();

                     declarePipeline(pipelineBuilder);

                     return pipelineBuilder.Build();
                 });

            return builder;
        }

        public static ITotemBuilder AddQueries(this ITotemBuilder builder, Action<IQueryPipelineBuilder> declarePipeline)
        {
            if(builder == null)
                throw new ArgumentNullException(nameof(builder));

            if(declarePipeline == null)
                throw new ArgumentNullException(nameof(declarePipeline));

            builder.Services
                .AddSingleton<QueryHandlerMiddleware>()
                .AddSingleton<QueryCorrelationMiddleware>()
                .AddSingleton<IQueryContextFactory, QueryContextFactory>()
                .AddTransient<IQueryPipelineBuilder, QueryPipelineBuilder>()
                .AddSingleton(provider =>
                {
                    var pipelineBuilder = provider.GetRequiredService<IQueryPipelineBuilder>();

                    declarePipeline(pipelineBuilder);

                    return pipelineBuilder.Build();
                });

            return builder;
        }

        public static ITotemBuilder AddQueueCommands(this ITotemBuilder builder, Action<IQueuePipelineBuilder> declarePipeline)
        {
            if(builder == null)
                throw new ArgumentNullException(nameof(builder));

            if(declarePipeline == null)
                throw new ArgumentNullException(nameof(declarePipeline));

            builder.Services
                .AddSingleton<QueueHandlerMiddleware>()
                .AddSingleton<QueueCorrelationMiddleware>()
                .AddSingleton<IQueueContextFactory, QueueContextFactory>()
                .AddTransient<IQueuePipelineBuilder, QueuePipelineBuilder>()
                .AddSingleton(provider =>
                {
                    var pipelineBuilder = provider.GetRequiredService<IQueuePipelineBuilder>();

                    declarePipeline(pipelineBuilder);

                    return pipelineBuilder.Build();
                });

            return builder;
        }

        public static ITotemBuilder AddEvents(this ITotemBuilder builder, Action<IEventPipelineBuilder> declarePipeline)
        {
            if(builder == null)
                throw new ArgumentNullException(nameof(builder));

            if(declarePipeline == null)
                throw new ArgumentNullException(nameof(declarePipeline));

            builder.Services
                .AddSingleton<EventHandlerMiddleware>()
                .AddSingleton<IEventContextFactory, EventContextFactory>()
                .AddSingleton<IRouteContextFactory, RouteContextFactory>()
                .AddTransient<IEventPipelineBuilder, EventPipelineBuilder>()
                .AddSingleton(provider =>
                {
                    var pipelineBuilder = provider.GetRequiredService<IEventPipelineBuilder>();

                    declarePipeline(pipelineBuilder);

                    return pipelineBuilder.Build();
                });

            return builder;
        }

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

        public static ITotemBuilder AddCommandHandlersAsServices(this ITotemBuilder builder)
        {
            if(builder == null)
                throw new ArgumentNullException(nameof(builder));

            var feature = builder.Features.Populate<CommandHandlerFeature>();

            foreach(var handler in feature.CommandHandlers)
            {
                var service = handler.ImplementedInterfaces.First(i => i.IsGenericTypeDefinition(typeof(ICommandHandler<>)));

                builder.Services.AddTransient(service, handler);
            }

            return builder;
        }

        public static ITotemBuilder AddQueryHandlersAsServices(this ITotemBuilder builder)
        {
            if(builder == null)
                throw new ArgumentNullException(nameof(builder));

            var feature = builder.Features.Populate<QueryHandlerFeature>();

            foreach(var handler in feature.QueryHandlers)
            {
                var service = handler.ImplementedInterfaces.First(i => i.IsGenericTypeDefinition(typeof(IQueryHandler<>)));

                builder.Services.AddTransient(service, handler);
            }

            return builder;
        }

        public static ITotemBuilder AddQueueHandlersAsServices(this ITotemBuilder builder)
        {
            if(builder == null)
                throw new ArgumentNullException(nameof(builder));

            var feature = builder.Features.Populate<QueueHandlerFeature>();

            foreach(var handler in feature.QueueHandlers)
            {
                var service = handler.ImplementedInterfaces.First(i => i.IsGenericTypeDefinition(typeof(IQueueHandler<>)));

                builder.Services.AddTransient(service, handler);
            }

            return builder;
        }

        public static ITotemBuilder AddEventHandlersAsServices(this ITotemBuilder builder)
        {
            if(builder == null)
                throw new ArgumentNullException(nameof(builder));

            var feature = builder.Features.Populate<EventHandlerFeature>();

            foreach(var handler in feature.EventHandlers)
            {
                var service = handler.ImplementedInterfaces.First(i => i.IsGenericTypeDefinition(typeof(IEventHandler<>)));

                builder.Services.AddTransient(service, handler);
            }

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

        public static ITotemBuilder AddInProcessTimelineStore(this ITotemBuilder builder)
        {
            if(builder == null)
                throw new ArgumentNullException(nameof(builder));

            builder.Services.AddSingleton<ITimelineStore, InProcessTimelineStore>();
            builder.Services.AddSingleton<IInProcessEventSubscription, InProcessEventSubscription>();

            return builder;
        }

        public static ITotemBuilder AddInProcessRouteStore(this ITotemBuilder builder)
        {
            if(builder == null)
                throw new ArgumentNullException(nameof(builder));

            builder.Services.AddSingleton<IRouteStore, InProcessRouteStore>();

            return builder;
        }

        public static ITotemBuilder AddInProcessQueueClient(this ITotemBuilder builder)
        {
            if(builder == null)
                throw new ArgumentNullException(nameof(builder));

            builder.Services.AddSingleton<IQueueClient, InProcessQueueClient>();

            return builder;
        }

        public static ITotemBuilder AddInProcessStorage(this ITotemBuilder builder)
        {
            if(builder == null)
                throw new ArgumentNullException(nameof(builder));

            builder.Services.AddSingleton<IStorage, InProcessStorage>();

            return builder;
        }

        public static ITotemBuilder AddLocalFileStorage(this ITotemBuilder builder)
        {
            if(builder == null)
                throw new ArgumentNullException(nameof(builder));

            builder.Services.AddSingleton<IFileStorage, LocalFileStorage>();
            builder.Services.AddSingleton<ILocalFileStorageSettings>(provider =>
            {
                var configuration = provider.GetRequiredService<IConfiguration>();

                var configValue = configuration["Totem:Files:LocalFileStorage:BaseDirectory"];

                return !string.IsNullOrWhiteSpace(configValue)
                    ? new LocalFileStorageSettings(configValue)
                    : new LocalFileStorageSettings(AppContext.BaseDirectory);
            });

            return builder;
        }
    }
}