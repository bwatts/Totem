using System;
using System.Collections.Generic;
using System.Linq;
using Totem.Runtime.Timeline;

namespace Totem.Runtime.Map.Timeline
{
	/// <summary>
	/// A .NET type representing a flow on the timeline
	/// </summary>
	public class FlowType : DurableType
	{
		internal FlowType(RuntimeTypeRef type, FlowConstructor constructor, Many<RuntimeTypeKey> priorKeys) : base(type)
		{
			Constructor = constructor;
      PriorKeys = priorKeys;
			Events = new FlowEventSet();
			IsTopic = this is TopicType;
			IsView = this is ViewType;
			IsRequest = this is RequestType;
		}

		public readonly FlowConstructor Constructor;
    public readonly Many<RuntimeTypeKey> PriorKeys;
		public readonly FlowEventSet Events;
		public readonly bool IsTopic;
		public readonly bool IsView;
		public readonly bool IsRequest;

		public bool IsSingleInstance { get; private set; }
		public bool IsRouted { get; private set; }

		internal void SetSingleInstance()
		{
			IsSingleInstance = true;
			IsRouted = false;
		}

		internal void SetRouted()
		{
			IsSingleInstance = false;
			IsRouted = true;
		}

		public FlowKey CreateKey(Id id)
		{
			ExpectNot(IsSingleInstance && id.IsAssigned, Text.Of("Flow {0} is single-instance and cannot have an assigned id of {1}", this, id));
			ExpectNot(IsRouted && id.IsUnassigned, Text.Of("Flow {0} is routed and must have an assigned id", this));

			return FlowKey.From(this, id);
		}

		public Flow New() => Constructor.Call();
	}
}