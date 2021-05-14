using Microsoft.Extensions.DependencyInjection;

namespace Totem.Hosting
{
    internal class TotemClientBuilder : ITotemClientBuilder
    {
        internal TotemClientBuilder(IServiceCollection services) =>
            Services = services;

        public IServiceCollection Services { get; }
    }
}