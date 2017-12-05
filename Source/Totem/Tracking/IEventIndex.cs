using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Totem.Runtime.Timeline;

namespace Totem.Tracking
{
	public interface IEventIndex
	{
		void Index(TrackedEvent e);
	}
}
