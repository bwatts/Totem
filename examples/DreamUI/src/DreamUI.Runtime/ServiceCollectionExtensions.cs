using System;
using Microsoft.Extensions.DependencyInjection;

namespace DreamUI
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddDream(this IServiceCollection services)
        {
            if(services == null)
                throw new ArgumentNullException(nameof(services));

            return services;
        }
    }
}
