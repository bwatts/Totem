using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Totem.Runtime.Timeline
{
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
	public class SnapshotAlwaysAttribute : Attribute, ISnapshotCondition
	{
		public bool ShouldSnapshot(TimelinePosition last, TimelinePoint current)
		{
			return true;
		}
	}
}
