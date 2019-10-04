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
    readonly List<object> _trackedObjects = new List<object>();

    public ConnectionState State { get; } = new ConnectionState();

    //
    // Connecting
    //

    public async Task Connect(CancellationToken cancellationToken = default)
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
      await Open();

      State.OnConnected();

      try
      {
        await StartTracking();
      }
      catch(Exception startError)
      {
        try
        {
          await StopTracking();
        }
        catch(Exception stopError)
        {
          throw new AggregateException(startError, stopError);
        }

        throw;
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
      await StopTracking();
      await Close();
    }

    protected virtual Task Close() =>
      Task.CompletedTask;

    //
    // Tracking
    //

    protected void Track(IDisposable lifetime)
    {
      State.ExpectConnecting();

      _trackedObjects.Add(lifetime);
    }

    protected void Track(IEnumerable<IDisposable> lifetimes)
    {
      State.ExpectConnecting();

      _trackedObjects.AddRange(lifetimes);
    }

    protected void Track(IConnectable connection)
    {
      State.ExpectConnecting();

      _trackedObjects.Add(connection);
    }

    protected void Track(IEnumerable<IConnectable> connections)
    {
      State.ExpectConnecting();

      _trackedObjects.AddRange(connections);
    }

    async Task StartTracking()
    {
      foreach(var connection in _trackedObjects.OfType<IConnectable>())
      {
        if(connection.State.IsDisconnected)
        {
          await connection.Connect(this);
        }
      }
    }

    async Task StopTracking()
    {
      var errors = new List<Exception>();

      foreach(var trackedObject in _trackedObjects.AsEnumerable().Reverse())
      {
        try
        {
          if(trackedObject is IConnectable connection)
          {
            if(connection.State.IsConnected)
            {
              await connection.Disconnect();
            }
          }
          else
          {
            ((IDisposable) trackedObject).Dispose();
          }
        }
        catch(Exception error)
        {
          errors.Add(error);
        }
      }

      if(errors.Count > 0)
      {
        var message = $"Connection failed to stop tracking one or more objects: {GetType()} ";

        if(errors.Count == 1)
        {
          throw new Exception(message, errors[0]);
        }
        else
        {
          throw new AggregateException(message, errors).Flatten();
        }
      }
    }

    //
    // None
    //

    public static readonly IConnectable None = new NoneConnection();

    class NoneConnection : IConnectable
    {
      public ConnectionState State { get; } = new ConnectionState();

      public Task Connect(CancellationToken cancellationToken = default) =>
        Task.CompletedTask;

      public Task Connect(IConnectable connection) =>
        Task.CompletedTask;

      public Task Disconnect() =>
        Task.CompletedTask;
    }
  }
}