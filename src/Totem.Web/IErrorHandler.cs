using System;
using System.Collections.Generic;
using System.Linq;
using Nancy;

namespace Totem.Web
{
	/// <summary>
	/// Describes the handling of errors during requests to web APIs
	/// </summary>
	public interface IErrorHandler
	{
		Response CreateResponse(NancyContext context, Exception exception);
	}
}