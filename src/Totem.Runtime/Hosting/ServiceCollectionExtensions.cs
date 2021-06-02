using System;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Totem.Core;
using Totem.Features;
using Totem.Features.Default;
using Totem.Timelines;

namespace Totem.Hosting
{
    public static class ServiceCollectionExtensions
    {
        public static ITotemBuilder AddTotemRuntime(this IServiceCollection services)
        {
            if(services == null)
                throw new ArgumentNullException(nameof(services));

            services.TryAddSingleton<IClock, SystemClock>();
            services.AddSingleton(typeof(ITimelineRepository<>), typeof(TimelineRepository<>));

            var features = services.GetFeatures();

            AddDefaultProviders(features);

            return new TotemBuilder(services, features);
        }

        public static IServiceCollection ConfigureFeatures(this IServiceCollection services, Action<FeatureRegistry> configure)
        {
            if(services == null)
                throw new ArgumentNullException(nameof(services));

            if(configure == null)
                throw new ArgumentNullException(nameof(configure));

            configure(services.GetFeatures());

            return services;
        }

        public static FeatureRegistry GetFeatures(this IServiceCollection services)
        {
            if(services == null)
                throw new ArgumentNullException(nameof(services));

            var features = (FeatureRegistry?) services
                .LastOrDefault(x => x.ServiceType == typeof(FeatureRegistry))
                ?.ImplementationInstance;

            if(features == null)
            {
                features = new FeatureRegistry();

                services.AddSingleton(features);
            }

            return features;
        }

        static void AddDefaultProviders(FeatureRegistry features)
        {
            Add<EventHandlerFeatureProvider>();
            Add<HttpCommandProvider>();
            Add<HttpCommandHandlerProvider>();
            Add<HttpQueryProvider>();
            Add<HttpQueryHandlerProvider>();
            Add<LocalCommandHandlerProvider>();
            Add<LocalQueryHandlerProvider>();
            Add<QueueCommandHandlerProvider>();
            Add<ReportProvider>();
            Add<WorkflowProvider>();

            void Add<TProvider>() where TProvider : IFeatureProvider, new()
            {
                if(!features.Providers.OfType<TProvider>().Any())
                {
                    features.Providers.Add(new TProvider());
                }
            }
        }
    }
}