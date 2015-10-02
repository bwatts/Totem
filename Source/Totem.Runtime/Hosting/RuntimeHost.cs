using System;
using System.Collections.Generic;
using System.Linq;
using Totem.Runtime.Hosting.Commands;

namespace Totem.Runtime.Hosting
{
	/// <summary>
	/// Hosts an instance of the Totem runtime
	/// </summary>
	public static class RuntimeHost
	{
		public static int Run<TProgram>(string[] args) where TProgram : IRuntimeProgram, new()
		{
			return HostCommandLine.From(args).Execute<TProgram>();
		}
	}
}