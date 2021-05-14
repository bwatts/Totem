using Microsoft.Extensions.DependencyInjection;
using Totem.Features;

namespace Totem.Hosting
{
    internal class TotemBuilder : ITotemBuilder
    {
        internal TotemBuilder(IServiceCollection services, FeatureRegistry features)
        {
            Services = services;
            Features = features;
        }

        public IServiceCollection Services { get; }
        public FeatureRegistry Features { get; }
    }
}