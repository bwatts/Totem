using System;
using System.Collections.Generic;
using System.Linq;
using Topshelf.HostConfigurators;

namespace Totem.Runtime.Hosting
{
	/// <summary>
	/// Describes the hosting of an instance of the Totem runtime
	/// </summary>
	public interface IRuntimeHost
	{
		void Configure(HostConfigurator host);
	}
}