using System;
using System.Collections.Generic;
using System.ComponentModel.Composition.Hosting;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using Serilog;
using Serilog.Events;
using Topshelf;
using Totem.IO;
using Totem.Reflection;
using Totem.Runtime.Configuration;
using Totem.Runtime.Map;
using Totem.Runtime.Reflection;

namespace Totem.Runtime.Hosting
{
	/// <summary>
	/// The Topshelf service control hosting the Totem runtime
	/// </summary>
	internal sealed class RuntimeService : ServiceControl
	{
		private readonly Assembly _hostAssembly;
		private RuntimeSection _section;
		private RuntimeMap _map;
		private HostControl _hostControl;
		private CompositionContainer _container;
		private CancellationTokenSource _cancellationTokenSource;
		private IDisposable _instance;

		internal RuntimeService(Assembly hostAssembly)
		{
			_hostAssembly = hostAssembly;

			SetCurrentDirectoryToHost();

			ReadSection();

			InitializeBridge();

			InitializeConsoleIfHasUI();

			InitializeMap();

			InitializeLog();
		}

		internal SerilogAdapter Log { get; private set; }

		//
		// Initialization
		//

		private void SetCurrentDirectoryToHost()
		{
			// Run the service where installed, instead of a system folder

			Directory.SetCurrentDirectory(_hostAssembly.GetDirectoryName());
		}

		private void ReadSection()
		{
			_section = RuntimeSection.Read();
		}

		private void InitializeBridge()
		{
			RuntimeHost.Bridge.Restarting += (sender, e) =>
			{
				_hostControl.Stop();
			};
		}

		private void InitializeConsoleIfHasUI()
		{
			if(_section.HasUI)
			{
				_section.Console.Initialize();
			}
		}

		private void InitializeMap()
		{
			_map = _section.ReadMap(_hostAssembly);

			Notion.Traits.InitializeRuntime(_map);
		}

		private void InitializeLog()
		{
			Log = new SerilogAdapter(ReadLogger(), _section.Log.Level);

			Notion.Traits.InitializeLog(Log);
		}

		private ILogger ReadLogger()
		{
			var configuration = new LoggerConfiguration().MinimumLevel.Is(ReadSerilogLevel());

			WriteToRollingFile(configuration);

			WriteToConsoleIfHasUI(configuration);

			return configuration.CreateLogger();
		}

		private LogEventLevel ReadSerilogLevel()
		{
			return (LogEventLevel) (_section.Log.Level - 1);
		}

		private void WriteToRollingFile(LoggerConfiguration configuration)
		{
			var pathFormat = _map.Deployment.LogFolder.Link.Then(FileResource.From("runtime-{Date}.txt")).ToString();

			configuration.WriteTo.RollingFile(
				pathFormat,
				outputTemplate: "{Timestamp:hh:mm:ss.fff tt} {Level,-11} | {Message}{NewLine}{Exception}");
		}

		private void WriteToConsoleIfHasUI(LoggerConfiguration configuration)
		{
			if(_section.HasUI)
			{
				configuration.WriteTo.ColoredConsole(outputTemplate: "{Timestamp:hh:mm:ss.fff tt} | {Message}{NewLine}{Exception}");
			}
		}

		//
		// Start
		//

		public bool Start(HostControl hostControl)
		{
			_hostControl = hostControl;

			Log.Info("[runtime] Starting service");

			OpenScope();

			LoadInstance();

			Log.Info("[runtime] Service started");

			return true;
		}

		private void OpenScope()
		{
			_container = new CompositionContainer(_map.Catalog, CompositionOptions.DisableSilentRejection);

			_cancellationTokenSource = new CancellationTokenSource();
		}

		private void LoadInstance()
		{
			var compositionRoot = _container.GetExportedValue<CompositionRoot>();

			_instance = compositionRoot.Connect(_cancellationTokenSource.Token);
		}

		//
		// Stop
		//

		public bool Stop(HostControl hostControl)
		{
			UnloadInstance();

			CloseScope();

			return true;
		}

		private void UnloadInstance()
		{
			_cancellationTokenSource.Cancel();

			_instance.Dispose();
		}

		private void CloseScope()
		{
			_container.Dispose();

			_section = null;
			_map = null;
			_hostControl = null;
			_container = null;
			_cancellationTokenSource = null;
			_instance = null;
		}
	}
}