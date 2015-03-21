using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;

namespace Totem.Runtime.Configuration.Console
{
	/// <summary>
	/// Configures the rows of the system console
	/// </summary>
	public class RowsElement : ConfigurationElement
	{
		[ConfigurationProperty("display", DefaultValue = 80)]
		public int Display
		{
			get { return (int) this["display"]; }
			set { this["display"] = value; }
		}

		[ConfigurationProperty("buffer", DefaultValue = 1000)]
		public int Buffer
		{
			get { return (int) this["buffer"]; }
			set { this["buffer"] = value; }
		}

		public void Initialize()
		{
			var maxHeight = System.Console.LargestWindowHeight;

			System.Console.WindowHeight = Display <= maxHeight ? Display : maxHeight;

			System.Console.BufferHeight = Buffer >= Display ? Buffer : Display;
		}
	}
}