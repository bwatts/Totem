using System;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using Totem.Core;
using Totem.Events;
using Totem.Features.Default;
using Totem.Routes;

namespace Totem.Hosting
{
    public static class EventBuilderExtensions
    {
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

        public static ITotemBuilder AddEventHandlersAsServices(this ITotemBuilder builder)
        {
            if(builder == null)
                throw new ArgumentNullException(nameof(builder));

            var feature = builder.Features.Populate<EventHandlerFeature>();

            foreach(var handler in feature.Handlers)
            {
                var service = handler.ImplementedInterfaces.First(i => i.IsGenericTypeDefinition(typeof(IEventHandler<>)));

                builder.Services.AddTransient(service, handler);
            }

            return builder;
        }
    }
}