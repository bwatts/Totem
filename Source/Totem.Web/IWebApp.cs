using System;
using System.Collections.Generic;
using System.Linq;

namespace Totem.Web
{
	/// <summary>
	/// Describes a web application hosted by a Totem runtime
	/// </summary>
	public interface IWebApp
	{
		IDisposable Start();
	}
}