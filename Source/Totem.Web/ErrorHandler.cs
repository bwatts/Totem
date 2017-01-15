using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Nancy;
using Nancy.ViewEngines;
using Totem.IO;
using Totem.Runtime.Timeline;

namespace Totem.Web
{
	/// <summary>
	/// The handling of errors during requests to web APIs
	/// </summary>
	public sealed class ErrorHandler : IErrorHandler
	{
		readonly bool _detailed;

		public ErrorHandler(bool detailed)
		{
      _detailed = detailed;
		}

		public Response CreateResponse(NancyContext context, Exception error)
		{
      var code = GetCode(error);

      var response = new Response { StatusCode = code };

      if(_detailed)
      {
        response.ContentType = MediaType.Plain.ToTextUtf8();

        response.Contents = body =>
        {
          using(var writer = new StreamWriter(body))
          {
            writer.Write(error);
          }
        };
      }

      return response;
		}

    HttpStatusCode GetCode(Exception error)
		{
      if(error is FormatException)
      {
        return HttpStatusCode.BadRequest;
      }
      else if(error is UnauthorizedAccessException)
      {
        return HttpStatusCode.Unauthorized;
      }
      else if(error is RequestDeniedException)
      {
        return HttpStatusCode.Forbidden;
      }
      else if(error is ViewNotFoundException)
      {
        return HttpStatusCode.NotFound;
      }
      else if(error is ExpectException)
      {
        return HttpStatusCode.UnprocessableEntity;
      }
      else
      {
        if(error is AggregateException)
        {
          foreach(var innerError in (error as AggregateException).InnerExceptions)
          {
            var code = GetCode(innerError);

            if(code != HttpStatusCode.InternalServerError)
            {
              return code;
            }
          }
        }

        return HttpStatusCode.InternalServerError;
      }
		}
  }
}