using System;
using System.Collections.Generic;
using System.Linq;
using Totem.Http;

namespace Totem.Web
{
	/// <summary>
	/// Describes part of the Totem web API
	/// </summary>
	public interface IWebApi : IWritable, ITaggable
	{
		WebApiScope Scope { get; }

		HttpLink Link { get; }

		HttpAuthorization Authorization { get; }

		IRequestBody RequestBody { get; }
	}
}