using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Nancy;
using Nancy.Responses;
using Nancy.ViewEngines;
using Totem.IO;

namespace Totem.Web
{
	/// <summary>
	/// The handling of errors during requests to web APIs
	/// </summary>
	public sealed class ErrorHandler : IErrorHandler
	{
		public Response CreateResponse(NancyContext context, Exception exception)
		{
			var codeAndText = GetCodeAndText(context, exception);

			return new Response
			{
				StatusCode = codeAndText.Item1,
				ContentType = MediaType.Plain.ToString(),
				Contents = body => new StreamWriter(body).Write(codeAndText.Item2)
			};
		}

		private Tuple<HttpStatusCode, Text> GetCodeAndText(NancyContext context, Exception exception)
		{
			return GetKnownError(exception) ?? Tuple.Create(HttpStatusCode.InternalServerError, Text.Of(exception));
		}

		// TODO: Eventually we will not want to send stack traces to production clients - this is the place to change that

		private Tuple<HttpStatusCode, Text> GetKnownError(Exception exception)
		{
			if(exception is FormatException)
			{
				return Tuple.Create(HttpStatusCode.BadRequest, Text.Of(exception));
			}
			else if(exception is ViewNotFoundException)
			{
				return Tuple.Create(HttpStatusCode.NotFound, Text.Of(exception));
			}
			else if(exception is AggregateException)
			{
				return (exception as AggregateException).InnerExceptions.Select(GetKnownError).FirstOrDefault();
			}
			else if(exception is UnauthorizedAccessException)
			{
				return Tuple.Create(HttpStatusCode.Unauthorized, Text.Of(exception));
			}
			{
				return null;
			}
		}
	}
}