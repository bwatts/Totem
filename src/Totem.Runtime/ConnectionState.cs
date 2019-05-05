using System;
using System.Threading;

namespace Totem.Runtime
{
  /// <summary>
  /// The observable state of a connection to a resource
  /// </summary>
  public class ConnectionState
  {
    CancellationTokenRegistration _cancellationTokenRegistration;

    public CancellationToken CancellationToken { get; private set; }
    public ConnectionPhase Phase { get; private set; }

    public bool IsDisconnected => Phase == ConnectionPhase.Disconnected;
    public bool IsConnecting => Phase == ConnectionPhase.Connecting;
    public bool IsConnected => Phase == ConnectionPhase.Connected;
    public bool IsCancelled => Phase == ConnectionPhase.Cancelled;
    public bool IsDisconnecting => Phase == ConnectionPhase.Disconnecting;

    internal void OnConnecting(CancellationToken cancellationToken)
    {
      Phase = ConnectionPhase.Connecting;

      CancellationToken = cancellationToken;

      _cancellationTokenRegistration = cancellationToken.Register(OnCancelled);
    }

    internal void OnConnectFailed()
    {
      Phase = ConnectionPhase.Disconnected;

      CancellationToken = default(CancellationToken);
    }

    internal void OnConnected()
    {
      Phase = ConnectionPhase.Connected;
    }

    internal void OnDisconnecting()
    {
      Phase = ConnectionPhase.Disconnecting;

      UnregisterCancellationToken();
    }

    internal void OnDisconnected()
    {
      Phase = ConnectionPhase.Disconnected;
    }

    void OnCancelled()
    {
      Phase = ConnectionPhase.Cancelled;

      UnregisterCancellationToken();
    }

    void UnregisterCancellationToken()
    {
      try
      {
        _cancellationTokenRegistration.Dispose();
      }
      catch(NullReferenceException)
      {
        // .NET code sometimes fails here - ignore if so, can't do anything else
      }

      _cancellationTokenRegistration = default(CancellationTokenRegistration);
    }

    //
    // Expectations
    //

    public void ExpectDisconnected() =>
      Expect.That(Phase).Is(ConnectionPhase.Disconnected);

    public void ExpectConnecting() =>
      Expect.That(Phase).Is(ConnectionPhase.Connecting);

    public void ExpectConnected() =>
      Expect.That(Phase).Is(ConnectionPhase.Connected);

    public void ExpectCancelled() =>
      Expect.That(Phase).Is(ConnectionPhase.Cancelled);

    public void ExpectDisconnecting() =>
      Expect.That(Phase).Is(ConnectionPhase.Disconnecting);
  }
}