using System;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using Totem.Core;
using Totem.Features.Default;
using Totem.Http;

namespace Totem.Hosting
{
    public static class HttpBuilderExtensions
    {
        public static ITotemBuilder AddHttpCommands(this ITotemBuilder builder, Action<IHttpCommandPipelineBuilder> declarePipeline)
        {
            if(builder == null)
                throw new ArgumentNullException(nameof(builder));

            if(declarePipeline == null)
                throw new ArgumentNullException(nameof(declarePipeline));

            builder.Services
                .AddSingleton<HttpCommandHandlerMiddleware>()
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
            if(builder == null)
                throw new ArgumentNullException(nameof(builder));

            if(declarePipeline == null)
                throw new ArgumentNullException(nameof(declarePipeline));

            builder.Services
                .AddSingleton<HttpQueryHandlerMiddleware>()
                .AddSingleton<IHttpQueryContextFactory, HttpQueryContextFactory>()
                .AddTransient<IHttpQueryPipelineBuilder, HttpQueryPipelineBuilder>()
                .AddSingleton(provider =>
                {
                    var pipelineBuilder = provider.GetRequiredService<IHttpQueryPipelineBuilder>();

                    declarePipeline(pipelineBuilder);

                    return pipelineBuilder.Build();
                });

            return builder;
        }

        public static ITotemBuilder AddHttpCommandHandlersAsServices(this ITotemBuilder builder)
        {
            if(builder == null)
                throw new ArgumentNullException(nameof(builder));

            var feature = builder.Features.Populate<HttpCommandHandlerFeature>();

            foreach(var handler in feature.Handlers)
            {
                var service = handler.ImplementedInterfaces.First(i => i.IsGenericTypeDefinition(typeof(IHttpCommandHandler<>)));

                builder.Services.AddTransient(service, handler);
            }

            return builder;
        }

        public static ITotemBuilder AddHttpQueryHandlersAsServices(this ITotemBuilder builder)
        {
            if(builder == null)
                throw new ArgumentNullException(nameof(builder));

            var feature = builder.Features.Populate<HttpQueryHandlerFeature>();

            foreach(var handler in feature.Handlers)
            {
                var service = handler.ImplementedInterfaces.First(i => i.IsGenericTypeDefinition(typeof(IHttpQueryHandler<>)));

                builder.Services.AddTransient(service, handler);
            }

            return builder;
        }
    }
}