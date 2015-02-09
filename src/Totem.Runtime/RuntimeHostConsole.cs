using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Totem.Runtime.Configuration;

namespace Totem.Runtime
{
	/// <summary>
	/// A console application hosting an instance of the Totem runtime
	/// </summary>
	internal sealed class RuntimeHostConsole : Notion
	{
		private readonly RuntimeInstance _instance = new RuntimeInstance();
		private readonly RuntimeSection _settings;

		internal RuntimeHostConsole(RuntimeSection settings)
		{
			_settings = settings;
		}

		internal int Run()
		{
			InitializeConsole();

			return _instance.TryStart(_settings) ? RunConsole() : FailConsole();
		}

		private void InitializeConsole()
		{
			Console.Title = _settings.Service.Name;
		}

		private int RunConsole()
		{
			ReadKey("Press any key to stop . . .");

			var exitCode = _instance.TryStop() ? 0 : -1;

			if(Debugger.IsAttached)
			{
				ReadKey("Press any key to continue . . .");
			}
			else
			{
				Console.WriteLine();
			}

			return exitCode;
		}

		private int FailConsole()
		{
			if(Debugger.IsAttached)
			{
				ReadKey("Press any key to continue . . .");
			}

			return -1;
		}

		private static void ReadKey(string prompt)
		{
			Console.WriteLine();
			Console.WriteLine(prompt);
			Console.WriteLine();

			System.Console.ReadKey(intercept: true);
		}
	}
}