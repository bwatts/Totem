using System;
using Microsoft.Extensions.DependencyInjection;
using Totem.Commands;
using Totem.Queries;

namespace Totem.Hosting
{
    public static class TotemClientBuilderExtensions
    {
        public static ITotemClientBuilder AddCommands(this ITotemClientBuilder builder, Action<IClientCommandPipelineBuilder> declarePipeline)
        {
            if(builder == null)
                throw new ArgumentNullException(nameof(builder));

            if(declarePipeline == null)
                throw new ArgumentNullException(nameof(declarePipeline));

            builder.Services
                .AddSingleton<ClientCommandRequestMiddleware>()
                .AddSingleton<IClientCommandNegotiator, ClientCommandNegotiator>()
                .AddSingleton<IClientCommandContextFactory, ClientCommandContextFactory>()
                .AddTransient<IClientCommandPipelineBuilder, ClientCommandPipelineBuilder>()
                .AddSingleton(provider =>
                 {
                     var pipelineBuilder = provider.GetRequiredService<IClientCommandPipelineBuilder>();

                     declarePipeline(pipelineBuilder);

                     return pipelineBuilder.Build();
                 });

            return builder;
        }

        public static ITotemClientBuilder AddQueries(this ITotemClientBuilder builder, Action<IClientQueryPipelineBuilder> declarePipeline)
        {
            if(builder == null)
                throw new ArgumentNullException(nameof(builder));

            if(declarePipeline == null)
                throw new ArgumentNullException(nameof(declarePipeline));

            builder.Services
                .AddSingleton<ClientQueryRequestMiddleware>()
                .AddSingleton<IClientQueryNegotiator, ClientQueryNegotiator>()
                .AddSingleton<IClientQueryContextFactory, ClientQueryContextFactory>()
                .AddTransient<IClientQueryPipelineBuilder, ClientQueryPipelineBuilder>()
                .AddSingleton(provider =>
                {
                    var pipelineBuilder = provider.GetRequiredService<IClientQueryPipelineBuilder>();

                    declarePipeline(pipelineBuilder);

                    return pipelineBuilder.Build();
                });

            return builder;
        }
    }
}