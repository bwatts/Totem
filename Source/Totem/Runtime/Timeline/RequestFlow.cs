using System;
using System.Collections.Generic;
using System.Linq;

namespace Totem.Runtime.Timeline
{
	/// <summary>
	/// An process observing and publishing to the timeline in order to respond to a pending request
	/// </summary>
	/// <remarks>
	/// I generally choose attributes over marker classes. However, this allows a type constraint on
	/// the ITimeline.Run method, preventing an easy mistake and gently correcting the author at compile-time.
	/// </remarks>
	public abstract class RequestFlow : Flow
	{

	}
}