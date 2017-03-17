using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Linq;

namespace Totem.Web.Push
{
	/// <summary>
	/// Indicates a view updated its position on the timeline
	/// </summary>
	public class ViewUpdated : Event
	{
		public ViewUpdated(string key, string etag, JObject content)
		{
			Key = key;
			ETag = etag;
      Content = content;
		}

		public readonly string Key;
		public readonly string ETag;
		public readonly JObject Content;

		public override Text ToText() => ETag;
	}
}