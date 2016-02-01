using System;
using System.Collections.Generic;
using System.Linq;

namespace Totem.Runtime
{
	/// <summary>
	/// The current phase in the lifecycle of a connection
	/// </summary>
	public enum ConnectionPhase
	{
		Disconnected,
		Connecting,
		Connected,
		Cancelled,
		Disconnecting,
		Reconnecting,
		Reconnected
	}
}