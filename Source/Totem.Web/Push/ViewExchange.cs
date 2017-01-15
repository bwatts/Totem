using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading.Tasks;
using Totem.Runtime.Json;
using Totem.Runtime.Timeline;

namespace Totem.Web.Push
{
	/// <summary>
	/// An exchange routing updates to view subscriptions
	/// </summary>
	public sealed class ViewExchange : Notion, IViewExchange
	{
		readonly Dictionary<Id, ViewConnection> _connectionsById = new Dictionary<Id, ViewConnection>();
		readonly Dictionary<FlowKey, SubscribedView> _viewsByKey = new Dictionary<FlowKey, SubscribedView>();
		readonly IViewDb _viewDb;
		readonly IPushChannel _push;
		readonly TimeSpan _updateThrottle;

		public ViewExchange(IViewDb viewDb, IPushChannel push, TimeSpan updateThrottle)
		{
			_viewDb = viewDb;
			_push = push;
			_updateThrottle = updateThrottle;
		}

		public async Task<ViewSubscription> Subscribe(Id connectionId, ViewETag etag)
		{
			lock(_connectionsById)
			{
				SubscribeView(connectionId, etag);
			}

			return await GetSubscribeResult(connectionId, etag);
		}

		public void Unsubscribe(Id connectionId)
		{
			lock(_connectionsById)
			{
				GetConnectionOrNull(connectionId)?.Unsubscribe();
			}
		}

		public void Unsubscribe(Id connectionId, FlowKey key)
		{
			lock(_connectionsById)
			{
				GetConnectionOrNull(connectionId)?.Unsubscribe(key);
			}
		}

		public void PushUpdate(View view)
		{
			SubscribedView subscribedView;

			lock(_connectionsById)
			{
				subscribedView = GetViewOrNull(view.Context.Key);
			}

			subscribedView?.PushUpdate(view);
		}

		void SubscribeView(Id connectionId, ViewETag etag)
		{
			var connection = GetConnectionOrNull(connectionId);

			if(connection == null)
			{
				connection = new ViewConnection(connectionId, this);

				_connectionsById[connectionId] = connection;
			}

			var view = GetViewOrNull(etag.Key);

			if(view == null)
			{
				view = new SubscribedView(etag.Key, this);

				_viewsByKey[etag.Key] = view;
			}

			connection.Subscribe(view);
		}

		async Task<ViewSubscription> GetSubscribeResult(Id connectionId, ViewETag etag)
		{
			var snapshot = await _viewDb.ReadSnapshot(etag.Key, etag.Checkpoint);

			if(snapshot.NotFound || snapshot.NotModified)
			{
				return new ViewSubscription(etag);
			}

			etag = ViewETag.From(snapshot.Key, snapshot.Checkpoint);

			var diff = JsonFormat.Text.SerializeJson(snapshot.ReadContent());

			return new ViewSubscription(etag, diff);
		}

		ViewConnection GetConnectionOrNull(Id id)
		{
			ViewConnection connection;

			_connectionsById.TryGetValue(id, out connection);

			return connection;
		}

		SubscribedView GetViewOrNull(FlowKey key)
		{
			SubscribedView view;

			_viewsByKey.TryGetValue(key, out view);

			return view;
		}

		void RemoveConnection(ViewConnection connection)
		{
			_connectionsById.Remove(connection.Id);
		}

		void RemoveView(SubscribedView view)
		{
			_viewsByKey.Remove(view.Key);
		}

		sealed class ViewConnection
		{
			readonly Dictionary<FlowKey, SubscribedView> _viewsByKey = new Dictionary<FlowKey, SubscribedView>();
			readonly ViewExchange _exchange;

			internal ViewConnection(Id id, ViewExchange exchange)
			{
				Id = id;
				_exchange = exchange;
			}

			internal readonly Id Id;

			internal void Subscribe(SubscribedView view)
			{
				if(!_viewsByKey.ContainsKey(view.Key))
				{
					_viewsByKey[view.Key] = view;

					view.Subscribe(this);
				}
			}

			internal void Unsubscribe()
			{
				foreach(var view in _viewsByKey.Values)
				{
					view.Unsubscribe(this);
				}

				_exchange.RemoveConnection(this);
			}

			internal void Unsubscribe(FlowKey key)
			{
				SubscribedView view;

				if(_viewsByKey.TryGetValue(key, out view))
				{
					view.Unsubscribe(this);

					_viewsByKey.Remove(key);

					if(_viewsByKey.Count == 0)
					{
						_exchange.RemoveConnection(this);
					}
				}
			}
		}

		sealed class SubscribedView
		{
			readonly Many<Id> _connectionIds = new Many<Id>();
			readonly Subject<ViewUpdated> _updates = new Subject<ViewUpdated>();
			readonly ViewExchange _exchange;
			readonly IDisposable _updateSubscription;

			internal SubscribedView(FlowKey key, ViewExchange exchange)
			{
				Key = key;
				_exchange = exchange;

				_updateSubscription = _updates
					.Throttle(_exchange._updateThrottle)
					.ObserveOn(ThreadPoolScheduler.Instance)
					.Subscribe(WhenUpdated);
			}

			internal readonly FlowKey Key;

			internal void Subscribe(ViewConnection connection)
			{
				_connectionIds.Write.Add(connection.Id);
			}

			internal void Unsubscribe(ViewConnection connection)
			{
				_connectionIds.Write.Remove(connection.Id);

				if(_connectionIds.Count == 0)
				{
					_updateSubscription.Dispose();

					_exchange.RemoveView(this);
				}
			}

			internal void PushUpdate(View view)
			{
				var etag = ViewETag.From(view.Context.Key, view.Context.CheckpointPosition);
				var diff = JsonFormat.Text.SerializeJson(view);

				_updates.OnNext(new ViewUpdated(view.Context.Key.ToString(), etag.ToString(), diff));
			}

			void WhenUpdated(ViewUpdated e)
			{
				// This returns a task, but does not need to be awaited unless we use a SQL backplane: http://stackoverflow.com/a/19193493/37815

				_exchange._push.PushToClients(e, _connectionIds);
			}
		}
	}
}