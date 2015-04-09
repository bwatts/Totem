using System;
using System.Collections.Generic;
using System.Linq;

namespace Totem.Runtime
{
	/// <summary>
	/// Settings for the Dash setup UI
	/// </summary>
	public class SetupView : View
	{
		public SetupView(string key) : base(key)
		{
			Environments = new List<Environment>();
		}

		public readonly List<Environment> Environments;
		public string SuggestedNewEnvironmentName;

		public class Environment
		{
			public string Id;
			public string Name;
		}
	}
}