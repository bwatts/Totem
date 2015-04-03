using System;
using System.Collections.Generic;
using System.Linq;
using Totem.Http;
using Totem.Runtime;

namespace Totem.Web
{
	/// <summary>
	/// Settings for the Totem web server
	/// </summary>
	public class WebAreaView : View
	{
		public WebAreaView(string key) : base(key)
		{
			WebApiAppBindings = new List<HttpLink>();
			PushAppBindings = new List<HttpLink>();
		}

		public List<HttpLink> WebApiAppBindings { get; private set; }
		public List<HttpLink> PushAppBindings { get; private set; }
	}
}