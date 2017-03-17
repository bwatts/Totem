using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Totem.Runtime.Timeline
{
	/// <summary>
	/// Describes an exchange routing view content to subscribers
	/// </summary>
	public interface IViewExchange
	{
		Task<ViewSubscription> Subscribe(Id connectionId, ViewETag etag);

    void Unsubscribe(Id connectionId);

    void Unsubscribe(Id connectionId, FlowKey key);

    void PushUpdate(View view);
	}
}