using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Totem.Runtime
{
  /// <summary>
  /// A stateful connection to a resource
  /// </summary>
  public abstract class Connection : Notion, IConnectable
  {
    readonly List<IDisposable> _trackedLifetimes = new List<IDisposable>();
    readonly List<IConnectable> _trackedConnections = new List<IConnectable>();

    public ConnectionState State { get; } = new ConnectionState();

    //
    // Connecting
    //

    public async Task Connect(CancellationToken cancellationToken = default(CancellationToken))
    {
      State.OnConnecting(cancellationToken);

      try
      {
        await TryOpen();
      }
      catch
      {
        State.OnConnectFailed();

        throw;
      }
    }

    public Task Connect(IConnectable connection) =>
      Connect(connection.State.CancellationToken);

    async Task TryOpen()
    {
      var errors = new List<Exception>();

      try
      {
        await Open();

        State.OnConnected();
      }
      catch(Exception error)
      {
        errors.Add(error);

        State.OnDisconnected();
      }

      if(errors.Count == 0)
      {
        await StartTracking();
      }
      else
      {
        try
        {
          await StopTracking();
        }
        catch(Exception error)
        {
          errors.Add(error);
        }

        if(errors.Any())
        {
          throw new AggregateException($"Connection of type {GetType()} failed to open", errors).Flatten();
        }
      }
    }

    protected virtual Task Open() =>
      Task.CompletedTask;

    //
    // Disconnecting
    //

    public async Task Disconnect()
    {
      State.OnDisconnecting();

      try
      {
        await TryClose();
      }
      finally
      {
        State.OnDisconnected();
      }
    }

    async Task TryClose()
    {
      var errors = new List<Exception>();

      try
      {
        await StopTracking();
      }
      catch(Exception error)
      {
        errors.Add(error);
      }

      try
      {
        await Close();
      }
      catch(Exception error)
      {
        errors.Add(error);
      }

      if(errors.Count > 0)
      {
        throw new AggregateException($"Connection of type {GetType()} failed to close", errors).Flatten();
      }
    }

    protected virtual Task Close() =>
      Task.CompletedTask;

    //
    // Tracking
    //

    protected void Track(IDisposable lifetime)
    {
      State.ExpectConnecting();

      _trackedLifetimes.Add(lifetime);
    }

    protected void Track(IEnumerable<IDisposable> lifetimes)
    {
      State.ExpectConnecting();

      _trackedLifetimes.AddRange(lifetimes);
    }

    protected void Track(IConnectable connection)
    {
      State.ExpectConnecting();

      _trackedConnections.Add(connection);
    }

    protected void Track(IEnumerable<IConnectable> connections)
    {
      State.ExpectConnecting();

      _trackedConnections.AddRange(connections);
    }

    async Task StartTracking()
    {
      var errors = new List<Exception>();

      foreach(var connection in _trackedConnections)
      {
        if(connection.State.IsConnected)
        {
          continue;
        }

        try
        {
          await connection.Connect(State.CancellationToken);
        }
        catch(Exception error)
        {
          errors.Add(error);
        }
      }

      if(errors.Count > 0)
      {
        throw new AggregateException($"Connection of type {GetType()} failed to start tracking", errors).Flatten();
      }
    }

    async Task StopTracking()
    {
      var errors = new List<Exception>();

      foreach(var lifetime in _trackedLifetimes.AsEnumerable().Reverse())
      {
        try
        {
          lifetime.Dispose();
        }
        catch(Exception error)
        {
          errors.Add(error);
        }
      }

      foreach(var connection in _trackedConnections.AsEnumerable().Reverse())
      {
        try
        {
          await connection.Disconnect();
        }
        catch(Exception error)
        {
          errors.Add(error);
        }
      }

      if(errors.Count > 0)
      {
        throw new AggregateException($"Connection of type {GetType()} failed to stop tracking", errors).Flatten();
      }
    }

    //
    // None
    //

    public static readonly IConnectable None = new NoneConnection();

    class NoneConnection : IConnectable
    {
      public ConnectionState State { get; } = new ConnectionState();

      public Task Connect(CancellationToken cancellationToken = default(CancellationToken)) =>
        Task.CompletedTask;

      public Task Connect(IConnectable connection) =>
        Task.CompletedTask;

      public Task Disconnect() =>
        Task.CompletedTask;
    }
  }
}