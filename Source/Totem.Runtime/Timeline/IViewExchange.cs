using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Linq;

namespace Totem.Runtime.Timeline
{
	/// <summary>
	/// Describes an exchange routing view diffs to subscribers
	/// </summary>
	public interface IViewExchange
	{
		ViewSubscription Subscribe(Id connectionId, ViewETag etag);

		void Unsubscribe(Id connectionId);

		void Unsubscribe(Id connectionId, FlowKey key);

		void PushUpdate(View view);
	}
}