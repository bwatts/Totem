using System;
using System.Collections.Generic;
using System.Linq;

namespace Totem
{
	/// <summary>
	/// The relative importance of a log message
	/// </summary>
	public enum LogLevel
	{
		/// <summary>
		/// A message whose importance is determined by other criteria
		/// </summary>
		Inherit,

		/// <summary>
		/// A message relevant to observers of runtime state and flow
		/// </summary>
		Verbose,

		/// <summary>
		/// A message relevant to system authors
		/// </summary>
		Debug,

		/// <summary>
		/// A message relevant to system authors and users
		/// </summary>
		Info,

		/// <summary>
		/// A message indicating an expected scenario that may cause future errors
		/// </summary>
		Warning,

		/// <summary>
		/// A message indicating an unexpected scenario
		/// </summary>
		Error,

		/// <summary>
		/// A message indicating the system may shut down
		/// </summary>
		Fatal
	}
}