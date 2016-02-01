using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace Totem
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
		public ConnectionPhase Phase { get { return _phase; } }

		public bool IsDisconnected { get { return _phase == ConnectionPhase.Disconnected; } }
		public bool IsConnecting { get { return _phase == ConnectionPhase.Connecting; } }
		public bool IsConnected { get { return _phase == ConnectionPhase.Connected; } }
		public bool IsCancelled { get { return _phase == ConnectionPhase.Cancelled; } }
		public bool IsDisconnecting { get { return _phase == ConnectionPhase.Disconnecting; } }
		public bool IsReconnecting { get { return _phase == ConnectionPhase.Reconnecting; } }
		public bool IsReconnected { get { return _phase == ConnectionPhase.Reconnected; } }

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
			_cancellationTokenRegistration.Dispose();
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