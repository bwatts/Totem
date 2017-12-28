using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace Totem.Runtime
{
	/// <summary>
	/// Indicates the observable state of a connection
	/// </summary>
	public sealed class ConnectionState : Notion
	{
		private CancellationTokenRegistration _cancellationTokenRegistration;
		private volatile ConnectionPhase _phase;
		private bool _hasConnected;

		public CancellationToken CancellationToken { get; private set; }
		public ConnectionPhase Phase => _phase;

		public bool IsDisconnected => _phase == ConnectionPhase.Disconnected;
		public bool IsConnecting => _phase == ConnectionPhase.Connecting;
		public bool IsConnected => _phase == ConnectionPhase.Connected;
		public bool IsCancelled => _phase == ConnectionPhase.Cancelled;
		public bool IsDisconnecting => _phase == ConnectionPhase.Disconnecting;
		public bool IsReconnecting => _phase == ConnectionPhase.Reconnecting;
		public bool IsReconnected => _phase == ConnectionPhase.Reconnected;

		//
		// Lifecycle
		//

		internal void OnConnecting(CancellationToken cancellationToken)
		{
			_phase = _hasConnected ? ConnectionPhase.Reconnecting : ConnectionPhase.Connecting;

			CancellationToken = cancellationToken;

			_cancellationTokenRegistration = cancellationToken.Register(OnCancelled);
		}

		internal void OnConnected()
		{
			if(_hasConnected)
			{
				_phase = ConnectionPhase.Reconnected;
			}
			else
			{
				_phase = ConnectionPhase.Connected;

				_hasConnected = true;
			}
		}

		internal void OnDisconnecting()
		{
			_phase = ConnectionPhase.Disconnecting;

			UnregisterCancellationToken();
		}

		internal void OnDisconnected()
		{
			_phase = ConnectionPhase.Disconnected;
		}

		private void OnCancelled()
		{
			_phase = ConnectionPhase.Cancelled;

			UnregisterCancellationToken();
		}

		private void UnregisterCancellationToken()
		{
      try
      {
        _cancellationTokenRegistration.Dispose();
      }
      catch(NullReferenceException)
      {
        // Every once in a while .NET throws an exception while disposing.
        //
        // We can't do anything about it, so ignore it.
      }

      _cancellationTokenRegistration = default(CancellationTokenRegistration);
		}

		//
		// Expectations
		//

		public void ExpectPhase(ConnectionPhase phase)
		{
			Expect(_phase).Is(phase, "Connection is in the wrong phase");
		}

		public void ExpectPhases(params ConnectionPhase[] phases)
		{
			Expect(phases.Contains(_phase), "Connection is in the wrong phase");
		}

		public void ExpectDisconnected()
		{
			ExpectPhase(ConnectionPhase.Disconnected);
		}

		public void ExpectConnecting()
		{
			ExpectPhase(ConnectionPhase.Connecting);
		}

		public void ExpectConnected()
		{
			ExpectPhase(ConnectionPhase.Connected);
		}

		public void ExpectDisconnecting()
		{
			ExpectPhase(ConnectionPhase.Disconnecting);
		}

		public void ExpectCancelled()
		{
			ExpectPhase(ConnectionPhase.Cancelled);
		}
	}
}