using System;
using System.Collections.Generic;
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

			if(!TryStartHost())
			{
				return -1;
			}
			else
			{
				ReadKey();

				return TryStopHost() ? 0 : -1;
			}
		}

		private void InitializeConsole()
		{
			Console.Title = _settings.Service.Name;
		}

		private bool TryStartHost()
		{
			//Console.WriteInfo("Runtime starting");

			if(_instance.TryStart(_settings))
			{
				//Console.WriteInfo("Runtime started. Press any key to stop...");

				return true;
			}
			else
			{
				//Console.WriteError(Text
				//	.Of("Runtime failed to start")
				//	.WriteTwoLines()
				//	.Write(_instance.Error)
				//	.WriteTwoLines()
				//	.Write("Press any key to stop..."));

				ReadKey();

				return false;
			}
		}

		private void ReadKey()
		{
			System.Console.ReadKey(intercept: true);
		}

		private bool TryStopHost()
		{
			//Console.WriteInfo("Runtime stopping");

			if(_instance.TryStop())
			{
				//Console.WriteInfo("Runtime stopped");

				return true;
			}
			else
			{
				//Console.WriteError("Runtime failed to stop", _instance.Error);

				return false;
			}
		}
	}
}