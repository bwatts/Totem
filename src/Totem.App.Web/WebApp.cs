using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
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
    public static Task Run<TArea>(ConfigureWebApp configure) where TArea : TimelineArea, new()
    {
      var host = WebHost.CreateDefaultBuilder();

      configure.ApplyHost(host);
      configure.ApplyApp(host);
      configure.ApplyAppConfiguration(host);
      configure.ApplyServices<TArea>(host);
      configure.ApplySerilog(host);

      return host.Build().RunAsync();
    }

    public static Task Run<TArea>(Action<WebHostBuilderContext, IServiceCollection> configureServices) where TArea : TimelineArea, new() =>
      Run<TArea>(Configure.AfterServices(configureServices));

    public static Task Run<TArea>(Action<IServiceCollection> configureServices) where TArea : TimelineArea, new() =>
      Run<TArea>(Configure.AfterServices(configureServices));

    public static Task Run<TArea>() where TArea : TimelineArea, new() =>
      Run<TArea>(new ConfigureWebApp());
  }
}