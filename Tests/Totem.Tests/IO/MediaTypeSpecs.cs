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
			Expect(MediaType.From(null, strict: false)).IsNull();
		}

		void FromEmpty()
		{
			Expect(MediaType.From("", strict: false)).IsNull();
		}

		void FromWhiteSpace()
		{
			Expect(MediaType.From(" \t ", strict: false)).IsNull();
		}

		void FromNullStrict()
		{
			ExpectThrows(() => MediaType.From(null));
		}

		void FromEmptyStrict()
		{
			ExpectThrows(() => MediaType.From(""));
		}

		void FromWhiteSpaceStrict()
		{
			ExpectThrows(() => MediaType.From(" \t "));
		}

		void From()
		{
			var mediaType = MediaType.From(_name);

			Expect(mediaType.Name).Is(_name);
		}

		void FromWithCharset()
		{
			var mediaType = MediaType.From(_nameWithCharset);

			Expect(mediaType.Name).Is(_name);
		}
	}
}