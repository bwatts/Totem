using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Totem.IO;

namespace Totem.Web
{
	/// <summary>
	/// Describes the body of a web API request
	/// </summary>
	public interface IRequestBody
	{
		Media<string> ReadAsString();

		Media<Stream> ReadAsStream();
	}
}