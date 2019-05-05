using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;

namespace Totem.Timeline.IntegrationTests.Hosting
{
  /// <summary>
  /// Controls the lifetime of a timeline declared by a test fixture
  /// </summary>
  internal sealed class TestLifetime : IHostLifetime
  {
    readonly TestApp _app;
    readonly TestTimeline _timeline;
    readonly IApplicationLifetime _appLifetime;

    internal TestLifetime(TestApp app, TestTimeline timeline, IApplicationLifetime appLifetime)
    {
      _app = app;
      _timeline = timeline;
      _appLifetime = appLifetime;

      appLifetime.ApplicationStarted.Register(OnAppStarted);
      appLifetime.ApplicationStopped.Register(OnAppStopped);
    }

    internal void StopApp() =>
      _appLifetime.StopApplication();

    Task IHostLifetime.WaitForStartAsync(CancellationToken cancellationToken) =>
      _timeline.Connect();

    Task IHostLifetime.StopAsync(CancellationToken cancellationToken) =>
      _timeline.Disconnect();

    void OnAppStarted() =>
      _app.OnStarted(this, _timeline);

    void OnAppStopped() =>
      _app.OnStopped();
  }
}