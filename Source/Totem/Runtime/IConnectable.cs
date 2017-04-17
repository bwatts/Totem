using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace Totem.Runtime
{
	/// <summary>
	/// Describes a stateful connection to a resource
	/// </summary>
	public interface IConnectable : IClean
	{
		ConnectionState State { get; }

		IDisposable Connect(CancellationToken cancellationToken = default(CancellationToken));

		IDisposable Connect(IConnectable context);
	}
}