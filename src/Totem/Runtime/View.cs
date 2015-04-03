using System;
using System.Collections.Generic;
using System.Linq;

namespace Totem.Runtime
{
	/// <summary>
	/// The state of a timeline query at a specific point
	/// </summary>
	public abstract class View : Notion
	{
		protected View(string key)
		{
			Key = key;
			WhenCreated = Clock.Now;
			WhenUpdated = WhenCreated;
		}

		public readonly string Key;
		public readonly DateTime WhenCreated;
		public DateTime WhenUpdated { get; private set; }

		public override Text ToText()
		{
			return Key;
		}

		public void OnUpdated()
		{
			WhenUpdated = Clock.Now;
		}
	}
}