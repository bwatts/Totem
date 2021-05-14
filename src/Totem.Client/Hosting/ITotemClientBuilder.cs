using Microsoft.Extensions.DependencyInjection;

namespace Totem.Hosting
{
    public interface ITotemClientBuilder
    {
        IServiceCollection Services { get; }
    }
}