using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Totem
{
	/// <summary>
	/// Indicates a value failed to meet an expectation
	/// </summary>
	/// <remarks>
	/// http://stackoverflow.com/questions/94488/what-is-the-correct-way-to-make-a-custom-net-exception-serializable
	/// </remarks>
	[Serializable]
	public sealed class ExpectException : Exception
	{
		internal ExpectException(string issue, string expected, string actual)
			: base(GetMessage(issue, expected, actual))
		{
			Issue = issue;
			Expected = expected;
			Actual = actual;
		}

		internal ExpectException(Exception inner, string issue, string expected, string actual)
			: base(GetMessage(issue, expected, actual), inner)
		{
			Issue = issue;
			Expected = expected;
			Actual = actual;
		}

		[SecurityPermissionAttribute(SecurityAction.Demand, SerializationFormatter = true)]
		private ExpectException(SerializationInfo info, StreamingContext context) : base(info, context)
    {
			Issue = info.GetString("Issue");
			Expected = info.GetString("Expected");
			Actual = info.GetString("Actual");
    }

		public string Issue { get; private set; }
		public string Expected { get; private set; }
		public string Actual { get; private set; }

		[SecurityPermissionAttribute(SecurityAction.Demand, SerializationFormatter = true)]
		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			info.AddValue("Issue", Issue);
			info.AddValue("Expected", Expected);
			info.AddValue("Actual", Actual);

			base.GetObjectData(info, context);
		}

		private static string GetMessage(string issue, string expected, string actual)
		{
			// TODO: Indent after line breaks

			return Text.None
				.WriteIf(
					issue.Length == 1,
					issue.ToUpper(),
					Char.ToUpper(issue[0]) + issue.Substring(1))
				.WriteTwoLines()
				.Write("Expected | ")
				.WriteLine(expected)
				.Write("Actual   | ")
				.Write(actual);
		}
	}
}