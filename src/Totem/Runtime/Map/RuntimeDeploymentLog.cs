using System;
using System.Collections.Generic;
using System.Linq;
using Totem.IO;

namespace Totem.Runtime.Map
{
	/// <summary>
	/// The log associated with the current deployment of the Totem runtime
	/// </summary>
	public sealed class RuntimeDeploymentLog
	{
		public RuntimeDeploymentLog(LogLevel level, IFolder folder, string serverUrl)
		{
			Level = level;
			Folder = folder;
			ServerUrl = serverUrl;
		}

		public LogLevel Level { get; private set; }
		public IFolder Folder { get; private set; }
		public string ServerUrl { get; private set; }
	}
}