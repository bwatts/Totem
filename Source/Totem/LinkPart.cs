using System;
using System.Collections.Generic;
using System.Text;

namespace Totem
{
	/// <summary>
	/// A composable element of a link. A part may represent a template - a pattern for producing links.
	/// </summary>
	public abstract class LinkPart : Notion
	{
		public abstract bool IsTemplate { get; }

		public abstract override Text ToText();
	}
}