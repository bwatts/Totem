using System;
using System.Collections.Generic;
using System.Linq;
using Topshelf;

namespace Totem.Runtime.Hosting.Commands
{
	/// <summary>
	/// A command to Topshelf in the current application domain
	/// </summary>
	public class PassThroughCommand : HostCommand
	{
		protected override int ExecuteCommand<TProgram>()
		{
			return (int) HostFactory.Run(host =>
			{
				host.UseSerilog(SerilogAdapter.Logger);

				host.Service(() => new RuntimeService(typeof(TProgram).Assembly));

				new TProgram().ConfigureHost(host);
			});
		}
	}
}