using System;
using System.Collections.Generic;
using System.Linq;
using Xunit.Sdk;

namespace Totem
{
	/// <summary>
	/// Indicates a scenario with an unexpected outcome
	/// </summary>
	public class ScenarioException : AssertException
	{
		internal ScenarioException(ExpectException error, string caller, string callerFile, int callerLine)
			: base(GetMessage(error, caller, callerFile, callerLine))
		{}

		private static string GetMessage(ExpectException error, string caller, string callerFile, int callerLine)
		{
			return Text
				.Of("[ln {0}] ", callerLine)
				.Write(error.Message)
				.WriteTwoLines()
				.Write("Scenario | ").WriteLine(caller)
				.WriteLine("Location | {0} ln {1}", callerFile, callerLine);
		}
	}
}