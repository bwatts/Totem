using System;
using System.Collections.Generic;
using System.Linq;
using Owin;
using Totem.Http;

namespace Totem.Web
{
	/// <summary>
	/// Describes an HTTP-bound application composed by OWIN
	/// </summary>
	public interface IWebApp
	{
		IReadOnlyList<HttpLink> HostBindings { get; }

		void Start(IAppBuilder builder);
	}
}