using System;
using System.Collections.Generic;
using System.Linq;

namespace Totem.Web
{
	/// <summary>
	/// The level of detail reported for web API errors
	/// </summary>
	public enum ErrorDetail
	{
		None,
    Issue,
		Message,
		StackTrace
	}
}