using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;

namespace Totem
{
	/// <summary>
	/// Extends <see cref="Totem.Text"/> with core operations
	/// </summary>
	[EditorBrowsable(EditorBrowsableState.Never)]
	public static class TextOperations
	{
		public static Text If(this Text text, bool condition, Text whenFalse)
		{
			return Text.If(condition, text, whenFalse);
		}

		public static Text If(this Text text, Func<bool> condition, Text whenFalse)
		{
			return Text.If(condition, text, whenFalse);
		}

		//
		// Write
		//

		public static Text Write(this Text text, bool value)
		{
			return text + Text.Of(value);
		}

		public static Text Write(this Text text, char[] value)
		{
			return text + Text.Of(value);
		}

		public static Text Write(this Text text, char[] value, int index, int count)
		{
			return text + Text.Of(value, index, count);
		}

		public static Text Write(this Text text, decimal value)
		{
			return text + Text.Of(value);
		}

		public static Text Write(this Text text, double value)
		{
			return text + Text.Of(value);
		}

		public static Text Write(this Text text, float value)
		{
			return text + Text.Of(value);
		}

		public static Text Write(this Text text, int value)
		{
			return text + Text.Of(value);
		}

		public static Text Write(this Text text, long value)
		{
			return text + Text.Of(value);
		}

		public static Text Write(this Text text, object value)
		{
			return text + Text.Of(value);
		}

		public static Text Write(this Text text, uint value)
		{
			return text + Text.Of(value);
		}

		public static Text Write(this Text text, ulong value)
		{
			return text + Text.Of(value);
		}

		public static Text Write(this Text text, string format, object arg0)
		{
			return text + Text.Of(format, arg0);
		}

		public static Text Write(this Text text, string format, object arg0, object arg1)
		{
			return text + Text.Of(format, arg0, arg1);
		}

		public static Text Write(this Text text, string format, object arg0, object arg1, object arg2)
		{
			return text + Text.Of(format, arg0, arg1, arg2);
		}

		public static Text Write(this Text text, string format, params object[] args)
		{
			return text + Text.Of(format, args);
		}

		//
		// WriteMany
		//

		public static Text WriteMany<T>(this Text text, IEnumerable<T> values, Func<T, Text> selectText)
		{
			return text + Text.OfMany(values, selectText);
		}

		public static Text WriteMany<T>(this Text text, IEnumerable<T> values, Func<T, int, Text> selectText)
		{
			return text + Text.OfMany(values, selectText);
		}

		public static Text WriteMany<T>(this Text text, IEnumerable<T> values)
		{
			return text + Text.OfMany(values);
		}

		public static Text WriteMany<T>(this Text text, params T[] values)
		{
			return text + Text.OfMany(values);
		}

		//
		// WriteLine
		//

		public static Text WriteLine(this Text text)
		{
			return text.Write(writer => writer.WriteLine());
		}

		public static Text WriteLine(this Text text, bool value)
		{
			return text.Write(writer => writer.WriteLine(value));
		}

		public static Text WriteLine(this Text text, char value)
		{
			return text.Write(writer => writer.WriteLine(value));
		}

		public static Text WriteLine(this Text text, char[] value)
		{
			return text.Write(writer => writer.WriteLine(value));
		}

		public static Text WriteLine(this Text text, char[] value, int index, int count)
		{
			return text.Write(writer => writer.WriteLine(value, index, count));
		}

		public static Text WriteLine(this Text text, decimal value)
		{
			return text.Write(writer => writer.WriteLine(value));
		}

		public static Text WriteLine(this Text text, double value)
		{
			return text.Write(writer => writer.WriteLine(value));
		}

		public static Text WriteLine(this Text text, float value)
		{
			return text.Write(writer => writer.WriteLine(value));
		}

		public static Text WriteLine(this Text text, int value)
		{
			return text.Write(writer => writer.WriteLine(value));
		}

		public static Text WriteLine(this Text text, long value)
		{
			return text.Write(writer => writer.WriteLine(value));
		}

		public static Text WriteLine(this Text text, object value)
		{
			return text.Write(writer => writer.WriteLine(value));
		}

		public static Text WriteLine(this Text text, string value)
		{
			return text.Write(writer => writer.WriteLine(value));
		}

		public static Text WriteLine(this Text text, uint value)
		{
			return text.Write(writer => writer.WriteLine(value));
		}

		public static Text WriteLine(this Text text, ulong value)
		{
			return text.Write(writer => writer.WriteLine(value));
		}

		public static Text WriteLine(this Text text, string format, object arg0)
		{
			return text.Write(writer => writer.WriteLine(format, arg0));
		}

		public static Text WriteLine(this Text text, string format, object arg0, object arg1)
		{
			return text.Write(writer => writer.WriteLine(format, arg0, arg1));
		}

		public static Text WriteLine(this Text text, string format, object arg0, object arg1, object arg2)
		{
			return text.Write(writer => writer.WriteLine(format, arg0, arg1, arg2));
		}

		public static Text WriteLine(this Text text, string format, params object[] args)
		{
			return text.Write(writer => writer.WriteLine(format, args));
		}

		//
		// WriteLines
		//

		public static Text WriteLines(this Text text, int count)
		{
			return text + Text.Line.Repeat(count);
		}

		public static Text WriteTwoLines(this Text text)
		{
			return text + Text.TwoLines;
		}

		public static Text WriteLines(this Text text, IEnumerable<Text> values, Func<Text, Text> selectText)
		{
			return text.WriteSeparatedBy(Text.Line, values, selectText);
		}

		public static Text WriteLines(this Text text, IEnumerable<Text> values, Func<Text, int, Text> selectText)
		{
			return text.WriteSeparatedBy(Text.Line, values, selectText);
		}

		public static Text WriteLines(this Text text, IEnumerable<Text> values)
		{
			return text.WriteSeparatedBy(Text.Line, values);
		}

		public static Text WriteLines(this Text text, params Text[] values)
		{
			return text.WriteSeparatedBy(Text.Line, values);
		}

		public static Text WriteLines<T>(this Text text, IEnumerable<T> values, Func<T, Text> selectText)
		{
			return text.WriteSeparatedBy(Text.Line, values, selectText);
		}

		public static Text WriteLines<T>(this Text text, IEnumerable<T> values, Func<T, int, Text> selectText)
		{
			return text.WriteSeparatedBy(Text.Line, values, selectText);
		}

		public static Text WriteLines<T>(this Text text, IEnumerable<T> values)
		{
			return text.WriteSeparatedBy(Text.Line, values);
		}

		public static Text WriteLines<T>(this Text text, params T[] values)
		{
			return text.WriteSeparatedBy(Text.Line, values);
		}

		//
		// WriteSeparatedBy
		//

		public static Text WriteSeparatedBy(this Text text, Text separator, IEnumerable<Text> values, Func<Text, Text> selectText)
		{
			return text + values.ToTextSeparatedBy(separator, selectText);
		}

		public static Text WriteSeparatedBy(this Text text, Text separator, IEnumerable<Text> values, Func<Text, int, Text> selectText)
		{
			return text + values.ToTextSeparatedBy(separator, selectText);
		}

		public static Text WriteSeparatedBy(this Text text, Text separator, IEnumerable<Text> values)
		{
			return text + values.ToTextSeparatedBy(separator);
		}

		public static Text WriteSeparatedBy(this Text text, Text separator, params Text[] values)
		{
			return text + values.ToTextSeparatedBy(separator);
		}

		public static Text WriteSeparatedBy<T>(this Text text, Text separator, IEnumerable<T> values, Func<T, Text> selectText)
		{
			return text + values.ToTextSeparatedBy(separator, selectText);
		}

		public static Text WriteSeparatedBy<T>(this Text text, Text separator, IEnumerable<T> values, Func<T, int, Text> selectText)
		{
			return text + values.ToTextSeparatedBy(separator, selectText);
		}

		public static Text WriteSeparatedBy<T>(this Text text, Text separator, IEnumerable<T> values)
		{
			return text + values.ToTextSeparatedBy(separator);
		}

		public static Text WriteSeparatedBy<T>(this Text text, Text separator, params T[] values)
		{
			return text + values.ToTextSeparatedBy(separator);
		}

		//
		// In
		//

		public static Text InParentheses(this Text value)
		{
			return Text.Of("({0})", value);
		}

		public static Text InBraces(this Text value)
		{
			return Text.Of("{{{0}}}", value);
		}

		public static Text InBrackets(this Text value)
		{
			return Text.Of("[{0}]", value);
		}

		public static Text InAngleBrackets(this Text value)
		{
			return Text.Of("<{0}>", value);
		}

		public static Text InSingleQuotes(this Text value)
		{
			return Text.Of("'{0}'", value);
		}

		public static Text InDoubleQuotes(this Text value)
		{
			return Text.Of("\"{0}\"", value);
		}

		public static Text WriteInParentheses(this Text text, Text value)
		{
			return text + value.InParentheses();
		}

		public static Text WriteInBraces(this Text text, Text value)
		{
			return text + value.InBraces();
		}

		public static Text WriteInBrackets(this Text text, Text value)
		{
			return text + value.InBrackets();
		}

		public static Text WriteInAngleBrackets(this Text text, Text value)
		{
			return text + value.InAngleBrackets();
		}

		public static Text WriteInSingleQuotes(this Text text, Text value)
		{
			return text + value.InSingleQuotes();
		}

		public static Text WriteInDoubleQuotes(this Text text, Text value)
		{
			return text + value.InDoubleQuotes();
		}

		//
		// ToText
		//

		public static Text ToText(this string value)
		{
			return value;
		}

		public static Text ToText(this IEnumerable<string> values, Func<string, Text> selectText)
		{
			return Text.OfMany(values, selectText);
		}

		public static Text ToText(this IEnumerable<string> values, Func<string, int, Text> selectText)
		{
			return Text.OfMany(values, selectText);
		}

		public static Text ToText(this IEnumerable<string> values)
		{
			return Text.OfMany(values);
		}

		public static Text ToText(this IEnumerable<Text> values, Func<Text, Text> selectText)
		{
			return Text.OfMany(values, selectText);
		}

		public static Text ToText(this IEnumerable<Text> values, Func<Text, int, Text> selectText)
		{
			return Text.OfMany(values, selectText);
		}

		public static Text ToText(this IEnumerable<Text> values)
		{
			return Text.OfMany(values);
		}

		public static Text ToText<T>(this IEnumerable<T> values, Func<T, Text> selectText)
		{
			return Text.OfMany(values, selectText);
		}

		public static Text ToText<T>(this IEnumerable<T> values, Func<T, int, Text> selectText)
		{
			return Text.OfMany(values, selectText);
		}

		public static Text ToText<T>(this IEnumerable<T> values)
		{
			return Text.OfMany(values);
		}

		//
		// ToTextValues
		//

		public static IEnumerable<Text> ToTextValues(this IEnumerable<string> values, Func<string, Text> selectText)
		{
			return values.Select(selectText);
		}

		public static IEnumerable<Text> ToTextValues(this IEnumerable<string> values, Func<string, int, Text> selectText)
		{
			return values.Select(selectText);
		}

		public static IEnumerable<Text> ToTextValues(this IEnumerable<string> values)
		{
			return values.Select(value => Text.Of(value));
		}

		public static IEnumerable<Text> ToTextValues<T>(this IEnumerable<T> values, Func<T, Text> selectText)
		{
			return values.Select(selectText);
		}

		public static IEnumerable<Text> ToTextValues<T>(this IEnumerable<T> values, Func<T, int, Text> selectText)
		{
			return values.Select(selectText);
		}

		public static IEnumerable<Text> ToTextValues<T>(this IEnumerable<T> values)
		{
			return values.Select(value => Text.Of(value));
		}

		//
		// ToTextSeparatedBy
		//

		public static Text ToTextSeparatedBy(this IEnumerable<Text> values, Text separator, Func<Text, Text> selectText)
		{
			return Text.SeparatedBy(separator, values, selectText);
		}

		public static Text ToTextSeparatedBy(this IEnumerable<Text> values, Text separator, Func<Text, int, Text> selectText)
		{
			return Text.SeparatedBy(separator, values, selectText);
		}

		public static Text ToTextSeparatedBy(this IEnumerable<Text> values, Text separator)
		{
			return Text.SeparatedBy(separator, values);
		}

		public static Text ToTextSeparatedBy<T>(this IEnumerable<T> values, Text separator, Func<T, Text> selectText)
		{
			return Text.SeparatedBy(separator, values, selectText);
		}

		public static Text ToTextSeparatedBy<T>(this IEnumerable<T> values, Text separator, Func<T, int, Text> selectText)
		{
			return Text.SeparatedBy(separator, values, selectText);
		}

		public static Text ToTextSeparatedBy<T>(this IEnumerable<T> values, Text separator)
		{
			return Text.SeparatedBy(separator, values);
		}

		//
		// Conditions
		//

		public static Text WriteIf(this Text text, bool condition, Text whenTrue, Text whenFalse)
		{
			return text + Text.If(condition, whenTrue, whenFalse);
		}

		public static Text WriteIf(this Text text, bool condition, Text whenTrue)
		{
			return text + Text.If(condition, whenTrue);
		}

		public static Text WriteIf(this Text text, Func<bool> condition, Text whenTrue, Text whenFalse)
		{
			return text + Text.If(condition, whenTrue, whenFalse);
		}

		public static Text WriteIf(this Text text, Func<bool> condition, Text whenTrue)
		{
			return text + Text.If(condition, whenTrue);
		}

		//
		// Counts (int)
		//

		public static Text WriteSingularOrPlural(this Text text, int count, Text singular, Text plural)
		{
			return text + Text.SingularOrPlural(count, singular, plural);
		}

		public static Text WriteSingularOrPlural(this Text text, int count, Text singular)
		{
			return text + Text.SingularOrPlural(count, singular);
		}

		public static Text WriteCount(this Text text, int count, Text singular, Text plural)
		{
			return text + Text.Count(count, singular, plural);
		}

		public static Text WriteCount(this Text text, int count, Text singular)
		{
			return text + Text.Count(count, singular);
		}

		//
		// Counts (long)
		//

		public static Text WriteSingularOrPlural(this Text text, long count, Text singular, Text plural)
		{
			return text + Text.SingularOrPlural(count, singular, plural);
		}

		public static Text WriteSingularOrPlural(this Text text, long count, Text singular)
		{
			return text + Text.SingularOrPlural(count, singular);
		}

		public static Text WriteCount(this Text text, long count, Text singular, Text plural)
		{
			return text + Text.Count(count, singular, plural);
		}

		public static Text WriteCount(this Text text, long count, Text singular)
		{
			return text + Text.Count(count, singular);
		}

		public static Text Repeat(this Text value, int count)
		{
			return Text.Of(writer =>
			{
				for(var i = 0; i < count; i++)
				{
					value.ToWriter(writer);
				}
			});
		}

		//
		// Indentation
		//

		public static Text Indent(
			this Text text,
			int level = 1,
			string indent = Text.FourSpaceIndent,
			bool retainIndent = false,
			bool retainWhitespace = false,
			string linePrefix = "")
		{
			return Text.Of(writer =>
			{
				var wroteFirst = false;

				foreach(var lineText in text.SplitLines(includeEmptyValues: true))
				{
					if(wroteFirst)
					{
						writer.WriteLine();
					}
					else
					{
						wroteFirst = true;
					}

					var line = lineText.ToString();

					var lineLevel = 0;

					while(line.Length > 0 && line.StartsWith(indent))
					{
						line = line.Substring(indent.Length);

						lineLevel++;
					}

					if(!retainWhitespace)
					{
						line = line.TrimStart();
					}

					writer.Write(linePrefix);

					var effectiveLevel = retainIndent ? level + lineLevel : level;

					for(var i = 0; i < effectiveLevel; i++)
					{
						writer.Write(indent);
					}

					writer.Write(line);
				}
			});
		}

		public static Text IndentWithTabs(this Text text, int level = 1, bool retainIndent = true, bool retainWhitespace = true)
		{
			return text.Indent(level, "\t", retainIndent, retainWhitespace);
		}

		public static Text IndentWithSpaces(this Text text, int level = 1, int tabSize = 2, bool retainIndent = true, bool retainWhitespace = true)
		{
			return text.Indent(level, new string(' ', level * tabSize), retainIndent, retainWhitespace);
		}

		public static Text Scoped(
			this Text text,
			string startToken = "{",
			string endToken = "}",
			int currentIndent = 0,
			string indent = Text.FourSpaceIndent,
			bool retainIndent = true,
			bool retainWhitespace = true,
			string linePrefix = "")
		{
			return Text.None
				.WriteLine(startToken)
				.WriteIndented(text, currentIndent + 1, indent, retainIndent, retainWhitespace, linePrefix)
				.WriteLine()
				.Write(endToken);
		}

		public static Text WriteIndented(
			this Text text,
			Text indentedText,
			int level = 1,
			string indent = Text.FourSpaceIndent,
			bool retainIndent = false,
			bool retainWhitespace = false,
			string linePrefix = "")
		{
			return text + indentedText.Indent(level, indent, retainIndent, retainWhitespace, linePrefix);
		}

		public static Text WriteIndentedWithTabs(this Text text, Text indentedText, int level = 1, bool retainIndent = true, bool retainWhitespace = true)
		{
			return text + indentedText.IndentWithTabs(level, retainIndent, retainWhitespace);
		}

		public static Text WriteIndentedWithSpaces(this Text text, Text indentedText, int level = 1, int tabSize = 2, bool retainIndent = true, bool retainWhitespace = true)
		{
			return text + indentedText.IndentWithSpaces(level, tabSize, retainIndent, retainWhitespace);
		}

		public static Text WriteTabs(this Text text, int count)
		{
			return text + new string('\t', count);
		}

		public static Text WriteSpaces(this Text text, int count)
		{
			return text + new string(' ', count);
		}

		public static Text WriteSpaceTabs(this Text text, int count, int size = 2)
		{
			return text + new string(' ', count * size);
		}

		public static Text WriteScoped(
			this Text text,
			Text body,
			string startToken = "{",
			string endToken = "}",
			int currentIndent = 0,
			string indent = Text.FourSpaceIndent,
			bool retainIndent = true,
			bool retainWhitespace = true,
			string linePrefix = "")
		{
			return text + body.Scoped(startToken, endToken, currentIndent, indent, retainIndent, retainWhitespace, linePrefix);
		}

		//
		// Split
		//

		public static IEnumerable<string> Split(this Text text, Text separator, int maximumSubstrings = int.MaxValue, bool includeEmptyValues = false)
		{
			return text.ToString().Split(
				new string[] { separator },
				maximumSubstrings,
				includeEmptyValues ? StringSplitOptions.None : StringSplitOptions.RemoveEmptyEntries);
		}

		public static IEnumerable<string> SplitLines(this Text text, int maximumLineLength = int.MaxValue, int maximumSubstrings = int.MaxValue, bool includeEmptyValues = false)
		{
			foreach(var lineText in text.Split(Text.Line, maximumSubstrings, includeEmptyValues))
			{
				var line = lineText.ToString();

				while(line.Length > maximumLineLength)
				{
					yield return line.Substring(0, maximumLineLength);

					line = line.Substring(maximumLineLength);
				}

				yield return line;
			}
		}

		public static IEnumerable<string> SplitSpaces(this Text text, int maximumSubstrings = int.MaxValue, bool includeEmptyValues = false)
		{
			return text.Split(' ', maximumSubstrings, includeEmptyValues);
		}

		public static IEnumerable<string> SplitCommas(this Text text, int maximumSubstrings = int.MaxValue, bool includeEmptyValues = false)
		{
			return text.Split(',', maximumSubstrings, includeEmptyValues);
		}

		//
		// Compact
		//

		public static Text Compact(this Text text)
		{
			return Text.Of(() =>
			{
				var source = text.ToString();

				if(source.Length < 3)
				{
					return source;
				}

				return source.ToText().Compact(source.Length / 2);
			});
		}

		public static Text Compact(this Text text, int maxLength)
		{
			return Text.Of(() =>
			{
				var source = text.ToString();

				if(source.Length <= maxLength)
				{
					return source;
				}

				var partLength = maxLength / 2;

				var leftPart = source.Substring(0, partLength);
				var rightPart = source.Substring(source.Length - partLength + 1);

				return leftPart + Text.Ellipsis + rightPart;
			});
		}

		public static Text CompactLeft(this Text text, int maxLength)
		{
			return Text.Of(() =>
			{
				var source = text.ToString();

				if(source.Length <= maxLength)
				{
					return source;
				}

				var partLength = maxLength / 2;

				var rightPart = source.Substring(source.Length - partLength + 1);

				return Text.Ellipsis + rightPart;
			});
		}

		public static Text CompactRight(this Text text, int maxLength)
		{
			return Text.Of(() =>
			{
				var source = text.ToString();

				if(source.Length <= maxLength)
				{
					return source;
				}

				var partLength = maxLength / 2;

				var leftPart = source.Substring(0, partLength - 1);

				return leftPart + Text.Ellipsis;
			});
		}

		public static Text WriteCompacted(this Text text, Text value)
		{
			return text + value.Compact();
		}

		public static Text WriteCompacted(this Text text, Text value, int maxLength)
		{
			return text + value.Compact(maxLength);
		}

		public static Text WriteCompactedLeft(this Text text, Text value, int maxLength)
		{
			return text + value.CompactLeft(maxLength);
		}

		public static Text WriteCompactedRight(this Text text, Text value, int maxLength)
		{
			return text + value.CompactRight(maxLength);
		}
	}
}