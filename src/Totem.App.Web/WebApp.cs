using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Totem.Timeline.Hosting;

namespace Totem.App.Web
{
  /// <summary>
  /// Hosts an instance of a Totem web application
  /// </summary>
  public static class WebApp
  {
    public static Task Run<TArea>(ConfigureWebApp configure) where TArea : TimelineArea, new() =>
      new WebAppHost<TArea>(configure).Run();

    public static Task Run<TArea>(Action<WebHostBuilderContext, IServiceCollection> configureServices) where TArea : TimelineArea, new() =>
      Run<TArea>(Configure.Services(configureServices));

    public static Task Run<TArea>(Action<IServiceCollection> configureServices) where TArea : TimelineArea, new() =>
      Run<TArea>(Configure.Services(configureServices));

    public static Task Run<TArea>() where TArea : TimelineArea, new() =>
      Run<TArea>(new ConfigureWebApp());
  }
}