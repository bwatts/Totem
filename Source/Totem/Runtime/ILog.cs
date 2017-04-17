using System;
using System.Collections.Generic;
using System.Linq;

namespace Totem.Runtime
{
	/// <summary>
	/// Describes a log capturing execution details of the runtime
	/// </summary>
	public interface ILog : IClean
	{
		LogLevel Level { get; }

		void Write(LogEvent e);
	}
}