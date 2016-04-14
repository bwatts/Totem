using System;
using System.Collections.Generic;
using System.Linq;
using Totem.IO;

namespace Totem.Runtime.Hosting.Commands
{
	/// <summary>
	/// A command to deploy the runtime to a target location
	/// </summary>
	public sealed class DeployCommand : HostCommand
	{
		private readonly IRuntimeBuild _build;
		private readonly IOLink _location;

		public DeployCommand(IRuntimeBuild build, IOLink location)
		{
			_build = build;
			_location = location;
		}

		protected override int ExecuteCommand<TProgram>()
		{
			_build.Deploy(_location);

			return 0;
		}
	}
}