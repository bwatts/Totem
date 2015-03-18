using System;
using System.Collections.Generic;
using System.Linq;
using Totem.Runtime.Configuration;

namespace Totem.Runtime
{
	/// <summary>
	/// A deployment of the current runtime host to a new location
	/// </summary>
	internal sealed class RuntimeHostDeployment : Notion
	{
		private readonly RuntimeSection _settings;
		private readonly IReadOnlyList<string> _args;

		internal RuntimeHostDeployment(RuntimeSection settings, IReadOnlyList<string> args)
		{
			_settings = settings;
			_args = args;
		}

		internal int Run()
		{
			return 0;
		}
	}
}