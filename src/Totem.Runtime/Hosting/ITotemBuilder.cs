using Microsoft.Extensions.DependencyInjection;
using Totem.Features;

namespace Totem.Hosting
{
    public interface ITotemBuilder
    {
        IServiceCollection Services { get; }
        FeatureRegistry Features { get; }
    }
}