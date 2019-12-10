using System.ComponentModel;
using System.Reflection;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Totem.Runtime.Hosting;

namespace Totem.Timeline.Mvc.Hosting
{
  /// <summary>
  /// Extends <see cref="IMvcBuilder"/> to declare timeline MVC extensions
  /// </summary>
  [EditorBrowsable(EditorBrowsableState.Never)]
  public static class TimelineMvcBuilderExtensions
  {
    public static IMvcBuilder AddCommandsAndQueries(this IMvcBuilder mvc)
    {
      mvc.Services.TryAddSingleton<IHttpContextAccessor, HttpContextAccessor>();

      mvc.Services.AddScoped<IQueryServer, QueryServer>();
      mvc.Services.AddScoped<ICommandServer, CommandServer>();

      return mvc;
    }

    public static IMvcBuilder AddEntryAssemblyPart(this IMvcBuilder mvc) =>
      mvc.AddApplicationPart(Assembly.GetEntryAssembly());

    public static IMvcBuilder AddTotemWebRuntime(this IMvcBuilder mvc)
    {
      mvc.Services.AddOptionsSetup<WebRuntimeOptionsSetup>();

      return mvc;
    }
  }
}