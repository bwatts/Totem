using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace Totem.Runtime
{
	/// <summary>
	/// A stateful connection to a resource
	/// </summary>
	public abstract class Connection : Notion, IConnectable
	{
		private readonly BlockingCollection<IDisposable> _trackedConnections = new BlockingCollection<IDisposable>();

		protected Connection()
		{
			State = new ConnectionState();
		}

		public ConnectionState State { get; private set; }

		//
		// Open
		//

		public IDisposable Connect(CancellationToken cancellationToken = default(CancellationToken))
		{
			State.OnConnecting(cancellationToken);

			TryOpen();

			return Disposal.Of(Disconnect);
		}

		public IDisposable Connect(IConnectable context)
		{
			return Connect(context.State.CancellationToken);
		}

		private void TryOpen()
		{
			Exception error = null;

			try
			{
				Open();

				State.OnConnected();
			}
			catch(Exception exception)
			{
				error = exception;

				State.OnDisconnected();
			}

			_trackedConnections.CompleteAdding();

			if(error != null)
			{
				CloseTrackedConnections();

				throw new Exception(Text.Of("Connection of type {0} failed to open", GetType()), error);
			}
		}

		protected virtual void Open()
		{}

		//
		// Close
		//

		protected void Disconnect()
		{
			State.OnDisconnecting();

			try
			{
				TryClose();
			}
			finally
			{
				State.OnDisconnected();
			}
		}

		private void TryClose()
		{
			var errors = new List<Exception>();

			try
			{
				CloseTrackedConnections();
			}
			catch(Exception error)
			{
				errors.Add(error);
			}

			try
			{
				Close();
			}
			catch(Exception error)
			{
				errors.Add(error);
			}

			if(errors.Any())
			{
				throw new AggregateException(Text.Of("Connection of type {0} failed to close", GetType()), errors).Flatten();
			}
		}

		protected virtual void Close()
		{}

		//
		// Tracking
		//

		protected void Track(IDisposable connection)
		{
			_trackedConnections.Add(connection);
		}

		protected void Track(IEnumerable<IDisposable> connections)
		{
			Track(Disposal.OfMany(connections));
		}

		protected void Track(IConnectable connection)
		{
			Track(connection.Connect(this));
		}

		protected void Track(IEnumerable<IConnectable> connections)
		{
			foreach(var connection in connections)
			{
				Track(connection);
			}
		}

		private void CloseTrackedConnections()
		{
			// Close in the opposite order as opened (the most dependent connections track last)

			var connections = _trackedConnections.GetConsumingEnumerable().Reverse().ToList();

			CloseTrackedConnections(connections);
		}

		private void CloseTrackedConnections(IEnumerable<IDisposable> connections)
		{
			var errors = new List<Exception>();

			foreach(var connection in connections)
			{
				try
				{
					connection.Dispose();
				}
				catch(Exception error)
				{
					errors.Add(error);
				}
			}

			if(errors.Any())
			{
				throw new AggregateException(Text.Of("Connection of type {0} failed to close", GetType()), errors);
			}
		}

		//
		// None
		//

		public static readonly IConnectable None = new NoneConnection();

		private sealed class NoneConnection : IConnectable
		{
			internal NoneConnection()
			{
				State = new ConnectionState();
			}

			public ConnectionState State { get; private set; }

			public IDisposable Connect(CancellationToken cancellationToken = default(CancellationToken))
			{
				return Disposal.None;
			}

			public IDisposable Connect(IConnectable context)
			{
				return Disposal.None;
			}
		}
	}
}