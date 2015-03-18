using System;
using System.Collections.Generic;
using System.Linq;
using Totem.Http;
using Totem.IO;
using Totem.Runtime.Map;

namespace Totem.Runtime
{
	/// <summary>
	/// A context-bound instance of the Totem runtime
	/// </summary>
	public sealed class RuntimeDeployment : IRuntimeDeployment
	{
		public RuntimeDeployment(RuntimeContext context)
		{
			Context = context;
		}

		public RuntimeContext Context { get; private set; }

		public override string ToString()
		{
			return Context.ToString();
		}

		public RuntimeMap ReadMap()
		{
			return RuntimeInitialization.ReadMap(this);
		}

		public void Deploy(IFolder outputFolder)
		{
			
		}

		public static string BuildType
		{
			get
			{
#if DEBUG
				return "Debug";
#else
				return "Release";
#endif
			}
		}
	}
}