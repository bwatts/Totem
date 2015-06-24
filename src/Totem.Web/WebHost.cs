using System;
using System.Collections.Generic;
using System.Linq;

namespace Totem.Web
{
	/// <summary>
	/// The context of a set of web applications
	/// </summary>
	public sealed class WebHost : Connection
	{
		private readonly IEnumerable<IWebApp> _apps;

		public WebHost(IEnumerable<IWebApp> apps)
		{
			_apps = apps;
		}

		protected override void Open()
		{
			Track(_apps.Select(app => app.Start()));
		}
	}
}