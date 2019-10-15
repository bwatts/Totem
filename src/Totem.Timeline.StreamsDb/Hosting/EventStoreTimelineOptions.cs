using System;

namespace Totem.Timeline.EventStore.Hosting
{
  /// <summary>
  /// Configures the timeline's connection to an instance of EventStore
  /// </summary>
  public class EventStoreTimelineOptions
  {
    public bool Verbose { get; set; } = false;
    public ServerOptions Server { get; set; } = new ServerOptions();
    public ConnectionOptions Connection { get; set; } = new ConnectionOptions();
    public ReconnectOptions Reconnects { get; set; } = new ReconnectOptions();
    public HeartbeatOptions Heartbeat { get; set; } = new HeartbeatOptions();
    public OperationOptions Operations { get; set; } = new OperationOptions();
    public SubscriptionOptions Subscription { get; set; } = new SubscriptionOptions();
    public ProjectionOptions Projections { get; set; } = new ProjectionOptions();

    public class ServerOptions
    {
      public string Name { get; set; } = "localhost";
      public int TcpPort { get; set; } = 1113;
      public int HttpPort { get; set; } = 2113;
    }

    public class ConnectionOptions
    {
      public string Username { get; set; }
      public string Password { get; set; }
      public TimeSpan Timeout { get; set; } = TimeSpan.FromSeconds(1);
    }

    public class ReconnectOptions
    {
      public int Limit { get; set; } = 5;
      public TimeSpan Delay { get; set; } = TimeSpan.FromMilliseconds(500);
    }

    public class HeartbeatOptions
    {
      public TimeSpan Interval { get; set; } = TimeSpan.FromMilliseconds(750);
      public TimeSpan Timeout { get; set; } = TimeSpan.FromMilliseconds(1500);
    }

    public class OperationOptions
    {
      public TimeSpan Timeout { get; set; } = TimeSpan.FromSeconds(7);
      public TimeSpan TimeoutCheckPeriod { get; set; } = TimeSpan.FromSeconds(1);
      public int AttemptLimit { get; set; } = 11;
      public int RetryLimit { get; set; } = 10;
      public int QueueLimit { get; set; } = 5000;
      public bool FailOnNoServerResponse { get; set; } = false;
    }

    public class SubscriptionOptions
    {
      public int MaxLiveQueueSize { get; set; } = 100000;
      public int ReadBatchSize { get; set; } = 500;
    }

    public class ProjectionOptions
    {
      public TimeSpan InstallTimeout { get; set; } = TimeSpan.FromSeconds(5);
    }
  }
}