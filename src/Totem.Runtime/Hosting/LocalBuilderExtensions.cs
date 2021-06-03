using System;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Totem.Core;
using Totem.Features.Default;
using Totem.Local;

namespace Totem.Hosting
{
    public static class LocalBuilderExtensions
    {
        public static ITotemBuilder AddLocalCommands(this ITotemBuilder builder, Action<ILocalCommandPipelineBuilder> declarePipeline)
        {
            if(builder == null)
                throw new ArgumentNullException(nameof(builder));

            if(declarePipeline == null)
                throw new ArgumentNullException(nameof(declarePipeline));

            builder.Services
                .AddSingleton<LocalCommandHandlerMiddleware>()
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
            if(builder == null)
                throw new ArgumentNullException(nameof(builder));

            if(declarePipeline == null)
                throw new ArgumentNullException(nameof(declarePipeline));

            builder.Services
                .AddSingleton<LocalQueryHandlerMiddleware>()
                .AddSingleton<ILocalQueryContextFactory, LocalQueryContextFactory>()
                .AddTransient<ILocalQueryPipelineBuilder, LocalQueryPipelineBuilder>()
                .AddSingleton(provider =>
                {
                    var pipelineBuilder = provider.GetRequiredService<ILocalQueryPipelineBuilder>();

                    declarePipeline(pipelineBuilder);

                    return pipelineBuilder.Build();
                });

            builder.Services.TryAddSingleton<ILocalClient, LocalClient>();

            return builder;
        }

        public static ITotemBuilder AddLocalCommandHandlersAsServices(this ITotemBuilder builder)
        {
            if(builder == null)
                throw new ArgumentNullException(nameof(builder));

            var feature = builder.Features.Populate<LocalCommandHandlerFeature>();

            foreach(var handler in feature.Handlers)
            {
                var service = handler.ImplementedInterfaces.First(i => i.IsGenericTypeDefinition(typeof(ILocalCommandHandler<>)));

                builder.Services.AddTransient(service, handler);
            }

            return builder;
        }

        public static ITotemBuilder AddLocalQueryHandlersAsServices(this ITotemBuilder builder)
        {
            if(builder == null)
                throw new ArgumentNullException(nameof(builder));

            var feature = builder.Features.Populate<LocalQueryHandlerFeature>();

            foreach(var handler in feature.Handlers)
            {
                var service = handler.ImplementedInterfaces.First(i => i.IsGenericTypeDefinition(typeof(ILocalQueryHandler<>)));

                builder.Services.AddTransient(service, handler);
            }

            return builder;
        }
    }
}