using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using Serilog;
using Serilog.Events;
using Totem.IO;
using Totem.Runtime.Configuration;
using Totem.Runtime.Map;

namespace Totem.Runtime.Hosting.Commands
{
	/// <summary>
	/// A command executed by the runtime host
	/// </summary>
	public abstract class HostCommand : Notion, IHostCommand
	{
		protected RuntimeSection Section { get; private set; }
		internal SerilogAdapter SerilogAdapter { get; private set; }

		public int Execute<TProgram>() where TProgram : IRuntimeProgram, new()
		{
			try
			{
				ReadSection();

				InitializeRuntime<TProgram>();

				InitializeLog();

				return ExecuteCommand<TProgram>();
			}
			catch(Exception error)
			{
				Log.Error(error, "[runtime] Error while hosting runtime");

				return -1;
			}
		}

		protected abstract int ExecuteCommand<TProgram>() where TProgram : IRuntimeProgram, new();

		private void ReadSection()
		{
			Section = RuntimeSection.Read();

			if(Section.HasUI)
			{
				Section.Console.Initialize();
			}
		}

		private void InitializeRuntime<TProgram>()
		{
      var deployment = Section.ReadDeployment(typeof(TProgram).Assembly);

      var runtime = new RuntimeReader(deployment).Read();

			Notion.Traits.InitializeRuntime(runtime);
		}

		private void InitializeLog()
		{
			SerilogAdapter = new SerilogAdapter(ReadLogger(), Section.Log.Level);

			Notion.Traits.InitializeLog(SerilogAdapter);
		}

		private ILogger ReadLogger()
		{
			var configuration = new LoggerConfiguration();

			SetSerilogLevel(configuration);

			WriteToRollingFile(configuration);

			WriteToConsoleIfHasUI(configuration);

			FormatDates(configuration);

			return configuration.CreateLogger();
		}

		private void SetSerilogLevel(LoggerConfiguration configuration)
		{
			configuration.MinimumLevel.Is((LogEventLevel) (Section.Log.Level - 1));
		}

		private void WriteToRollingFile(LoggerConfiguration configuration)
		{
			var pathFormat = Runtime.Deployment.LogFolder.Link.Then(FileResource.From("runtime-{Date}.txt")).ToString();

			configuration.WriteTo.RollingFile(
				pathFormat,
				outputTemplate: "{Timestamp:hh:mm:ss.fff tt} {Level,-11} | {Message}{NewLine}{Exception}");
		}

		private void WriteToConsoleIfHasUI(LoggerConfiguration configuration)
		{
			if(Section.HasUI)
			{
				configuration.WriteTo.ColoredConsole(outputTemplate: "{Timestamp:hh:mm:ss.fff tt} | {Message}{NewLine}{Exception}");
			}
		}

		private void FormatDates(LoggerConfiguration configuration)
		{
			configuration.Destructure.ByTransforming<DateTime>(value =>
				value.ToString(DateTimeFormatInfo.InvariantInfo.UniversalSortableDateTimePattern));
		}
	}
}