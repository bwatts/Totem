using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Extensions.DependencyInjection;
using Totem.Commands;
using Totem.Queries;

namespace Totem.Hosting
{
    public static class TotemClientBuilderExtensions
    {
        public static ITotemClientBuilder AddCommands(
            this ITotemClientBuilder builder,
            Action<IClientCommandPipelineBuilder> declarePipeline,
            Action<JsonSerializerOptions>? configureJson = null)
        {
            if(builder == null)
                throw new ArgumentNullException(nameof(builder));

            if(declarePipeline == null)
                throw new ArgumentNullException(nameof(declarePipeline));

            builder.Services
                .AddSingleton<ClientCommandHttpMiddleware>()
                .AddSingleton<IClientCommandNegotiator, ClientCommandNegotiator>()
                .AddSingleton<IClientCommandContextFactory, ClientCommandContextFactory>()
                .AddTransient<IClientCommandPipelineBuilder, ClientCommandPipelineBuilder>()
                .AddSingleton(provider =>
                 {
                     var pipelineBuilder = provider.GetRequiredService<IClientCommandPipelineBuilder>();

                     declarePipeline(pipelineBuilder);

                     return pipelineBuilder.Build();
                 });

            var jsonOptions = new JsonSerializerOptions(JsonSerializerDefaults.Web)
            {
                WriteIndented = true,
                Converters = { new JsonStringEnumConverter() }
            };

            configureJson?.Invoke(jsonOptions);

            builder.Services.AddSingleton(jsonOptions);

            return builder;
        }

        public static ITotemClientBuilder AddQueries(this ITotemClientBuilder builder, Action<IClientQueryPipelineBuilder> declarePipeline)
        {
            if(builder == null)
                throw new ArgumentNullException(nameof(builder));

            if(declarePipeline == null)
                throw new ArgumentNullException(nameof(declarePipeline));

            builder.Services
                .AddSingleton<ClientQueryHttpMiddleware>()
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