using Majorsoft.Blazor.Components.Common.JsInterop;
using Microsoft.Extensions.DependencyInjection;

namespace DreamUI.Hosting
{
    internal static class ServiceCollectionExtensions
    {
        internal static IServiceCollection AddDreamUI(this IServiceCollection services) =>
            services.AddJsInteropExtensions();
    }
}