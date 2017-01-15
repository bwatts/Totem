using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Nancy;
using Totem.IO;
using Totem.Runtime;
using Totem.Runtime.Json;

namespace Totem.Web
{
	/// <summary>
	/// A process publishing and observing the timeline to make a web request
	/// </summary>
	public abstract class WebApiRequest : Runtime.Timeline.Request
	{
		[Transient] public Response Response { get; private set; }

		protected void Respond(Response response)
		{
			Expect(Response).IsNull("Web API flow has already responded");

      Response = response;

			ThenDone();
		}

    protected void Respond(HttpStatusCode code, string reason, object contents = null)
    {
      Respond(new Response { StatusCode = code, ReasonPhrase = reason });

      if(contents != null)
      {
        Response.ContentType = MediaType.Json.ToTextUtf8();

        Response.Contents = body =>
        {
          using(var writer = new StreamWriter(body))
          {
            writer.Write(JsonFormat.Text.Serialize(contents));
          }
        };
      }
    }

    // 200

    protected void RespondOK(string reason, object contents = null)
		{
			Respond(HttpStatusCode.OK, reason, contents);
		}

		// 201

    protected void RespondCreated(string reason, string location, object contents = null)
    {
      Respond(HttpStatusCode.Created, reason, contents);

      Response.Headers["Location"] = location;
    }

		// 202

		protected void RespondAccepted(string reason, string location, object contents = null)
		{
      Respond(HttpStatusCode.Accepted, reason, contents);

      Response.Headers["Location"] = location;
		}

		// 204

		protected void RespondNoContent(string reason, object contents = null)
		{
			Respond(HttpStatusCode.NoContent, reason, contents);
		}

		// 404

		protected void RespondNotFound(string reason, object contents = null)
		{
			Respond(HttpStatusCode.NotFound, reason, contents);
		}

		// 409

		protected void RespondConflict(string reason, object contents = null)
		{
			Respond(HttpStatusCode.Conflict, reason, contents);
		}

		// 422

		protected void RespondUnprocessableEntity(string reason, object contents = null)
		{
			Respond(HttpStatusCode.UnprocessableEntity, reason, contents);
		}

		// 500

		protected void RespondError(string reason, object contents = null)
		{
			Log.Error("[web] 500 Internal server error: {Reason:l}", reason);

			Respond(HttpStatusCode.InternalServerError, reason, contents);
		}

		protected void RespondError(string reason, string error)
		{
			Log.Error("[web] 500 Internal server error: {Reason:l} {Error}", reason, error);

      Respond(HttpStatusCode.InternalServerError, reason);

      Response.ContentType = MediaType.Plain.ToTextUtf8();

      Response.Contents = body =>
      {
        using(var writer = new StreamWriter(body))
        {
          writer.Write(error);
        }
      };
		}
	}
}