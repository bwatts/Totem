using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using Totem.Http;

namespace Totem.Web.Configuration
{
	/// <summary>
	/// Configures Totem web APIs
	/// </summary>
	public class WebSection : ConfigurationSection
	{
		[ConfigurationProperty("hostBinding", IsRequired = true)]
		public HttpLink HostBinding
		{
			get { return (HttpLink) this["hostBinding"]; }
			set { this["hostBinding"] = value; }
		}

		[ConfigurationProperty("apiRoot", IsRequired = true)]
		public HttpResource ApiRoot
		{
			get { return (HttpResource) this["apiRoot"]; }
			set { this["apiRoot"] = value; }
		}

		[ConfigurationProperty("pushRoot", IsRequired = true)]
		public HttpResource PushRoot
		{
			get { return (HttpResource) this["pushRoot"]; }
			set { this["pushRoot"] = value; }
		}
	}
}