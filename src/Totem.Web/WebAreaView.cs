using System;
using System.Collections.Generic;
using System.Linq;
using Totem.Http;

namespace Totem.Web
{
	/// <summary>
	/// Settings for the web area of the runtime
	/// </summary>
	public class WebAreaView : View
	{
		public WebAreaView(string key) : base(key)
		{}

		public ErrorDetail ErrorDetail;
	}
}