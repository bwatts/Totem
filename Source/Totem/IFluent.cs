using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace Totem
{
	/// <summary>
	/// Describes an interface whose discovery experience excludes definitions from <see cref="System.Object"/>
	/// </summary>
	public interface IFluent
	{
		[EditorBrowsable(EditorBrowsableState.Never)] Type GetType();
		[EditorBrowsable(EditorBrowsableState.Never)] int GetHashCode();
		[EditorBrowsable(EditorBrowsableState.Never)] string ToString();
		[EditorBrowsable(EditorBrowsableState.Never)] bool Equals(object obj);
	}
}