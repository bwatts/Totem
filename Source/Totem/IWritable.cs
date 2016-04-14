using System;
using System.Collections.Generic;
using System.Linq;

namespace Totem
{
	/// <summary>
	/// Describes an object that writes itself to text
	/// </summary>
	public interface IWritable : IFluent
	{
		Text ToText();
	}
}