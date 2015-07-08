using System;
using System.Collections.Generic;
using System.Linq;
using Autofac;
using Totem.Http;
using Totem.IO;

namespace Totem.Web
{
	/// <summary>
	/// The context established for an HTTP-bound API serving a user interface
	/// </summary>
	public class WebUIContext : WebAppContext
	{
		public WebUIContext(Many<HttpLink> bindings, ILifetimeScope scope, bool enableCors, FolderLink contentFolder) : base(bindings, scope, enableCors)
		{
			ContentFolder = contentFolder;
		}

		public readonly FolderLink ContentFolder;
	}
}