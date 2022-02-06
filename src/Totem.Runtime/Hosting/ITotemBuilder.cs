using Microsoft.Extensions.DependencyInjection;
using Totem.Map;

namespace Totem.Hosting;

public interface ITotemBuilder
{
    IServiceCollection Services { get; }
    RuntimeMap Map { get; }
}
