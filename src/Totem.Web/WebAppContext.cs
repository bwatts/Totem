using System;
using System.Collections.Generic;
using System.Linq;
using Autofac;
using Totem.Http;
using Totem.IO;
using Totem.Runtime.Map;

namespace Totem.Web
{
	/// <summary>
	/// The context established for an HTTP-bound application
	/// </summary>
	public class WebAppContext
	{
		public WebAppContext(Many<WebApiType> apiTypes, Many<HttpLink> bindings, FolderLink uiFolder, ILifetimeScope scope)
		{
			ApiTypes = apiTypes;
			Bindings = bindings;
			UIFolder = uiFolder;
			Scope = scope;
		}

		public readonly Many<WebApiType> ApiTypes;
		public readonly Many<HttpLink> Bindings;
		public readonly FolderLink UIFolder;
		public readonly ILifetimeScope Scope;
	}
}