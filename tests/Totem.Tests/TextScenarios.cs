using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Xunit;

namespace Totem
{
	/// <summary>
	/// Scenarios involving the <see cref="Totem.Text"/> class
	/// </summary>
	/// <remarks>
	/// Totem's expectations are written in terms of <see cref="Totem.Text"/>; use native xUnit assertions here
	/// </remarks>
	public class TextScenarios : Scenarios
	{
		//
		// Values to strings
		//

		void NoneValue()
		{
			Assert.Equal("", Text.None.ToString());
			Assert.Equal("", Text.Of("").ToString());
			Assert.Equal("", Text.Of(null as string).ToString());
		}

		void CharValue()
		{
			Assert.Equal("x", Text.Of('x').ToString());
		}

		void StringValue()
		{
			Assert.Equal("x", Text.Of("x").ToString());
		}

		void WritableValue()
		{
			var writableValue = (IWritable) Text.Of("x");

			Assert.Equal("x", Text.Of(writableValue).ToString());
		}

		void ActionValue()
		{
			Assert.Equal("x", Text.Of(writer => writer.Write("x")).ToString());
		}

		void FuncValue()
		{
			Assert.Equal("x", Text.Of(() => "x").ToString());
		}

		void LineValue()
		{
			Assert.Equal(Environment.NewLine, Text.Line.ToString());
		}

		void TwoLinesValue()
		{
			Assert.Equal(Environment.NewLine + Environment.NewLine, Text.TwoLines.ToString());
		}

		//
		// Values to non-strings
		//

		void ValueToText()
		{
			var value = Text.Of("x");

			Assert.Same(value, value.ToText());
		}

		void ValueToBuilder()
		{
			Assert.Equal("x", Text.Of("x").ToBuilder().ToString());
		}

		void ValueToBuilderProvided()
		{
			var builder = new StringBuilder();

			Text.Of("x").ToBuilder(builder);

			Assert.Equal("x", builder.ToString());
		}

		void ValueToStream()
		{
			var stream = Text.Of("x").ToStream();

			Assert.True(stream.CanRead);
			Assert.True(stream.CanSeek);
			Assert.Equal(1, stream.Length);
			Assert.Equal(1, stream.Position);
			Assert.Equal("", new StreamReader(stream).ReadToEnd());
		}

		void ValueToStreamSeekBegin()
		{
			var stream = Text.Of("x").ToStream(seekBegin: true);

			Assert.Equal(1, stream.Length);
			Assert.Equal(0, stream.Position);
			Assert.Equal("x", new StreamReader(stream).ReadToEnd());
		}

		void ValueToStreamProvided()
		{
			var stream = new MemoryStream();

			Text.Of("x").ToStream(stream);

			Assert.Equal(1, stream.Length);
			Assert.Equal(1, stream.Position);
			Assert.Equal("", new StreamReader(stream).ReadToEnd());
		}

		void ValueToStreamProvidedSeekBegin()
		{
			var stream = new MemoryStream();

			Text.Of("x").ToStream(stream, seekBegin: true);

			Assert.Equal(1, stream.Length);
			Assert.Equal(0, stream.Position);
			Assert.Equal("x", new StreamReader(stream).ReadToEnd());
		}

		//
		// Conversions
		//

		void ConvertToString()
		{
			string value = Text.Of("x");

			Assert.Equal("x", value);
		}

		void ConvertFromChar()
		{
			Text value = 'x';

			Assert.Equal("x", value.ToString());
		}

		void ConvertFromString()
		{
			Text value = "x";

			Assert.Equal("x", value.ToString());
		}

		void ConvertFromAction()
		{
			Action<TextWriter> action = writer => writer.Write("x");

			Text value = action;

			Assert.Equal("x", value.ToString());
		}

		//
		// Writes
		//

		void WriteChar()
		{
			Assert.Equal("xy", Text.Of("x").Write('y').ToString());
		}

		void WriteString()
		{
			Assert.Equal("xy", Text.Of("x").Write("y").ToString());
		}

		void WriteWritable()
		{
			var writableValue = (IWritable) Text.Of("y");

			Assert.Equal("xy", Text.Of("x").Write(writableValue).ToString());
		}

		void WriteAction()
		{
			Assert.Equal("xy", Text.Of("x").Write(writer => writer.Write("y")).ToString());
		}

		void WriteFunc()
		{
			Assert.Equal("xy", Text.Of("x").Write(() => "y").ToString());
		}

		void WriteText()
		{
			Assert.Equal("xy", Text.Of("x").Write(Text.Of("y")).ToString());
		}

		void RepeatValue()
		{
			Assert.Equal("xx", Text.Of("x").Repeat(2).ToString());
		}

		//
		// Write operators
		//

		void AddChar()
		{
			var value = Text.Of("x") + 'y';

			Assert.Equal("xy", value.ToString());
		}

		void AddString()
		{
			var value = Text.Of("x") + "y";

			Assert.Equal("xy", value.ToString());
		}

		void AddWritable()
		{
			var value = Text.Of("x") + (IWritable) Text.Of("y");

			Assert.Equal("xy", value.ToString());
		}

		void AddAction()
		{
			Action<TextWriter> action = writer => writer.Write("y");

			var value = Text.Of("x") + action;

			Assert.Equal("xy", value.ToString());
		}

		void AddText()
		{
			var value = Text.Of("x") + Text.Of("y");

			Assert.Equal("xy", value.ToString());
		}

		//
		// Write to none
		//

		void NoneWriteChar()
		{
			Assert.Equal("y", Text.None.Write('y').ToString());
		}

		void NoneWriteString()
		{
			Assert.Equal("y", Text.None.Write("y").ToString());
		}

		void NoneWriteWritable()
		{
			var writableValue = (IWritable) Text.Of("y");

			Assert.Equal("y", Text.None.Write(writableValue).ToString());
		}

		void NoneWriteAction()
		{
			Assert.Equal("y", Text.None.Write(writer => writer.Write("y")).ToString());
		}

		void NoneWriteFunc()
		{
			Assert.Equal("y", Text.None.Write(() => "y").ToString());
		}

		void NoneWriteText()
		{
			Assert.Equal("y", Text.None.Write(Text.Of("y")).ToString());
		}

		void RepeatNone()
		{
			Assert.Same(Text.None, Text.None.Repeat(2));
		}

		//
		// Many values to strings
		//

		void ManyChars()
		{
			var value = Text.OfMany(new[] { 'x', 'y' });

			Assert.Equal("xy", value.ToString());
		}

		void ManyStrings()
		{
			var value = Text.OfMany(new[] { "x", "y" });

			Assert.Equal("xy", value.ToString());
		}

		void ManyWritables()
		{
			var value = Text.OfMany(new[]
			{
				(IWritable) Text.Of("x"),
				(IWritable) Text.Of("y")
			});

			Assert.Equal("xy", value.ToString());
		}

		void ManyActions()
		{
			var value = Text.OfMany(new Action<TextWriter>[]
			{
				writer => writer.Write("x"),
				writer => writer.Write("y")
			});

			Assert.Equal("xy", value.ToString());
		}

		void ManyTexts()
		{
			var value = Text.OfMany(new Text[] { "x", "y" });

			Assert.Equal("xy", value.ToString());
		}

		void ManyValues()
		{
			var value = Text.OfMany(new[] { 8, 9 });

			Assert.Equal("89", value.ToString());
		}

		void ManyValuesSelect()
		{
			var value = Text.OfMany(
				new[] { 8, 9 },
				item => item.ToString() + "x");

			Assert.Equal("8x9x", value.ToString());
		}

		void ManyValuesSelectIndex()
		{
			var value = Text.OfMany(
				new[] { 8, 9 },
				(item, index) => item.ToString() + "x" + index.ToString());

			Assert.Equal("8x09x1", value.ToString());
		}

		//
		// If
		//

		void IfTrue()
		{
			Assert.Equal("x", Text.If(true, "x").ToString());
		}

		void IfFalse()
		{
			Assert.Equal("", Text.If(false, "x").ToString());
		}

		void IfTrueDeferred()
		{
			Assert.Equal("x", Text.If(() => true, "x").ToString());
		}

		void IfFalseDeferred()
		{
			Assert.Equal("", Text.If(() => false, "x").ToString());
		}

		//
		// SeparatedBy
		//

		void SeparatedBy()
		{
			var value = Text.SeparatedBy(",", new Text[] { "x", "y" });

			Assert.Equal("x,y", value.ToString());
		}

		void SeparatedBySelect()
		{
			var value = Text.SeparatedBy(
				",",
				new Text[] { "x", "y" },
				item => item + "z");

			Assert.Equal("xz,yz", value.ToString());
		}

		void SeparatedBySelectIndex()
		{
			var value = Text.SeparatedBy(
				",",
				new Text[] { "x", "y" },
				(item, index) => item + index.ToString());

			Assert.Equal("x0,y1", value.ToString());
		}

		void ValuesSeparatedBy()
		{
			var value = Text.SeparatedBy(",", new[] { 8, 9 });

			Assert.Equal("8,9", value.ToString());
		}

		void ValuesSeparatedBySelect()
		{
			var value = Text.SeparatedBy(
				",",
				new[] { 8, 9 },
				item => item.ToString() + "x");

			Assert.Equal("8x,9x", value.ToString());
		}

		void ValuesSeparatedBySelectIndex()
		{
			var value = Text.SeparatedBy(
				",",
				new[] { 8, 9 },
				(item, index) => item.ToString() + "x" + index.ToString());

			Assert.Equal("8x0,9x1", value.ToString());
		}

		//
		// Types
		//

		void TypeValue()
		{
			Assert.Equal("Totem.TextScenarios", Text.OfType(this).ToString());
		}

		void TypeValueGeneric()
		{
			Assert.Equal("Totem.TextScenarios", Text.OfType<TextScenarios>().ToString());
		}
	}
}