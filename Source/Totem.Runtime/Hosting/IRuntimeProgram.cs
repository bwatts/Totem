using System;
using System.Collections.Generic;
using System.Linq;
using Topshelf.HostConfigurators;

namespace Totem.Runtime.Hosting
{
	/// <summary>
	/// Describes a configuration of the Totem runtime
	/// </summary>
	public interface IRuntimeProgram
	{
		void ConfigureHost(HostConfigurator host);
	}
}