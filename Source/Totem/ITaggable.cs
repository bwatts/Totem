using System;
using System.Collections.Generic;
using System.Linq;

namespace Totem
{
	/// <summary>
	/// Describes an object hosting values corresponding to a set of tags
	/// </summary>
	public interface ITaggable : IFluent
	{
		Tags Tags { get; }
	}
}