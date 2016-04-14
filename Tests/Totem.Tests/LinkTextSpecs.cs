using System;
using System.Collections.Generic;
using System.Linq;

namespace Totem
{
	/// <summary>
	/// Scenarios involving the <see cref="Totem.LinkText"/> class
	/// </summary>
	public class LinkTextSpecs : Specs
  {
		void CreateEmpty()
		{
			var text = new LinkText("");

			Expect(text.ToString()).Is("");
			Expect(text.IsNone);
			ExpectNot(text.IsTemplate);
		}

		void CreateNonEmpty()
		{
			var text = new LinkText("x");

			Expect(text.ToString()).Is("x");
			ExpectNot(text.IsNone);
			ExpectNot(text.IsTemplate);
		}

		void CreateTemplate()
		{
			var text = new LinkText("{x}");

			Expect(text.ToString()).Is("{x}");
			ExpectNot(text.IsNone);
			Expect(text.IsTemplate);
		}

		void CreateTemplateHalf()
		{
			var text = new LinkText("{x");

			Expect(text.ToString()).Is("{x");
			ExpectNot(text.IsNone);
			ExpectNot(text.IsTemplate);
		}

		void Equal()
		{
			var text1 = new LinkText("x");
			var text2 = new LinkText("x");

			Expect(text1).Is(text2);
		}

		void NotEqual()
		{
			var text1 = new LinkText("x");
			var text2 = new LinkText("y");

			Expect(text1).IsNot(text2);
		}
	}
}