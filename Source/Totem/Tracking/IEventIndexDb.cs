using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Totem.Tracking
{
	public interface IEventIndexDb
	{
		Task PushWrites(Action<IEventIndex> writes);
	}
}
