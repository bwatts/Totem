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
	public sealed class PushHub : Hub, ITaggable
	{
		private readonly IViewExchange _viewExchange;

		public PushHub(IViewExchange viewExchange)
		{
			_viewExchange = viewExchange;

			Tags = new Tags();
		}

		public Tags Tags { get; private set; }
		private ILog Log => Notion.Traits.Log.Get(this);

		private string CompactConnectionId() => Text.Of(Context.ConnectionId).CompactRight(8, "...");

		//
		// Connection
		//

		public override Task OnConnected()
		{
			Log.Verbose("[push] [{ConnectionId:l}] Connected", CompactConnectionId());

			return base.OnConnected();
		}

		public override Task OnReconnected()
		{
			Log.Verbose("[push] [{ConnectionId:l}] Reconnected", CompactConnectionId());

			return base.OnReconnected();
		}

		public override Task OnDisconnected(bool stopCalled)
		{
			UnsubscribeViews();

			if(stopCalled)
			{
				Log.Verbose("[push] [{ConnectionId:l}] Disconnected", CompactConnectionId());
			}
			else
			{
				Log.Verbose("[push] [{ConnectionId:l}] Timed out", CompactConnectionId());
			}

			return base.OnDisconnected(stopCalled);
		}

		//
		// Views
		//

		public string SubscribeView(string etag)
		{
			try
			{
				var result = _viewExchange.Subscribe(Id.From(Context.ConnectionId), ViewETag.From(etag));

				var resultJson = JsonFormat.Text.Serialize(new
				{
					key = result.ETag.Key.ToString(),
					etag = result.ETag.ToString(),
					diff = result.Diff
				});

				Log.Verbose("[push] [{ConnectionId:l}] Subscribed to {ETag:l}", CompactConnectionId(), result.ETag);

				return resultJson;
			}
			catch(Exception error)
			{
				Log.Error(error, "[push] [{ConnectionId:l}] Subscribe to {ETag:l} failed", CompactConnectionId(), etag);

				throw;
			}
		}

		public void UnsubscribeView(string key)
		{
			try
			{
				_viewExchange.Unsubscribe(Id.From(Context.ConnectionId), FlowKey.From(key));

				Log.Verbose("[push] [{ConnectionId:l}] Unsubscribed from {Key:l}", CompactConnectionId(), key);
			}
			catch(Exception error)
			{
				Log.Error(error, "[push] [{ConnectionId:l}] Unsubscribe from {Key:l} failed", CompactConnectionId(), key);

				throw;
			}
		}

		private void UnsubscribeViews()
		{
			try
			{
				_viewExchange.Unsubscribe(Id.From(Context.ConnectionId));

				Log.Verbose("[push] [{ConnectionId:l}] Unsubscribed from all views", CompactConnectionId());
			}
			catch(Exception error)
			{
				Log.Error(error, "[push] [{ConnectionId:l}] Unsubscribe from all views failed", CompactConnectionId());

				throw;
			}
		}
	}
}