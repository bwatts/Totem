using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Nancy;
using Totem.IO;
using Totem.Runtime.Timeline;

namespace Totem.Web
{
	/// <summary>
	/// An process observing and publishing to the timeline in order to make an API call
	/// </summary>
	public abstract class WebApiFlow : Flow
	{
		private Response _response;

		public Response ToResponse()
		{
			Expect(_response).IsNotNull("Web API flow has not responded");

			return _response;
		}

		protected void Respond(Response response)
		{
			Expect(_response).IsNull("Web API flow has already responded");

			_response = response;

			Done();
		}

		protected void RespondOK(string reasonPhrase)
		{
			Respond(new Response { StatusCode = HttpStatusCode.OK, ReasonPhrase = reasonPhrase });
		}

		protected void RespondError(string reasonPhrase)
		{
			Respond(new Response { StatusCode = HttpStatusCode.InternalServerError, ReasonPhrase = reasonPhrase });
		}

		protected void RespondError(string reasonPhrase, string error)
		{
			Respond(new Response
			{
				StatusCode = HttpStatusCode.InternalServerError,
				ReasonPhrase = reasonPhrase,
				ContentType = MediaType.Plain.ToTextUtf8(),
				Contents = content => new StreamWriter(content).Write(error)
			});
		}

		protected void RespondBadRequest(string reasonPhrase)
		{
			Respond(new Response { StatusCode = HttpStatusCode.BadRequest, ReasonPhrase = reasonPhrase });
		}
	}
}