using System;
using System.Collections.Generic;
using System.Linq;

namespace Totem
{
	/// <summary>
	/// Indicates a value failed to meet an expectation
	/// </summary>
	public class ExpectException : Exception
	{
		public ExpectException(object value, string expected, string actual, string because)
			: base(GetMessage(value, expected, actual, because))
		{
			Value = value;
			Expected = expected;
			Actual = actual;
			Because = because;
		}

		public ExpectException(Exception inner, object value, string expected, string actual, string because)
			: base(GetMessage(value, expected, actual, because), inner)
		{
			Value = value;
			Expected = expected;
			Actual = actual;
			Because = because;
		}

		public object Value { get; private set; }
		public string Expected { get; private set; }
		public string Actual { get; private set; }
		public string Because { get; private set; }

		private static string GetMessage(object value, string expected, string actual, string because)
		{
			var message = Text.None;

			if(!String.IsNullOrEmpty(because))
			{
				if(because.StartsWith("because", StringComparison.OrdinalIgnoreCase))
				{
					because = because.Substring("because".Length).TrimStart();
				}

				message = message.WriteIf(
					because.Length == 1,
					because.ToUpper(),
					Char.ToUpper(because[0]) + because.Substring(1));
			}



			// TODO: Indent expected/actual if they contain line breaks



			return message
				.WriteTwoLines()
				.Write("Expected | ")
				.Write(expected)
				.WriteLine()
				.Write("Actual   | ")
				.WriteLine(actual);
		}
	}
}