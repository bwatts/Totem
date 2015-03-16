using System;
using System.Collections.Generic;
using System.Linq;
using Nancy;
using Totem.Http;

namespace Totem.Web
{
	/// <summary>
	/// The scope in which an instance of <see cref="WebApi"/> handles a request
	/// </summary>
	public class WebApiScope : Notion
	{
		public WebApiScope(NancyContext context, IRequestBody requestBody)
		{
			Link = HttpLink.From(context.Request.Url.ToString());
			Authorization = HttpAuthorization.From(context.Request.Headers.Authorization);
			RequestBody = requestBody;
		}

		public HttpLink Link { get; private set; }
		public HttpAuthorization Authorization { get; private set; }
		public IRequestBody RequestBody { get; private set; }

		public override Text ToText()
		{
			return Link.ToText();
		}
	}
}