using System;
using System.Reflection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Totem.Features;
using Totem.Files;
using Totem.InMemory;
using Totem.Routes;
using Totem.Timelines;

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

        public static ITotemBuilder AddInMemoryTimelineStore(this ITotemBuilder builder)
        {
            if(builder == null)
                throw new ArgumentNullException(nameof(builder));

            builder.Services.AddSingleton<ITimelineStore, InMemoryTimelineStore>();
            builder.Services.AddSingleton<IInMemoryEventSubscription, InMemoryEventSubscription>();

            return builder;
        }

        public static ITotemBuilder AddInMemoryRouteStore(this ITotemBuilder builder)
        {
            if(builder == null)
                throw new ArgumentNullException(nameof(builder));

            builder.Services.AddSingleton<IRouteStore, InMemoryRouteStore>();

            return builder;
        }

        public static ITotemBuilder AddInMemoryQueueClient(this ITotemBuilder builder)
        {
            if(builder == null)
                throw new ArgumentNullException(nameof(builder));

            builder.Services.AddSingleton<IQueueClient, InMemoryQueueClient>();

            return builder;
        }

        public static ITotemBuilder AddInMemoryStorage(this ITotemBuilder builder)
        {
            if(builder == null)
                throw new ArgumentNullException(nameof(builder));

            builder.Services.AddSingleton<IStorage, InMemoryStorage>();

            return builder;
        }

        public static ITotemBuilder AddDiskFileStorage(this ITotemBuilder builder)
        {
            if(builder == null)
                throw new ArgumentNullException(nameof(builder));

            builder.Services.AddSingleton<IFileStorage, DiskStorage>();
            builder.Services.AddSingleton<IDiskStorageSettings>(provider =>
            {
                var configuration = provider.GetRequiredService<IConfiguration>();
                var configValue = configuration["Totem:Files:DiskStorage:BaseDirectory"];

                return !string.IsNullOrWhiteSpace(configValue)
                    ? new DiskStorageSettings(configValue)
                    : new DiskStorageSettings(AppContext.BaseDirectory);
            });

            return builder;
        }
    }
}