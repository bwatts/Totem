using Majorsoft.Blazor.Components.Common.JsInterop;

namespace DreamUI.Hosting;

internal static class ServiceCollectionExtensions
{
    internal static IServiceCollection AddDreamWebAssembly(this IServiceCollection services) =>
        services.AddJsInteropExtensions();
}
