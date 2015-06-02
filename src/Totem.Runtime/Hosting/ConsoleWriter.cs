using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Totem.Runtime.Hosting
{
	/// <summary>
	/// Performs writes in the runtime app domain to the console in the host app domain
	/// </summary>
	internal sealed class ConsoleWriter : TextWriter
	{
		public override Encoding Encoding
		{
			get { return Console.OutputEncoding; }
		}

		public override void Write(bool value) { Console.Write(value); }
		public override void Write(char value) { Console.Write(value); }
		public override void Write(char[] buffer) { Console.Write(buffer); }
		public override void Write(decimal value) { Console.Write(value); }
		public override void Write(double value) { Console.Write(value); }
		public override void Write(float value) { Console.Write(value); }
		public override void Write(int value) { Console.Write(value); }
		public override void Write(long value) { Console.Write(value); }
		public override void Write(object value) { Console.Write(value); }
		public override void Write(string value) { Console.Write(value); }
		public override void Write(uint value) { Console.Write(value); }
		public override void Write(ulong value) { Console.Write(value); }
		public override void Write(string format, object arg0) { Console.Write(format, arg0); }
		public override void Write(string format, params object[] arg) { Console.Write(format, arg); }
		public override void Write(char[] buffer, int index, int count) { Console.Write(buffer, index, count); }
		public override void Write(string format, object arg0, object arg1) { Console.Write(format, arg0, arg1); }
		public override void Write(string format, object arg0, object arg1, object arg2) { Console.Write(format, arg0, arg1, arg2); }

		public override void WriteLine() { Console.WriteLine(); }
		public override void WriteLine(bool value) { Console.WriteLine(value); }
		public override void WriteLine(char value) { Console.WriteLine(value); }
		public override void WriteLine(char[] buffer) { Console.WriteLine(buffer); }
		public override void WriteLine(decimal value) { Console.WriteLine(value); }
		public override void WriteLine(double value) { Console.WriteLine(value); }
		public override void WriteLine(float value) { Console.WriteLine(value); }
		public override void WriteLine(int value) { Console.WriteLine(value); }
		public override void WriteLine(long value) { Console.WriteLine(value); }
		public override void WriteLine(object value) { Console.WriteLine(value); }
		public override void WriteLine(string value) { Console.WriteLine(value); }
		public override void WriteLine(uint value) { Console.WriteLine(value); }
		public override void WriteLine(ulong value) { Console.WriteLine(value); }
		public override void WriteLine(string format, object arg0) { Console.WriteLine(format, arg0); }
		public override void WriteLine(string format, params object[] arg) { Console.WriteLine(format, arg); }
		public override void WriteLine(char[] buffer, int index, int count) { Console.WriteLine(buffer, index, count); }
		public override void WriteLine(string format, object arg0, object arg1) { Console.WriteLine(format, arg0, arg1); }
		public override void WriteLine(string format, object arg0, object arg1, object arg2) { Console.WriteLine(format, arg0, arg1, arg2); }

		public override object InitializeLifetimeService()
		{
			// Null tells .NET Remoting that this object has an infinite lease. See Modifying Lease Properties:
			//
			// http://msdn.microsoft.com/en-us/library/23bk23zc%28v=vs.71%29.aspx

			return null;
		}
	}
}