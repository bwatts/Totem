using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration.Install;
using System.Linq;
using Totem.Runtime.Configuration;

namespace Totem.Runtime
{
	/// <summary>
	/// Installs a service which hosts the Totem runtime
	/// </summary>
	[RunInstaller(true)]
	public sealed class RuntimeHostServiceInstaller : Installer
	{
		public RuntimeHostServiceInstaller()
		{
			Installers.AddRange(RuntimeSection.Read().Service.ReadInstallers().ToArray());
		}
	}
}