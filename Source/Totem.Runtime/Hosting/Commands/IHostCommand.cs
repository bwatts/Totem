using System;
using System.Collections.Generic;
using System.Linq;

namespace Totem.Runtime.Hosting.Commands
{
	/// <summary>
	/// Describes a command executed by the runtime host
	/// </summary>
	public interface IHostCommand
	{
		int Execute<TProgram>() where TProgram : IRuntimeProgram, new();
	}
}