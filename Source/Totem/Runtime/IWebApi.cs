using System;
using System.Collections.Generic;
using System.Linq;

namespace Totem.Runtime
{
	/// <summary>
	/// Describes an instance of a Totem web API bound to an HTTP request
	/// </summary>
	public interface IWebApi : ITextable, IBindable
	{
		WebApiCall Call { get; }
	}
}