using System;
using System.Collections.Generic;
using System.Linq;

namespace Totem
{
	/// <summary>
	/// Scenarios involving the <see cref="Totem.LinkText"/> class
	/// </summary>
	public class LinkTextScenarios : Scenarios
	{
		void CreateEmpty()
		{
			var text = new LinkText("");

			Expect(text.ToString()).Is("");
			ExpectTrue(text.IsEmpty);
			ExpectFalse(text.IsNotEmpty);
			ExpectFalse(text.IsTemplate);
		}

		void CreateNonEmpty()
		{
			var text = new LinkText("x");

			Expect(text.ToString()).Is("x");
			ExpectFalse(text.IsEmpty);
			ExpectTrue(text.IsNotEmpty);
			ExpectFalse(text.IsTemplate);
		}

		void CreateTemplate()
		{
			var text = new LinkText("{x}");

			Expect(text.ToString()).Is("{x}");
			ExpectFalse(text.IsEmpty);
			ExpectTrue(text.IsNotEmpty);
			ExpectTrue(text.IsTemplate);
		}

		void CreateTemplateHalf()
		{
			var text = new LinkText("{x");

			Expect(text.ToString()).Is("{x");
			ExpectFalse(text.IsEmpty);
			ExpectTrue(text.IsNotEmpty);
			ExpectFalse(text.IsTemplate);
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