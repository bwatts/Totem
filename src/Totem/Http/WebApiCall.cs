using System;
using System.Collections.Generic;
using System.Linq;
using Totem.Http;

namespace Totem.Http
{
	/// <summary>
	/// A call to a Totem web API
	/// </summary>
	public class WebApiCall
	{
		public WebApiCall(HttpLink link, HttpAuthorization authorization, WebApiCallBody body)
		{
			Link = link;
			Authorization = authorization;
			Body = body;
		}

		public readonly HttpLink Link;
		public readonly HttpAuthorization Authorization;
		public readonly WebApiCallBody Body;

		public override string ToString()
		{
			return Link.ToString();
		}
	}
}