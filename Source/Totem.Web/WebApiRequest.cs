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
	/// An process observing and publishing to the timeline in order to make a web request
	/// </summary>
	public abstract class WebApiRequest : Runtime.Timeline.Request
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

			ThenDone();
		}

		protected void RespondOK(string reason)
		{
			Respond(new Response { StatusCode = HttpStatusCode.OK, ReasonPhrase = reason });
		}

		protected void RespondError(string reason)
		{
			Respond(new Response { StatusCode = HttpStatusCode.InternalServerError, ReasonPhrase = reason });
		}

		protected void RespondError(string reason, string error)
		{
			Log.Error("[web] Internal server error: {Reason:l} {Error}", reason, error);

			Respond(new Response
			{
				StatusCode = HttpStatusCode.InternalServerError,
				ReasonPhrase = reason,
				ContentType = MediaType.Plain.ToTextUtf8(),
        Contents = body =>
        {
          using(var writer = new StreamWriter(body))
          {
            writer.Write(error);
          }
        }
      });
		}

		protected void RespondUnprocessableEntity(string reasonPhrase)
		{
			Log.Error("[web] Unprocessable entity: {Reason:l}", reasonPhrase);

			Respond(new Response { StatusCode = HttpStatusCode.UnprocessableEntity, ReasonPhrase = reasonPhrase });
		}

		void When(FlowStopped e)
		{
			RespondError("[web] Flow stopped: " + Text.Of(e.Type), e.Error);
		}
	}
}