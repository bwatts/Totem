using System;
using System.Collections.Generic;
using System.Linq;

namespace Totem
{
	/// <summary>
	/// Scenarios involving the <see cref="Totem.LinkText"/> class
	/// </summary>
	public class LinkPathSpecs : Specs
  {
		//
		// Root
		//

		void Root()
		{
			Expect(LinkPath.Root.Segments.Count).Is(0);
			Expect(LinkPath.Root.IsTemplate).IsFalse();
			Expect(LinkPath.Root.ToString()).Is("");
		}

		void RootTextSeparator()
		{
			var path = LinkPath.Root.ToText(separator: ".");

			Expect(path.ToString()).Is("");
		}

		void RootTextLeading()
		{
			var path = LinkPath.Root.ToText(leading: true);

			Expect(path.ToString()).Is("/");
		}

		void RootTextTrailing()
		{
			var path = LinkPath.Root.ToText(trailing: true);

			Expect(path.ToString()).Is("/");
		}

		void RootTextLeadingTrailing()
		{
			var path = LinkPath.Root.ToText(leading: true, trailing: true);

			Expect(path.ToString()).Is("/");
		}

		//
		// Segment
		//

		void Segment()
		{
			var path = LinkPath.From("x");

			Expect(path.ToString()).Is("x");
			Expect(path.Segments.Count).Is(1);
			Expect(path.Segments[0].ToString()).Is("x");
		}

		void SegmentSeparator()
		{
			var path = LinkPath.From("x").ToText(separator: ".");

			Expect(path.ToString()).Is("x");
		}

		void SegmentLeading()
		{
			var path = LinkPath.From("x").ToText(leading: true);

			Expect(path.ToString()).Is("/x");
		}

		void SegmentTrailing()
		{
			var path = LinkPath.From("x").ToText(trailing: true);

			Expect(path.ToString()).Is("x/");
		}

		void SegmentLeadingTrailing()
		{
			var path = LinkPath.From("x").ToText(leading: true, trailing: true);

			Expect(path.ToString()).Is("/x/");
		}

		void SegmentTemplate()
		{
			var path = LinkPath.From("{x}");

			Expect(path.IsTemplate).IsTrue();
			Expect(path.Segments[0].IsTemplate).IsTrue();
		}

		//
		// Segments
		//

		void Segments()
		{
			var path = LinkPath.From("x", "y");

			Expect(path.ToString()).Is("x/y");
			Expect(path.Segments.Count).Is(2);
			Expect(path.Segments[0].ToString()).Is("x");
			Expect(path.Segments[1].ToString()).Is("y");
		}

		void SegmentsSeparator()
		{
			var path = LinkPath.From("x", "y").ToText(separator: ".");

			Expect(path.ToString()).Is("x.y");
		}

		void SegmentsLeading()
		{
			var path = LinkPath.From("x", "y").ToText(leading: true);

			Expect(path.ToString()).Is("/x/y");
		}

		void SegmentsTrailing()
		{
			var path = LinkPath.From("x", "y").ToText(trailing: true);

			Expect(path.ToString()).Is("x/y/");
		}

		void SegmentsLeadingTrailing()
		{
			var path = LinkPath.From("x", "y").ToText(leading: true, trailing: true);

			Expect(path.ToString()).Is("/x/y/");
		}

		void SegmentsTemplate()
		{
			var path = LinkPath.From("{x}", "y");

			Expect(path.IsTemplate).IsTrue();
			Expect(path.Segments[0].IsTemplate).IsTrue();
			Expect(path.Segments[1].IsTemplate).IsFalse();
		}
	}
}