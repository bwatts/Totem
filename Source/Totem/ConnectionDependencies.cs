using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace Totem
{
	/// <summary>
	/// Extends sequences of connections with the ability to connect them in order of dependencies between them
	/// </summary>
	public static class ConnectionDependencies
	{
		public static IDisposable Connect<T>(this IEnumerable<T> source, Func<T, IEnumerable<T>> selectDependencies, CancellationToken cancellationToken = default(CancellationToken))
			where T : IConnectable
		{
			return new ConnectContext<T>(source, selectDependencies, cancellationToken).Connect();
		}

		public static IDisposable Connect<T>(this IEnumerable<T> source, Func<T, IEnumerable<T>> selectDependencies, IConnectable context)
			where T : IConnectable
		{
			return source.Connect(selectDependencies, context.State.CancellationToken);
		}

		private sealed class ConnectContext<T> where T : IConnectable
		{
			private readonly Dictionary<T, ConnectionNode> _nodesByItem = new Dictionary<T, ConnectionNode>();
			private readonly Func<T, IEnumerable<T>> _selectDependencies;
			private readonly CancellationToken _cancellationToken;
			private readonly HashSet<T> _addContext;

			internal ConnectContext(IEnumerable<T> source, Func<T, IEnumerable<T>> selectDependencies, CancellationToken cancellationToken)
			{
				_selectDependencies = selectDependencies;
				_cancellationToken = cancellationToken;

				_addContext = new HashSet<T>();

				foreach(var item in source)
				{
					GetOrAddNode(item);
				}

				_addContext = null;
			}

			//
			// Nodes
			//

			private ConnectionNode GetOrAddNode(T item)
			{
				ConnectionNode node;

				if(!_nodesByItem.TryGetValue(item, out node))
				{
					_addContext.Add(item);

					node = new ConnectionNode(item, GetOrAddDependencies(item), _cancellationToken);

					_nodesByItem[item] = node;

					_addContext.Remove(item);
				}

				return node;
			}

			private IEnumerable<ConnectionNode> GetOrAddDependencies(T item)
			{
				foreach(var dependency in _selectDependencies(item))
				{
          Expect.False(_addContext.Contains(dependency), Text
            .Of("There is a cycle in the connection graph")
            .WriteTwoLines()
            .Write("item: ")
            .WriteLine(item)
            .Write("dependency: ")
            .Write(dependency));

					yield return GetOrAddNode(dependency);
				}
			}

			private IEnumerable<ConnectionNode> Nodes { get { return _nodesByItem.Values; } }

			//
			// Connection
			//

			internal IDisposable Connect()
			{
				ConnectNodes();

				CheckFailure();

				return GetDisposal();
			}

			private void ConnectNodes()
			{
				foreach(var node in Nodes)
				{
					node.EnsureConnected();
				}
			}

			private void CheckFailure()
			{
				if(NodeHasError)
				{
					DisconnectNodes();

					ThrowNodeErrors();
				}
			}

			private void DisconnectNodes()
			{
				foreach(var node in Nodes)
				{
					node.Disconnect();
				}
			}

			private void ThrowNodeErrors()
			{
				throw new AggregateException("One or more nodes failed to connect", NodeErrors).Flatten();
			}

			private bool NodeHasError
			{
				get { return Nodes.Where(node => node.Exception != null).Any(); }
			}

			private IEnumerable<Exception> NodeErrors
			{
				get { return Nodes.Select(node => node.Exception).Where(exception => exception != null); }
			}

			private IDisposable GetDisposal()
			{
				return Disposal.OfMany(Nodes.Select(node => node.Connection).Reverse());
			}
		}

		private sealed class ConnectionNode : Notion
		{
			private readonly IConnectable _item;
			private readonly List<ConnectionNode> _dependencies;
			private readonly CancellationToken _cancellationToken;
			private bool _disconnected = true;

			internal ConnectionNode(IConnectable item, IEnumerable<ConnectionNode> dependencies, CancellationToken cancellationToken)
			{
				_item = item;
				_dependencies = dependencies.ToList();
				_cancellationToken = cancellationToken;
			}

			internal IDisposable Connection { get; private set; }
			internal Exception Exception { get; private set; }

			public override Text ToText()
			{
				return _item.ToString();
			}

			internal void EnsureConnected()
			{
				if(_disconnected)
				{
					_disconnected = false;

					Connect();
				}
			}

			internal void Disconnect()
			{
				if(Connection != null)
				{
					try
					{
						Connection.Dispose();
					}
					catch(Exception exception)
					{
						Exception = Exception == null ? exception : new AggregateException(Exception, exception).Flatten();
					}

					_disconnected = true;
				}
			}

			private void Connect()
			{
				try
				{
					Connection = ConnectWithDependencies();
				}
				catch(Exception exception)
				{
					Exception = exception;
				}
			}

			private IDisposable ConnectWithDependencies()
			{
				foreach(var dependency in _dependencies)
				{
					dependency.EnsureConnected();
				}

				return _item.Connect(_cancellationToken);
			}
		}
	}
}