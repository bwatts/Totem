using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Nancy;
using Nancy.ViewEngines;
using Totem.IO;

namespace Totem.Web
{
	/// <summary>
	/// The handling of errors during requests to web APIs
	/// </summary>
	public sealed class ErrorHandler : IErrorHandler
	{
		private readonly ErrorDetail _detail;

		public ErrorHandler(ErrorDetail detail)
		{
			_detail = detail;
		}

		public Response CreateResponse(NancyContext context, Exception error)
		{
			var codeAndText = GetCodeAndText(context, error);

			return new Response
			{
				StatusCode = codeAndText.Item1,
				ContentType = MediaType.Plain.ToTextUtf8(),
				Contents = body =>
				{
					using(var writer = new StreamWriter(body))
					{
						writer.Write(codeAndText.Item2);
					}
				}
			};
		}

		private Tuple<HttpStatusCode, Text> GetCodeAndText(NancyContext context, Exception error)
		{
			return GetKnownError(error) ?? GetError(HttpStatusCode.InternalServerError, error);
		}

		private Tuple<HttpStatusCode, Text> GetKnownError(Exception error)
		{
			if(error is FormatException)
			{
				return GetError(HttpStatusCode.BadRequest, error);
			}
			else if(error is UnauthorizedAccessException)
			{
				return GetError(HttpStatusCode.Unauthorized, error);
			}
			else if(error is ViewNotFoundException)
			{
				return GetError(HttpStatusCode.NotFound, error);
			}
			else if(error is AggregateException)
			{
				return (error as AggregateException).InnerExceptions.Select(GetKnownError).FirstOrDefault();
			}
			else
			{
				return null;
			}
		}

		private Tuple<HttpStatusCode, Text> GetError(HttpStatusCode statusCode, Exception error)
		{
			return Tuple.Create(statusCode, GetErrorText(error));
		}

		private Text GetErrorText(Exception error)
		{
			switch(_detail)
			{
				case ErrorDetail.None:
					return "An error occured (detail level prohibits further information)";
				case ErrorDetail.Type:
					return Text.Of("An error of type {0} occurred", error.GetType());
				case ErrorDetail.Message:
					return Text.Of("An error of type {0} occurred: {1}", error.GetType(), error.Message);
				case ErrorDetail.StackTrace:
					return Text.Of("An error of type {0} occurred: {1}", error.GetType(), error);
				default:
					throw new NotSupportedException("Unsupported detail level: " + Text.Of(_detail));
			}
		}
	}
}