using System;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using Totem.Core;
using Totem.Features.Default;
using Totem.Queues;

namespace Totem.Hosting
{
    public static class QueueBuilderExtensions
    {
        public static ITotemBuilder AddQueueCommands(this ITotemBuilder builder, Action<IQueueCommandPipelineBuilder> declarePipeline)
        {
            if(builder == null)
                throw new ArgumentNullException(nameof(builder));

            if(declarePipeline == null)
                throw new ArgumentNullException(nameof(declarePipeline));

            builder.Services
                .AddSingleton<QueueCommandHandlerMiddleware>()
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

        public static ITotemBuilder AddQueueHandlersAsServices(this ITotemBuilder builder)
        {
            if(builder == null)
                throw new ArgumentNullException(nameof(builder));

            var feature = builder.Features.Populate<QueueCommandHandlerFeature>();

            foreach(var handler in feature.Handlers)
            {
                var service = handler.ImplementedInterfaces.First(i => i.IsGenericTypeDefinition(typeof(IQueueCommandHandler<>)));

                builder.Services.AddTransient(service, handler);
            }

            return builder;
        }
    }
}