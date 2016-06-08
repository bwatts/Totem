using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Totem.Runtime.Timeline
{
	public interface ISnapshotCondition
	{
		bool ShouldSnapshot(TimelinePosition last, TimelinePoint current);
	}
}
