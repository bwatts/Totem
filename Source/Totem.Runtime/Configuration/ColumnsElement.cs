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
		[ConfigurationProperty("display", DefaultValue = 120)]
		public int Display
		{
			get { return (int) this["display"]; }
			set { this["display"] = value; }
		}

		[ConfigurationProperty("buffer", DefaultValue = 120)]
		public int Buffer
		{
			get { return (int) this["buffer"]; }
			set { this["buffer"] = value; }
		}

		public void InitializeConsole()
		{
			var maxWidth = System.Console.LargestWindowWidth;

			System.Console.WindowWidth = Display <= maxWidth ? Display : maxWidth;

			System.Console.BufferWidth = Buffer >= Display ? Buffer : Display;
		}
	}
}