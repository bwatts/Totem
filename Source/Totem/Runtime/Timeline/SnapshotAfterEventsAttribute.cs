using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Totem.Runtime.Timeline
{
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
	public class SnapshotAfterEventsAttribute : Attribute, ISnapshotCondition
	{
		private readonly int _count;

		public SnapshotAfterEventsAttribute(int count)
		{
      _count = count;
		}

		public bool ShouldSnapshot(TimelinePosition last, TimelinePoint current)
		{
			return current.Position.ToInt64() - last.ToInt64() >= _count;
		}
	}
}
