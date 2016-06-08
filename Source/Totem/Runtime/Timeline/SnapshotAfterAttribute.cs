using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Totem.Runtime.Timeline
{
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = true)]
	public class SnapshotAfterAttribute : Attribute, ISnapshotCondition
	{
		private readonly Type _type;

		public SnapshotAfterAttribute(Type type)
		{
			Expect.True(type).IsAssignableTo(typeof(Event));
			_type = type;
		}

		public bool ShouldSnapshot(TimelinePosition last, TimelinePoint current)
		{
			return current.EventType.Is(_type);
		}
	}
}
