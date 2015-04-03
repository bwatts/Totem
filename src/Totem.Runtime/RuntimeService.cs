using System;
using System.Collections.Generic;
using System.ComponentModel.Composition.Hosting;
using System.IO;
using System.Linq;
using System.Reflection;
using Serilog;
using Serilog.Events;
using Topshelf;
using Totem.IO;
using Totem.Reflection;
using Totem.Runtime.Configuration;
using Totem.Runtime.Map;

namespace Totem.Runtime
{
	/// <summary>
	/// The Topshelf service control hosting the Totem runtime
	/// </summary>
	internal sealed class RuntimeService : ServiceControl
	{
		private readonly Assembly _assembly;
		private readonly string _sectionName;
		private RuntimeSection _section;
		private RuntimeMap _map;
		private CompositionContainer _container;
		private IDisposable _instance;

		internal RuntimeService(Assembly assembly, string sectionName)
		{
			_assembly = assembly;
			_sectionName = sectionName;
		}

		internal SerilogAdapter Log { get; private set; }

		internal void Initialize()
		{
			SetCurrentDirectoryToRuntime();

			ReadSection();

			InitializeConsoleIfHasUI();

			InitializeMap();

			InitializeLog();
		}

		private void SetCurrentDirectoryToRuntime()
		{
			// Run the service where installed, instead of a system folder

			Directory.SetCurrentDirectory(_assembly.GetDirectoryName());
		}

		private void ReadSection()
		{
			_section = RuntimeSection.Read(_sectionName);
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
			_map = _section.ReadMap();

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
		// Start/stop
		//

		public bool Start(HostControl hostControl)
		{
			CreateContainer();

			LoadInstance();

			return true;
		}

		public bool Stop(HostControl hostControl)
		{
			UnloadInstance();

			DisposeContainer();

			ClearFields();

			return true;
		}

		private void CreateContainer()
		{
			_container = new CompositionContainer(CreateCatalog(), CompositionOptions.DisableSilentRejection);
		}

		private AssemblyCatalog CreateCatalog()
		{
			return new AssemblyCatalog(_assembly);
		}

		private void LoadInstance()
		{
			var loader = ReadLoader();

			if(loader != null)
			{
				_instance = loader.Connect();
			}
		}

		private IConnectable ReadLoader()
		{
			return _container.GetExportedValueOrDefault<IConnectable>(RuntimeHost.LoaderContract);
		}

		private void UnloadInstance()
		{
			if(_instance != null)
			{
				_instance.Dispose();
			}
		}

		private void DisposeContainer()
		{
			if(_container != null)
			{
				_container.Dispose();
			}
		}

		private void ClearFields()
		{
			_section = null;
			_container = null;
			_instance = null;
			_map = null;
			_instance = null;
		}
	}
}