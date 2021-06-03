using System;
using DreamUI.Installations;
using Microsoft.Extensions.DependencyInjection;

namespace DreamUI.Hosting
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddDreamUI(this IServiceCollection services)
        {
            if(services == null)
                throw new ArgumentNullException(nameof(services));

            services.AddSingleton<IInstallationRepository, InstallationRepository>();

            return services;
        }
    }
}