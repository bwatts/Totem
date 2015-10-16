using System;
using System.Collections.Generic;
using System.Linq;
using NDesk.Options;
using Totem.IO;

namespace Totem.Runtime.Hosting.Commands
{
	/// <summary>
	/// Input to the runtime host specifying a command to execute
	/// </summary>
	public sealed class HostCommandLine
	{
		private readonly string _verb;
		private readonly Many<string> _options;
		private readonly IRuntimeBuild _build;

		private HostCommandLine(string verb, Many<string> options, IRuntimeBuild build)
		{
			_verb = verb;
			_options = options;
			_build = build;
		}

		public override string ToString()
		{
			return _verb + " " + _options.ToTextSeparatedBy(" ");
		}

		public int Execute<TProgram>() where TProgram : IRuntimeProgram, new()
		{
			return ReadCommand().Execute<TProgram>();
		}

		private IHostCommand ReadCommand()
		{
			try
			{
				return ReadKnownCommand();
			}
			catch(Exception error)
			{
				return new IssueCommand(error);
			}
		}

		private IHostCommand ReadKnownCommand()
		{
			if(VerbIsAny("help", "install", "start", "stop", "uninstall"))
			{
				return new PassThroughCommand();
			}
			else if(VerbIsAny("", "run"))
			{
				return new RunCommand();
			}
			else if(VerbIs("deploy"))
			{
				return ReadDeployCommand();
			}
			else
			{
				throw new FormatException("Could not read command: " + ToString());
			}
		}

		private IHostCommand ReadDeployCommand()
		{
			string location = null;

			new OptionSet
			{
				{ "l=|location=", value => location = value }
			}
			.Parse(_options);

			return new DeployCommand(_build, IOLink.From(location));
		}

		private bool VerbIs(string verb)
		{
			return _verb.Equals(verb, StringComparison.InvariantCultureIgnoreCase);
		}

		private bool VerbIsAny(params string[] verbs)
		{
			return verbs.Any(VerbIs);
		}

		//
		// From
		//

		public static HostCommandLine From(string[] args)
		{
			var first = args.FirstOrDefault() ?? "";
			var rest = args.Skip(1);

			var implicitRun = first.StartsWith("-");

			var verb = implicitRun ? "run" : first;
			var options = implicitRun ? args : rest;

			return new HostCommandLine(verb, options.ToMany(), new RuntimeBuild());
		}

		public static HostCommandLine FromEnvironment()
		{
			return From(Environment.GetCommandLineArgs());
		}
	}
}