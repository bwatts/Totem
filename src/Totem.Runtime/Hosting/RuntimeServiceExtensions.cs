using System.ComponentModel;
using Microsoft.Extensions.DependencyInjection;

namespace Totem.Runtime.Hosting
{
  /// <summary>
  /// Extends <see cref="IWebHostBuilder"/> to declare the Totem JSON format
  /// </summary>
  [EditorBrowsable(EditorBrowsableState.Never)]
  public static class RuntimeServiceExtensions
  {
    public static IServiceCollection AddTotemRuntime(this IServiceCollection services) =>
      services.AddJsonFormat().AddHostedService<RuntimeLogService>();
  }
}