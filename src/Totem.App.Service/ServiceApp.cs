using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Totem.Timeline.Hosting;

namespace Totem.App.Service
{
  /// <summary>
  /// Hosts an instance of a Totem service application
  /// </summary>
  public static class ServiceApp
  {
    public static Task Run<TArea>(ConfigureServiceApp configure) where TArea : TimelineArea, new()
    {
      var host = new HostBuilder();

      configure.ApplyHost(host);
      configure.ApplyHostConfiguration(host);
      configure.ApplyAppConfiguration(host);
      configure.ApplyServices<TArea>(host);
      configure.ApplySerilog(host);

      return host.Build().RunAsync();
    }

    public static Task Run<TArea>(Action<HostBuilderContext, IServiceCollection> configureServices) where TArea : TimelineArea, new() =>
      Run<TArea>(Configure.AfterServices(configureServices));

    public static Task Run<TArea>(Action<IServiceCollection> configureServices) where TArea : TimelineArea, new() =>
      Run<TArea>(Configure.AfterServices(configureServices));

    public static Task Run<TArea>() where TArea : TimelineArea, new() =>
      Run<TArea>(new ConfigureServiceApp());
  }
}