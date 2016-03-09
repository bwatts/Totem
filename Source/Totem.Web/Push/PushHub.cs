using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using Totem.Runtime;

namespace Totem.Web.Push
{
	/// <summary>
	/// Communicates with clients interacting via push
	/// </summary>
	[HubName("push")]
	public sealed class PushHub : Hub, ITaggable
	{
		public PushHub()
		{
			Tags = new Tags();
		}

		public Tags Tags { get; private set; }
		private ILog Log => Notion.Traits.Log.Get(this);

		public override Task OnConnected()
		{
			Log.Verbose("[push] Client connected: {ConnectionId:l}", Context.ConnectionId);

			return base.OnConnected();
		}

		public override Task OnReconnected()
		{
			Log.Verbose("[push] Client reconnected: {ConnectionId:l}", Context.ConnectionId);

			return base.OnReconnected();
		}

		public override Task OnDisconnected(bool stopCalled)
		{
			if(stopCalled)
			{
				Log.Verbose("[push] Client disconnected gracefully: {ConnectionId:l}", Context.ConnectionId);
			}
			else
			{
				Log.Verbose("[push] Client disconnected after timing out: {ConnectionId:l}", Context.ConnectionId);
			}

			return base.OnDisconnected(stopCalled);
		}
	}
}