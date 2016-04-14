using System;
using System.Collections.Generic;
using System.Linq;

namespace Totem.IO
{
	/// <summary>
	/// Scenarios involving the <see cref="Totem.IO.MediaType"/> class
	/// </summary>
	public class MediaTypeSpecs : Specs
  {
		string _name = "text/plain";
		string _nameWithCharset = "text/plain; charset=utf-8";

		void FromNull()
		{
			var mediaType = new MediaType(null);

			Expect(mediaType.Name).Is(MediaType.Names.None);
			Expect(mediaType.IsNone);
		}

		void FromEmpty()
		{
			var mediaType = new MediaType("");

			Expect(mediaType.Name).Is(MediaType.Names.None);
			Expect(mediaType.IsNone);
		}

		void FromWhiteSpace()
		{
			var mediaType = new MediaType(" \t\n ");

			Expect(mediaType.Name).Is(MediaType.Names.None);
			Expect(mediaType.IsNone);
		}

		void FromName()
		{
			var mediaType = new MediaType(_name);

			Expect(mediaType.Name).Is(_name);
			ExpectNot(mediaType.IsNone);
		}

		void FromNameWithCharset()
		{
			var mediaType = new MediaType(_nameWithCharset);

			Expect(mediaType.Name).Is(_name);
			ExpectNot(mediaType.IsNone);
		}
	}
}