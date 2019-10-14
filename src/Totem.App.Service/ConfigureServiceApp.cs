using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Events;
using Totem.Runtime.Hosting;
using Totem.Timeline.EventStore.Hosting;
using Totem.Timeline.Hosting;

namespace Totem.App.Service
{
  /// <summary>
  /// Configures the hosting of a Totem service application
  /// </summary>
  public class ConfigureServiceApp
  {
    class ServiceStep<TArg> : ConfigureStep<HostBuilderContext, TArg> {}

    readonly ConfigureStep<IHostBuilder> _host = new ConfigureStep<IHostBuilder>();
    readonly ConfigureStep<IConfigurationBuilder> _hostConfiguration = new ConfigureStep<IConfigurationBuilder>();
    readonly ServiceStep<IConfigurationBuilder> _appConfiguration = new ServiceStep<IConfigurationBuilder>();
    readonly ServiceStep<IServiceCollection> _services = new ServiceStep<IServiceCollection>();
    readonly ServiceStep<ITimelineBuilder> _timeline = new ServiceStep<ITimelineBuilder>();
    readonly ServiceStep<LoggerConfiguration> _serilog = new ServiceStep<LoggerConfiguration>();
    CancellationToken _cancellationToken;
    bool _disableSerilog;

    public ConfigureServiceApp CancellationToken(CancellationToken token)
    {
      _cancellationToken = token;

      return this;
    }

    public ConfigureServiceApp DisableSerilog()
    {
      _disableSerilog = true;

      return this;
    }

    //
    // Before/After/Replace
    //

    public ConfigureServiceApp BeforeHost(Action<IHostBuilder> configure) =>
      _host.Before(this, configure);

    public ConfigureServiceApp BeforeHostConfiguration(Action<IConfigurationBuilder> configure) =>
      _hostConfiguration.Before(this, configure);

    public ConfigureServiceApp BeforeAppConfiguration(Action<HostBuilderContext, IConfigurationBuilder> configure) =>
      _appConfiguration.Before(this, configure);

    public ConfigureServiceApp BeforeServices(Action<HostBuilderContext, IServiceCollection> configure) =>
      _services.Before(this, configure);

    public ConfigureServiceApp BeforeTimeline(Action<HostBuilderContext, ITimelineBuilder> configure) =>
      _timeline.Before(this, configure);

    public ConfigureServiceApp BeforeSerilog(Action<HostBuilderContext, LoggerConfiguration> configure) =>
      _serilog.Before(this, configure);

    public ConfigureServiceApp AfterHost(Action<IHostBuilder> configure) =>
      _host.After(this, configure);

    public ConfigureServiceApp AfterHostConfiguration(Action<IConfigurationBuilder> configure) =>
      _hostConfiguration.After(this, configure);

    public ConfigureServiceApp AfterAppConfiguration(Action<HostBuilderContext, IConfigurationBuilder> configure) =>
      _appConfiguration.After(this, configure);

    public ConfigureServiceApp AfterServices(Action<HostBuilderContext, IServiceCollection> configure) =>
      _services.After(this, configure);

    public ConfigureServiceApp AfterTimeline(Action<HostBuilderContext, ITimelineBuilder> configure) =>
      _timeline.After(this, configure);

    public ConfigureServiceApp AfterSerilog(Action<HostBuilderContext, LoggerConfiguration> configure) =>
      _serilog.After(this, configure);

    public ConfigureServiceApp ReplaceHost(Action<IHostBuilder> configure) =>
      _host.Replace(this, configure);

    public ConfigureServiceApp ReplaceHostConfiguration(Action<IConfigurationBuilder> configure) =>
      _hostConfiguration.Replace(this, configure);

    public ConfigureServiceApp ReplaceAppConfiguration(Action<HostBuilderContext, IConfigurationBuilder> configure) =>
      _appConfiguration.Replace(this, configure);

    public ConfigureServiceApp ReplaceServices(Action<HostBuilderContext, IServiceCollection> configure) =>
      _services.Replace(this, configure);

    public ConfigureServiceApp ReplaceTimeline(Action<HostBuilderContext, ITimelineBuilder> configure) =>
      _timeline.Replace(this, configure);

    public ConfigureServiceApp ReplaceSerilog(Action<HostBuilderContext, LoggerConfiguration> configure) =>
      _serilog.Replace(this, configure);

    //
    // Before/After/Replace (without context)
    //

    public ConfigureServiceApp BeforeAppConfiguration(Action<IConfigurationBuilder> configure) =>
      _appConfiguration.Before(this, configure);

    public ConfigureServiceApp BeforeServices(Action<IServiceCollection> configure) =>
      _services.Before(this, configure);

    public ConfigureServiceApp BeforeTimeline(Action<ITimelineBuilder> configure) =>
      _timeline.Before(this, configure);

    public ConfigureServiceApp BeforeSerilog(Action<LoggerConfiguration> configure) =>
      _serilog.Before(this, configure);

    public ConfigureServiceApp AfterAppConfiguration(Action<IConfigurationBuilder> configure) =>
      _appConfiguration.After(this, configure);

    public ConfigureServiceApp AfterServices(Action<IServiceCollection> configure) =>
      _services.After(this, configure);

    public ConfigureServiceApp AfterTimeline(Action<ITimelineBuilder> configure) =>
      _timeline.After(this, configure);

    public ConfigureServiceApp AfterSerilog(Action<LoggerConfiguration> configure) =>
      _serilog.After(this, configure);

    public ConfigureServiceApp ReplaceAppConfiguration(Action<IConfigurationBuilder> configure) =>
      _appConfiguration.Replace(this, configure);

    public ConfigureServiceApp ReplaceServices(Action<IServiceCollection> configure) =>
      _services.Replace(this, configure);

    public ConfigureServiceApp ReplaceTimeline(Action<ITimelineBuilder> configure) =>
      _timeline.Replace(this, configure);

    public ConfigureServiceApp ReplaceSerilog(Action<LoggerConfiguration> configure) =>
      _serilog.Replace(this, configure);

    //
    // Apply
    //

    public void ApplyHost(IHostBuilder host) =>
      _host.Apply(host);

    public void ApplyHostConfiguration(IHostBuilder host) =>
      host.ConfigureHostConfiguration(hostConfiguration =>
        _hostConfiguration.Apply(hostConfiguration, () =>
        {
          var pairs = new Dictionary<string, string>
          {
            [HostDefaults.EnvironmentKey] = Environment.GetEnvironmentVariable("NETCORE_ENVIRONMENT") ?? EnvironmentName.Development
          };

          hostConfiguration.AddInMemoryCollection(pairs);
        }));

    public void ApplyAppConfiguration(IHostBuilder host) =>
      host.ConfigureAppConfiguration((context, appConfiguration) =>
        _appConfiguration.Apply(context, appConfiguration, () =>
        {
          appConfiguration
          .AddEnvironmentVariables()
          .AddCommandLine(Environment.GetCommandLineArgs())
          .AddJsonFile("appsettings.json", optional: true)
          .AddJsonFile($"appsettings.{context.HostingEnvironment.EnvironmentName}.json", optional: true);

          if(context.HostingEnvironment.IsDevelopment())
          {
            appConfiguration.AddUserSecrets(Assembly.GetEntryAssembly(), optional: true);
          }
        }));

    public void ApplyServices<TArea>(IHostBuilder host) where TArea : TimelineArea, new() =>
      host.ConfigureServices((context, services) =>
        _services.Apply(context, services, () =>
        {
          services.AddTotemRuntime();

          services.AddTimeline<TArea>(timeline =>
            _timeline.Apply(context, timeline, () =>
              timeline.AddEventStore().BindOptionsToConfiguration()));

          // Allow an external host (such as a Windows Service) to stop the application
          services.AddSingleton<IHostedService>(p =>
            new ServiceAppCancellation(p.GetService<IApplicationLifetime>(), _cancellationToken));
        }));

    public void ApplySerilog(IHostBuilder host)
    {
      if(_disableSerilog)
      {
        return;
      }

      host.UseSerilog((context, serilog) =>
        _serilog.Apply(context, serilog, () =>
        {
          if(Environment.UserInteractive)
          {
            serilog.WriteTo.Console();
          }

          if(context.HostingEnvironment.IsDevelopment())
          {
            serilog
            .MinimumLevel.Information()
            .MinimumLevel.Override("System", LogEventLevel.Warning)
            .MinimumLevel.Override("Microsoft", LogEventLevel.Warning);
          }
          else
          {
            serilog.MinimumLevel.Warning();
          }

          serilog.ReadFrom.Configuration(context.Configuration);
        }));
    }
  }
}