using System;
using System.Collections.Generic;
using System.Linq;

namespace Totem.Runtime.Hosting.Commands
{
	/// <summary>
	/// A command that the host chose not to execute
	/// </summary>
	public sealed class IssueCommand : HostCommand
	{
		private readonly Exception _error;

		public IssueCommand(Exception error)
		{
			_error = error;
		}

		protected override int ExecuteCommand<TProgram>()
		{
			if(Log != null)
			{
				Log.Error(_error, "[runtime] Issue while executing program {Program}", typeof(TProgram));
			}

			return -1;
		}
	}
}