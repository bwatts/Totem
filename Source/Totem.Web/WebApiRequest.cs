using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Nancy;
using Totem.Http;
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

		protected void Respond(HttpStatusCode statusCode, string reason)
		{
			Respond(new Response { StatusCode = statusCode, ReasonPhrase = reason });
		}

		// 200

		protected void RespondOK(string reason)
		{
			Respond(HttpStatusCode.OK, reason);
		}

		// 201

    protected void RespondCreated(string reason, string location)
    {
      Respond(new Response
			{
				StatusCode = HttpStatusCode.Created,
				ReasonPhrase = reason,
				Headers =
				{
					["Location"] = location
				}
			});
    }

		// 202

		protected void RespondAccepted(string reason, string location)
		{
			Respond(new Response
			{
				StatusCode = HttpStatusCode.Accepted,
				ReasonPhrase = reason,
				Headers =
				{
					["Location"] = location
				}
			});
		}

		// 204

		protected void RespondNoContent(string reason)
		{
			Respond(HttpStatusCode.NoContent, reason);
		}

		// 404

		protected void RespondNotFound(string reason)
		{
			Log.Error("[web] 404 Not Found: {Reason:l}", reason);

			Respond(HttpStatusCode.NotFound, reason);
		}

		// 409

		protected void RespondConflict(string reason)
		{
			Log.Error("[web] 409 Conflict: {Reason:l}", reason);

			Respond(HttpStatusCode.Conflict, reason);
		}

		// 422

		protected void RespondUnprocessableEntity(string reason)
		{
			Log.Error("[web] 422 Unprocessable entity: {Reason:l}", reason);

			Respond(HttpStatusCode.UnprocessableEntity, reason);
		}

		// 500

		protected void RespondError(string reason)
		{
			Log.Error("[web] 500 Internal server error: {Reason:l}", reason);

			Respond(HttpStatusCode.InternalServerError, reason);
		}

		protected void RespondError(string reason, string error)
		{
			Log.Error("[web] 500 Internal server error: {Reason:l} {Error}", reason, error);

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

		void When(FlowStopped e)
		{
			RespondError(
				"Flow stopped: " + Text.Of(e.Type).WriteIf(e.Id.IsAssigned, $"/{e.Id}"),
				e.Error);
		}
	}
}