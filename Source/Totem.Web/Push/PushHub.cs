using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using Totem.Runtime;
using Totem.Runtime.Json;
using Totem.Runtime.Timeline;

namespace Totem.Web.Push
{
	/// <summary>
	/// Communicates with clients interacting via push
	/// </summary>
	[HubName("push")]
	public sealed class PushHub : Hub, IBindable
	{
    readonly Lazy<Id> _connectionId;
    readonly Lazy<string> _compactConnectionId;
		readonly IViewExchange _viewExchange;

		public PushHub(IViewExchange viewExchange)
		{
			_viewExchange = viewExchange;

      _connectionId = new Lazy<Id>(() => Id.From(Context.ConnectionId));
      _compactConnectionId = new Lazy<string>(() => ConnectionId.ToText().CompactRight(8, "..."));
		}

    public Fields Fields { get; } = new Fields();
		ILog Log => Notion.Traits.Log.Get(this);

    Id ConnectionId => _connectionId.Value;
    string CompactConnectionId => _compactConnectionId.Value;

    //
    // Connection
    //

    public override Task OnConnected()
		{
			Log.Verbose("[push] [{ConnectionId:l}] Connected", CompactConnectionId);

			return base.OnConnected();
		}

		public override Task OnReconnected()
		{
			Log.Verbose("[push] [{ConnectionId:l}] Reconnected", CompactConnectionId);

			return base.OnReconnected();
		}

		public override Task OnDisconnected(bool stopCalled)
		{
			UnsubscribeViews();

			if(stopCalled)
			{
				Log.Verbose("[push] [{ConnectionId:l}] Disconnected", CompactConnectionId);
			}
			else
			{
				Log.Verbose("[push] [{ConnectionId:l}] Timed out", CompactConnectionId);
			}

			return base.OnDisconnected(stopCalled);
		}

		//
		// Views
		//

		public async Task<string> SubscribeView(string etag)
		{
			try
			{
				var result = await _viewExchange.Subscribe(ConnectionId, ViewETag.From(etag));

				var resultJson = JsonFormat.Text.Serialize(new
				{
					key = result.ETag.Key.ToString(),
					etag = result.ETag.ToString(),
					content = result.Content
				});

				Log.Verbose("[push] [{ConnectionId:l}] Subscribed to {ETag:l}", CompactConnectionId, result.ETag);

				return resultJson;
			}
			catch(Exception error)
			{
				Log.Error(error, "[push] [{ConnectionId:l}] Subscribe to {ETag:l} failed", CompactConnectionId, etag);

				throw;
			}
		}

		public void UnsubscribeView(string key)
		{
			try
			{
				_viewExchange.Unsubscribe(ConnectionId, FlowKey.From(key));

				Log.Verbose("[push] [{ConnectionId:l}] Unsubscribed from {Key:l}", CompactConnectionId, key);
			}
			catch(Exception error)
			{
				Log.Error(error, "[push] [{ConnectionId:l}] Unsubscribe from {Key:l} failed", CompactConnectionId, key);

				throw;
			}
		}

		void UnsubscribeViews()
		{
			try
			{
				_viewExchange.Unsubscribe(ConnectionId);

				Log.Verbose("[push] [{ConnectionId:l}] Unsubscribed from all views", CompactConnectionId);
			}
			catch(Exception error)
			{
				Log.Error(error, "[push] [{ConnectionId:l}] Unsubscribe from all views failed", CompactConnectionId);

				throw;
			}
		}
	}
}