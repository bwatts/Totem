using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;

namespace Totem.Runtime.Configuration.Console
{
	/// <summary>
	/// Configures the system console
	/// </summary>
	public class ConsoleElement : ConfigurationElement
	{
		[ConfigurationProperty("title", DefaultValue = "Totem Runtime")]
		public string Title
		{
			get { return (string) this["title"]; }
			set { this["title"] = value; }
		}

		[ConfigurationProperty("rows")]
		public RowsElement Rows
		{
			get { return (RowsElement) this["rows"]; }
			set { this["rows"] = value; }
		}

		[ConfigurationProperty("columns")]
		public ColumnsElement Columns
		{
			get { return (ColumnsElement) this["columns"]; }
			set { this["columns"] = value; }
		}

		public void Initialize()
		{
			System.Console.Title = Title;

			Rows.InitializeConsole();

			Columns.InitializeConsole();
		}
	}
}