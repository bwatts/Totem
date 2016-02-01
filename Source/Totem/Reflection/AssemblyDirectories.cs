using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;

namespace Totem.Reflection
{
	/// <summary>
	/// Extends assemblies with the ability to get the directories containing their corresponding .dll files
	/// </summary>
	[EditorBrowsable(EditorBrowsableState.Never)]
	public static class AssemblyDirectories
	{
		public static string GetDirectoryName(this Assembly assembly)
		{
			var codeBase = assembly.CodeBase;

			Uri codeBaseUri;

			if(Uri.TryCreate(codeBase, UriKind.Absolute, out codeBaseUri))
			{
				codeBase = codeBaseUri.AbsolutePath;
			}

			return System.IO.Path.GetDirectoryName(codeBase);
		}
	}
}