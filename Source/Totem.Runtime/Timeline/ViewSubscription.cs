using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Linq;

namespace Totem.Runtime.Timeline
{
	/// <summary>
	/// The result of a view subscribing to updates
	/// </summary>
	public class ViewSubscription
	{
    public ViewSubscription(ViewETag etag)
    {
      ETag = etag;
    }

    public ViewSubscription(ViewETag etag, JObject content)
		{
			ETag = etag;
      Content = content;
		}

    public readonly ViewETag ETag;
		public readonly JObject Content;

		public override string ToString() => ETag.ToString();
	}
}