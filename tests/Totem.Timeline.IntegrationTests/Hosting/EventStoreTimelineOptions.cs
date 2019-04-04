using System;

namespace Totem.Timeline.IntegrationTests.Hosting
{
  /// <summary>
  /// Configures the EventStore process for an integration test
  /// </summary>
  public class EventStoreProcessOptions
  {
    public string ExeFile { get; set; }
    public TimeSpan ReadyDelay { get; set; } = TimeSpan.FromMilliseconds(500);
  }
}