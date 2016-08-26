using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Topshelf;

namespace Totem.Runtime.Hosting.Commands
{
	/// <summary>
	/// Passes the current command through to Topshelf in this application domain
	/// </summary>
	public sealed class RunCommandBridge : MarshalByRefObject
	{
		public override object InitializeLifetimeService()
		{
			// Null tells .NET Remoting that this object has an infinite lease. See Modifying Lease Properties:
			//
			// http://msdn.microsoft.com/en-us/library/23bk23zc%28v=vs.71%29.aspx

			return null;
		}

		internal bool Restarted { get; private set; }

		public int? Execute<TProgram>(TextWriter consoleOut) where TProgram : IRuntimeProgram, new()
		{
			Console.SetOut(consoleOut);

			var command = new RuntimeCommand();

			var result = command.Execute<TProgram>();

			return command.Restarted ? (int?) null : result;
		}

		private sealed class RuntimeCommand : HostCommand
		{
			internal bool Restarted { get; private set; }

			protected override int ExecuteCommand<TProgram>()
			{
				var service = new RuntimeService(typeof(TProgram).Assembly);

				var result = (int) HostFactory.Run(host =>
				{
					host.UseSerilog(SerilogAdapter.Logger);

					host.Service(() => service);

					new TProgram().ConfigureHost(host);
				});

				Restarted = service.Restarted;

				return result;
			}
		}
	}
}