using System;
using System.Collections.Generic;
using System.Linq;

namespace Totem
{
	/// <summary>
	/// A signal on the timeline of a distributed environment
	/// </summary>
	public abstract class Message : Notion
	{
		protected Message()
		{
			When = Clock.Now;
		}

		public DateTime When
		{
			get { return Traits.When.Get(this); }
			private set { Traits.When.Set(this, value); }
		}

		public new static class Traits
		{
			public static readonly Tag<DateTime> When = Tag.Declare(() => When);
		}
	}
}