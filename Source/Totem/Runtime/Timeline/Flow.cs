using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Totem.Runtime.Timeline
{
	/// <summary>
	/// A timeline presence observing and reacting to events
	/// </summary>
	[Durable]
	public abstract class Flow : Notion
	{
		[Transient] public FlowContext Context { get; internal set; }
		[Transient] public FlowCall Call { get; internal set; }
		[Transient] public Id Id { get; internal set; }

		public override Text ToText() => Context.ToText();

		protected internal virtual Task CallWhen(FlowCall.When call)
		{
      return call.MakeInternal(this);
		}

		protected void ThenDone()
		{
			Context.SetDone();
		}

		public new static class Traits
		{
      public static readonly Tag<Id> RequestId = Tag.Declare(() => RequestId);

			public static Id EnsureRequestId(Event e)
			{
				var requestId = RequestId.Get(e);

				if(!requestId.IsAssigned)
				{
					requestId = Id.FromGuid();

					RequestId.Set(e, requestId);
				}

				return requestId;
			}

			public static void ForwardRequestId(Event source, Event target)
			{
				RequestId.Set(target, RequestId.Get(source));
			}
		}
	}
}