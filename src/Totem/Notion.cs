using System;
using System.Collections.Generic;
using System.Linq;

namespace Totem
{
	/// <summary>
	/// An object aware of Totem modeling techniques
	/// </summary>
	public abstract class Notion : IWritable, ITaggable
	{
		protected Notion()
		{
			Tags = new Tags();
		}

		Tags ITaggable.Tags { get { return Tags; } }

		protected Tags Tags { get; private set; }

		protected IClock Clock { get { return Traits.Clock.Get(this); } }

		public sealed override string ToString()
		{
			return ToText();
		}

		public virtual Text ToText()
		{
			return base.ToString();
		}

		protected IExpect<T> Expect<T>(T value)
		{
			return Totem.Expect.That(value);
		}

		protected void ExpectTrue(bool condition, Text issue = null, Text expected = null, Text actual = null)
		{
			Totem.Expect.True(condition, issue, expected, actual);
		}

		protected void ExpectFalse(bool condition, Text issue = null, Text expected = null, Text actual = null)
		{
			Totem.Expect.False(condition, issue, expected, actual);
		}

		public static class Traits
		{
			public static readonly Tag<IClock> Clock = Tag.Declare(() => Clock, new PlatformClock());

			private sealed class PlatformClock : IClock
			{
				public DateTime Now
				{
					get { return DateTime.UtcNow; }
				}
			}
		}
	}
}