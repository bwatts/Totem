using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Totem.Core;
using Totem.Events;
using Totem.Files;
using Totem.Http;
using Totem.InMemory;
using Totem.Local;
using Totem.Queues;
using Totem.Reports;
using Totem.Topics;
using Totem.Workflows;

namespace Totem.Hosting;

public static class TotemBuilderExtensions
{
    public static ITotemBuilder AddTopics(this ITotemBuilder builder, Action<ITopicPipelineBuilder> declarePipeline)
    {
        if(builder is null)
            throw new ArgumentNullException(nameof(builder));

        if(declarePipeline is null)
            throw new ArgumentNullException(nameof(declarePipeline));

        builder.Services
            .AddSingleton<TopicWhenMiddleware>()
            .AddSingleton<ITopicContextFactory, TopicContextFactory>()
            .AddTransient<ITopicPipelineBuilder, TopicPipelineBuilder>()
            .AddSingleton(provider =>
            {
                var pipelineBuilder = provider.GetRequiredService<ITopicPipelineBuilder>();

                declarePipeline(pipelineBuilder);

                return pipelineBuilder.Build();
            });

        foreach(var topic in builder.Services.GetRuntimeMap().Topics)
        {
            builder.Services.AddTransient(topic.DeclaredType);
        }

        return builder;
    }

    public static ITotemBuilder AddWorkflows(this ITotemBuilder builder, Action<IWorkflowPipelineBuilder> declarePipeline)
    {
        if(builder is null)
            throw new ArgumentNullException(nameof(builder));

        if(declarePipeline is null)
            throw new ArgumentNullException(nameof(declarePipeline));

        builder.Services
            .AddSingleton<WorkflowEventMiddleware>()
            .AddSingleton<WorkflowWhenMiddleware>()
            .AddSingleton<IWorkflowContextFactory, WorkflowContextFactory>()
            .AddTransient<IWorkflowPipelineBuilder, WorkflowPipelineBuilder>()
            .AddSingleton(provider =>
            {
                var pipelineBuilder = provider.GetRequiredService<IWorkflowPipelineBuilder>();

                declarePipeline(pipelineBuilder);

                return pipelineBuilder.Build();
            });

        foreach(var workflow in builder.Services.GetRuntimeMap().Workflows)
        {
            builder.Services.AddTransient(workflow.DeclaredType);
        }

        return builder;
    }

    public static ITotemBuilder AddReports(this ITotemBuilder builder, Action<IReportPipelineBuilder> declarePipeline)
    {
        if(builder is null)
            throw new ArgumentNullException(nameof(builder));

        if(declarePipeline is null)
            throw new ArgumentNullException(nameof(declarePipeline));

        builder.Services
            .AddSingleton<ReportEventMiddleware>()
            .AddSingleton<ReportWhenMiddleware>()
            .AddSingleton<IReportContextFactory, ReportContextFactory>()
            .AddTransient<IReportPipelineBuilder, ReportPipelineBuilder>()
            .AddSingleton(provider =>
            {
                var pipelineBuilder = provider.GetRequiredService<IReportPipelineBuilder>();

                declarePipeline(pipelineBuilder);

                return pipelineBuilder.Build();
            });

        foreach(var report in builder.Services.GetRuntimeMap().Reports)
        {
            builder.Services.AddTransient(report.DeclaredType);
        }

        return builder;
    }

    public static ITotemBuilder AddHttpCommands(this ITotemBuilder builder, Action<IHttpCommandPipelineBuilder> declarePipeline)
    {
        if(builder is null)
            throw new ArgumentNullException(nameof(builder));

        if(declarePipeline is null)
            throw new ArgumentNullException(nameof(declarePipeline));

        builder.Services
            .AddSingleton<HttpCommandTopicMiddleware>()
            .AddSingleton<IHttpCommandContextFactory, HttpCommandContextFactory>()
            .AddTransient<IHttpCommandPipelineBuilder, HttpCommandPipelineBuilder>()
            .AddSingleton(provider =>
            {
                var pipelineBuilder = provider.GetRequiredService<IHttpCommandPipelineBuilder>();

                declarePipeline(pipelineBuilder);

                return pipelineBuilder.Build();
            });

        return builder;
    }

    public static ITotemBuilder AddHttpQueries(this ITotemBuilder builder, Action<IHttpQueryPipelineBuilder> declarePipeline)
    {
        if(builder is null)
            throw new ArgumentNullException(nameof(builder));

        if(declarePipeline is null)
            throw new ArgumentNullException(nameof(declarePipeline));

        builder.Services
            .AddSingleton<HttpQueryDispatcherMiddleware>()
            .AddSingleton<IHttpQueryContextFactory, HttpQueryContextFactory>()
            .AddTransient<IHttpQueryPipelineBuilder, HttpQueryPipelineBuilder>()
            .AddSingleton(provider =>
            {
                var pipelineBuilder = provider.GetRequiredService<IHttpQueryPipelineBuilder>();

                declarePipeline(pipelineBuilder);

                return pipelineBuilder.Build();
            });

        builder.Services.TryAddSingleton<IQueryReportClient, QueryReportClient>();

        foreach(var handler in builder.Services.GetRuntimeMap().QueryHandlers)
        {
            foreach(var serviceType in handler.ServiceTypes)
            {
                builder.Services.AddTransient(serviceType, handler.DeclaredType);
            }
        }

        return builder;
    }

    public static ITotemBuilder AddLocalCommands(this ITotemBuilder builder, Action<ILocalCommandPipelineBuilder> declarePipeline)
    {
        if(builder is null)
            throw new ArgumentNullException(nameof(builder));

        if(declarePipeline is null)
            throw new ArgumentNullException(nameof(declarePipeline));

        builder.Services
            .AddSingleton<LocalCommandTopicMiddleware>()
            .AddSingleton<ILocalCommandContextFactory, LocalCommandContextFactory>()
            .AddTransient<ILocalCommandPipelineBuilder, LocalCommandPipelineBuilder>()
            .AddSingleton(provider =>
            {
                var pipelineBuilder = provider.GetRequiredService<ILocalCommandPipelineBuilder>();

                declarePipeline(pipelineBuilder);

                return pipelineBuilder.Build();
            });

        builder.Services.TryAddSingleton<ILocalClient, LocalClient>();

        return builder;
    }

    public static ITotemBuilder AddLocalQueries(this ITotemBuilder builder, Action<ILocalQueryPipelineBuilder> declarePipeline)
    {
        if(builder is null)
            throw new ArgumentNullException(nameof(builder));

        if(declarePipeline is null)
            throw new ArgumentNullException(nameof(declarePipeline));

        builder.Services
            .AddSingleton<LocalQueryDispatcherMiddleware>()
            .AddSingleton<ILocalQueryContextFactory, LocalQueryContextFactory>()
            .AddTransient<ILocalQueryPipelineBuilder, LocalQueryPipelineBuilder>()
            .AddSingleton(provider =>
            {
                var pipelineBuilder = provider.GetRequiredService<ILocalQueryPipelineBuilder>();

                declarePipeline(pipelineBuilder);

                return pipelineBuilder.Build();
            });

        builder.Services.TryAddSingleton<ILocalClient, LocalClient>();
        builder.Services.TryAddSingleton<IQueryReportClient, QueryReportClient>();

        return builder;
    }

    public static ITotemBuilder AddQueueCommands(this ITotemBuilder builder, Action<IQueueCommandPipelineBuilder> declarePipeline)
    {
        if(builder is null)
            throw new ArgumentNullException(nameof(builder));

        if(declarePipeline is null)
            throw new ArgumentNullException(nameof(declarePipeline));

        builder.Services
            .AddSingleton<QueueCommandTopicMiddleware>()
            .AddSingleton<IQueueCommandContextFactory, QueueCommandContextFactory>()
            .AddTransient<IQueueCommandPipelineBuilder, QueueCommandPipelineBuilder>()
            .AddSingleton(provider =>
            {
                var pipelineBuilder = provider.GetRequiredService<IQueueCommandPipelineBuilder>();

                declarePipeline(pipelineBuilder);

                return pipelineBuilder.Build();
            });

        return builder;
    }

    public static ITotemBuilder AddEvents(this ITotemBuilder builder, Action<IEventPipelineBuilder> declarePipeline)
    {
        if(builder is null)
            throw new ArgumentNullException(nameof(builder));

        if(declarePipeline is null)
            throw new ArgumentNullException(nameof(declarePipeline));

        builder.Services
            .AddSingleton<EventHandlerMiddleware>()
            .AddSingleton<IEventContextFactory, EventContextFactory>()
            .AddTransient<IEventPipelineBuilder, EventPipelineBuilder>()
            .AddSingleton(provider =>
            {
                var pipelineBuilder = provider.GetRequiredService<IEventPipelineBuilder>();

                declarePipeline(pipelineBuilder);

                return pipelineBuilder.Build();
            });

        return builder;
    }

    public static ITotemBuilder AddEventHandlersAsServices(this ITotemBuilder builder)
    {
        if(builder is null)
            throw new ArgumentNullException(nameof(builder));

        foreach(var handler in builder.Services.GetRuntimeMap().EventHandlers)
        {
            builder.Services.AddTransient(handler.ServiceType, handler.DeclaredType);
        }

        return builder;
    }

    public static ITotemBuilder AddQueryHandlersAsServices(this ITotemBuilder builder)
    {
        if(builder is null)
            throw new ArgumentNullException(nameof(builder));

        foreach(var handler in builder.Services.GetRuntimeMap().QueryHandlers)
        {
            foreach(var serviceType in handler.ServiceTypes)
            {
                builder.Services.AddTransient(serviceType, handler.DeclaredType);
            }
        }

        return builder;
    }

    public static ITotemBuilder AddInMemoryWorkflowStore(this ITotemBuilder builder)
    {
        if(builder is null)
            throw new ArgumentNullException(nameof(builder));

        builder.Services.AddSingleton<InMemoryWorkflowStore>();
        builder.Services.AddSingleton<IWorkflowStore, InMemoryWorkflowStore>();

        return builder;
    }

    public static ITotemBuilder AddInMemoryReportStore(this ITotemBuilder builder)
    {
        if(builder is null)
            throw new ArgumentNullException(nameof(builder));

        builder.Services.AddSingleton<InMemoryReportStore>();
        builder.Services.AddSingleton<IReportStore, InMemoryReportStore>();

        return builder;
    }

    public static ITotemBuilder AddInMemoryStorage(this ITotemBuilder builder)
    {
        if(builder is null)
            throw new ArgumentNullException(nameof(builder));

        builder.Services.AddSingleton<IStorage, InMemoryStorage>();

        return builder;
    }

    public static ITotemBuilder AddInMemoryTopicStore(this ITotemBuilder builder)
    {
        if(builder is null)
            throw new ArgumentNullException(nameof(builder));

        builder.Services.AddSingleton<ITopicStore, InMemoryTopicStore>();
        builder.Services.AddSingleton<IInMemoryEventSubscription, InMemoryEventSubscription>();

        return builder;
    }

    public static ITotemBuilder AddInMemoryQueueClient(this ITotemBuilder builder)
    {
        if(builder is null)
            throw new ArgumentNullException(nameof(builder));

        builder.Services.AddSingleton<IQueueClient, InMemoryQueueClient>();

        return builder;
    }

    public static ITotemBuilder AddDiskFileStorage(this ITotemBuilder builder)
    {
        if(builder is null)
            throw new ArgumentNullException(nameof(builder));

        builder.Services.AddSingleton<IFileStorage, DiskStorage>();
        builder.Services.AddSingleton<IDiskStorageSettings>(provider =>
        {
            var configuration = provider.GetRequiredService<IConfiguration>();
            var configValue = configuration[DiskStorage.BaseDirectoryConfigurationKey];

            return !string.IsNullOrWhiteSpace(configValue)
                ? new DiskStorageSettings(configValue)
                : new DiskStorageSettings(AppContext.BaseDirectory);
        });

        return builder;
    }
}
