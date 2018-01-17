using System;
using System.Threading.Tasks;

namespace Totem.Tracking
{
  /// <summary>
  /// Describes a database of writes to an event index
  /// </summary>
	public interface IEventIndexDb
	{
		Task PushWrites(Action<IEventIndex> writes);
	}
}