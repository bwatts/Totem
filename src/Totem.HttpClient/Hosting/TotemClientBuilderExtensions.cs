using Microsoft.Extensions.DependencyInjection;
using Totem.Http.Commands;
using Totem.Http.Queries;

namespace Totem.Hosting;

public static class TotemClientBuilderExtensions
{
    public static ITotemClientBuilder AddCommands(this ITotemClientBuilder builder, Action<IHttpCommandClientPipelineBuilder> declarePipeline)
    {
        if(builder is null)
            throw new ArgumentNullException(nameof(builder));

        if(declarePipeline is null)
            throw new ArgumentNullException(nameof(declarePipeline));

        builder.Services
            .AddSingleton<HttpCommandClientRequestMiddleware>()
            .AddSingleton<IHttpCommandNegotiator, HttpCommandNegotiator>()
            .AddSingleton<IHttpCommandClientContextFactory, HttpCommandClientContextFactory>()
            .AddTransient<IHttpCommandClientPipelineBuilder, HttpCommandClientPipelineBuilder>()
            .AddSingleton(provider =>
             {
                 var pipelineBuilder = provider.GetRequiredService<IHttpCommandClientPipelineBuilder>();

                 declarePipeline(pipelineBuilder);

                 return pipelineBuilder.Build();
             });

        return builder;
    }

    public static ITotemClientBuilder AddQueries(this ITotemClientBuilder builder, Action<IHttpQueryClientPipelineBuilder> declarePipeline)
    {
        if(builder is null)
            throw new ArgumentNullException(nameof(builder));

        if(declarePipeline is null)
            throw new ArgumentNullException(nameof(declarePipeline));

        builder.Services
            .AddSingleton<HttpQueryClientRequestMiddleware>()
            .AddSingleton<IHttpQueryNegotiator, HttpQueryNegotiator>()
            .AddSingleton<IHttpQueryClientContextFactory, HttpQueryClientContextFactory>()
            .AddTransient<IHttpQueryClientPipelineBuilder, HttpQueryClientPipelineBuilder>()
            .AddSingleton(provider =>
            {
                var pipelineBuilder = provider.GetRequiredService<IHttpQueryClientPipelineBuilder>();

                declarePipeline(pipelineBuilder);

                return pipelineBuilder.Build();
            });

        return builder;
    }
}
