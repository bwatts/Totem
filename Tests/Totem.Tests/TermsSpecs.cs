using System;
using System.Collections.Generic;
using System.Linq;

namespace Totem
{
	/// <summary>
	/// Scenarios involving the <see cref="Totem.Terms"/> class
	/// </summary>
	public class TermsSpecs : Specs
  {
		//
		// None
		//

		void None()
		{
			Expect(Terms.None.IsNone);
			ExpectNot(Terms.None.IsNotNone);
			Expect(Terms.None.Count).Is(0);
		}

		void NoneToText()
		{
			Expect(Terms.None.ToString()).IsEmpty();
		}

		void NoneIndex()
		{
			ExpectThrows<IndexOutOfRangeException>(() => Terms.None[0]);
		}

		//
		// Single
		//

		void Single()
		{
			var terms = Terms.From("x");

			ExpectNot(terms.IsNone);
			Expect(terms.IsNotNone);
			Expect(terms.Count).Is(1);
		}

		void SingleToText()
		{
			var terms = Terms.From("x");

			Expect(terms.ToString()).Is("x");
		}

		void SingleIndex()
		{
			var terms = Terms.From("x");

			Expect(terms[0]).Is("x");
			ExpectThrows<IndexOutOfRangeException>(() => terms[1]);
		}

		//
		// Multiple
		//

		void Multiple()
		{
			var terms = Terms.From("x", "y");

			ExpectNot(terms.IsNone);
			Expect(terms.IsNotNone);
			Expect(terms.Count).Is(2);
		}

		void MultipleToText()
		{
			var terms = Terms.From("x", "y");

			Expect(terms.ToString()).Is("x y");
		}

		void MultipleIndex()
		{
			var terms = Terms.From("x", "y");

			Expect(terms[0]).Is("x");
			Expect(terms[1]).Is("y");
			ExpectThrows<IndexOutOfRangeException>(() => terms[2]);
		}
	}
}