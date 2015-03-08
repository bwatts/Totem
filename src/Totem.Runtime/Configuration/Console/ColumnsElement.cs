using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;

namespace Totem.Runtime.Configuration.Console
{
	/// <summary>
	/// Configures the columns of the system console
	/// </summary>
	public class ColumnsElement : ConfigurationElement
	{
		[ConfigurationProperty("display", DefaultValue = 80)]
		public int Display
		{
			get { return (int) this["display"]; }
			set { this["display"] = value; }
		}

		[ConfigurationProperty("buffer", DefaultValue = 80)]
		public int Buffer
		{
			get { return (int) this["buffer"]; }
			set { this["buffer"] = value; }
		}

		public void Initialize()
		{
			System.Console.WindowWidth = Display;

			System.Console.BufferWidth = Buffer;
		}
	}
}