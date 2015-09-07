using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Totem.Runtime.Hosting.Commands
{
	/// <summary>
	/// A command to execute the runtime in a dedicated application domain
	/// </summary>
	public sealed class RunCommand : HostCommand
	{
		private AppDomain _appDomain;
		private RunCommandBridge _bridge;
		private int? _result;

		protected override int ExecuteCommand<TProgram>()
		{
			while(Executing)
			{
				ExecuteRuntime<TProgram>();
			}

			return FinalResult;
		}

		private bool Executing { get { return _result == null; } }
		private int FinalResult { get { return _result.Value; } }

		private void ExecuteRuntime<TProgram>() where TProgram : IRuntimeProgram, new()
		{
			try
			{
				ExecuteRuntimeAppDomain<TProgram>();
			}
			catch(Exception error)
			{
				_result = -1;

				Log.Error(error, "[runtime] Error while hosting runtime");
			}
		}

		//
		// App domain
		//

		private void ExecuteRuntimeAppDomain<TProgram>() where TProgram : IRuntimeProgram, new()
		{
			CreateAppDomain();

			CreateBridge();

			ExecuteBridge<TProgram>();
		}

		private void CreateAppDomain()
		{
			_appDomain = AppDomain.CreateDomain(typeof(RunCommand).FullName, null, new AppDomainSetup
			{
				ApplicationBase = AppDomain.CurrentDomain.SetupInformation.ApplicationBase,
				ConfigurationFile = AppDomain.CurrentDomain.SetupInformation.ConfigurationFile,
				ShadowCopyFiles = "true"
			});
		}

		private void CreateBridge()
		{
			var assemblyFile = Path.Combine(_appDomain.SetupInformation.ApplicationBase, "Totem.Runtime.dll");

			_bridge = (RunCommandBridge) _appDomain.CreateInstanceFromAndUnwrap(
				assemblyFile,
				typeof(RunCommandBridge).FullName,
				ignoreCase: false,
				bindingAttr: BindingFlags.Default,
				binder: null,
				args: null,
				culture: null,
				activationAttributes: null);
		}

		private void ExecuteBridge<TProgram>() where TProgram : IRuntimeProgram, new()
		{
			try
			{
				_result = _bridge.Execute<TProgram>(new ConsoleWriter());
			}
			finally
			{
				UnloadAppDomain();
			}
		}

		private void UnloadAppDomain()
		{
			try
			{
				AppDomain.Unload(_appDomain);
			}
			catch(Exception error)
			{
				Log.Error(error, "[runtime] Error while unloading app domain");

				_result = -1;
			}

			_appDomain = null;
			_bridge = null;
		}

		private sealed class ConsoleWriter : TextWriter
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
}