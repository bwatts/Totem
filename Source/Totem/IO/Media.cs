using System;
using System.Collections.Generic;
using System.Linq;

namespace Totem.IO
{
	/// <summary>
	/// Content of type <see cref="TContent"/> identified by media type
	/// </summary>
	/// <typeparam name="TContent">The identified type of content</typeparam>
	public class Media<TContent> : Clean
  {
		public Media(MediaType type, TContent content)
		{
			Type = type;
			Content = content;
		}

		public MediaType Type { get; private set; }
		public TContent Content { get; private set; }

		public override Text ToText() => Text.Of(Content);

		public bool Is(MediaType type)
		{
			return type == Type;
		}

		public Media<T> Cast<T>() where T : TContent
		{
			return new Media<T>(Type, (T) Content);
		}
	}
}