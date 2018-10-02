using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;

namespace Totem
{
  /// <summary>
  /// Extends <see cref="Totem.Text"/> with core operations
  /// </summary>
  [EditorBrowsable(EditorBrowsableState.Never)]
  public static class TextExtensions
  {
    public static Text If(this Text text, bool condition, Text whenFalse) =>
      Text.If(condition, text, whenFalse);

    public static Text If(this Text text, Func<bool> condition, Text whenFalse) =>
      Text.If(condition, text, whenFalse);

    //
    // Write
    //

    public static Text Write(this Text text, bool value) =>
      text + Text.Of(value);

    public static Text Write(this Text text, char[] value) =>
      text + Text.Of(value);

    public static Text Write(this Text text, char[] value, int index, int count) =>
      text + Text.Of(value, index, count);

    public static Text Write(this Text text, decimal value) =>
      text + Text.Of(value);

    public static Text Write(this Text text, double value) =>
      text + Text.Of(value);

    public static Text Write(this Text text, float value) =>
      text + Text.Of(value);

    public static Text Write(this Text text, int value) =>
      text + Text.Of(value);

    public static Text Write(this Text text, long value) =>
      text + Text.Of(value);

    public static Text Write(this Text text, object value) =>
      text + Text.Of(value);

    public static Text Write(this Text text, uint value) =>
      text + Text.Of(value);

    public static Text Write(this Text text, ulong value) =>
      text + Text.Of(value);

    public static Text Write(this Text text, string format, object arg0) =>
      text + Text.Of(format, arg0);

    public static Text Write(this Text text, string format, object arg0, object arg1) =>
      text + Text.Of(format, arg0, arg1);

    public static Text Write(this Text text, string format, object arg0, object arg1, object arg2) =>
      text + Text.Of(format, arg0, arg1, arg2);

    public static Text Write(this Text text, string format, params object[] args) =>
      text + Text.Of(format, args);

    //
    // WriteMany
    //

    public static Text WriteMany<T>(this Text text, IEnumerable<T> values, Func<T, Text> selectText) =>
      text + Text.OfMany(values, selectText);

    public static Text WriteMany<T>(this Text text, IEnumerable<T> values, Func<T, int, Text> selectText) =>
      text + Text.OfMany(values, selectText);

    public static Text WriteMany<T>(this Text text, IEnumerable<T> values) =>
      text + Text.OfMany(values);

    public static Text WriteMany<T>(this Text text, params T[] values) =>
      text + Text.OfMany(values);

    //
    // WriteLine
    //

    public static Text WriteLine(this Text text) =>
      text.Write(writer => writer.WriteLine());

    public static Text WriteLine(this Text text, bool value) =>
      text.Write(writer => writer.WriteLine(value));

    public static Text WriteLine(this Text text, char value) =>
      text.Write(writer => writer.WriteLine(value));

    public static Text WriteLine(this Text text, char[] value) =>
      text.Write(writer => writer.WriteLine(value));

    public static Text WriteLine(this Text text, char[] value, int index, int count) =>
      text.Write(writer => writer.WriteLine(value, index, count));

    public static Text WriteLine(this Text text, decimal value) =>
      text.Write(writer => writer.WriteLine(value));

    public static Text WriteLine(this Text text, double value) =>
      text.Write(writer => writer.WriteLine(value));

    public static Text WriteLine(this Text text, float value) =>
      text.Write(writer => writer.WriteLine(value));

    public static Text WriteLine(this Text text, int value) =>
      text.Write(writer => writer.WriteLine(value));

    public static Text WriteLine(this Text text, long value) =>
      text.Write(writer => writer.WriteLine(value));

    public static Text WriteLine(this Text text, object value) =>
      text.Write(writer => writer.WriteLine(value));

    public static Text WriteLine(this Text text, string value) =>
      text.Write(writer => writer.WriteLine(value));

    public static Text WriteLine(this Text text, uint value) =>
      text.Write(writer => writer.WriteLine(value));

    public static Text WriteLine(this Text text, ulong value) =>
      text.Write(writer => writer.WriteLine(value));

    public static Text WriteLine(this Text text, string format, object arg0) =>
      text.Write(writer => writer.WriteLine(format, arg0));

    public static Text WriteLine(this Text text, string format, object arg0, object arg1) =>
      text.Write(writer => writer.WriteLine(format, arg0, arg1));

    public static Text WriteLine(this Text text, string format, object arg0, object arg1, object arg2) =>
      text.Write(writer => writer.WriteLine(format, arg0, arg1, arg2));

    public static Text WriteLine(this Text text, string format, params object[] args) =>
      text.Write(writer => writer.WriteLine(format, args));

    //
    // WriteLines
    //

    public static Text WriteLines(this Text text, int count) =>
      text + Text.Line.Repeat(count);

    public static Text WriteTwoLines(this Text text) =>
      text + Text.TwoLines;

    public static Text WriteLines(this Text text, IEnumerable<Text> values, Func<Text, Text> selectText) =>
      text.WriteSeparatedBy(Text.Line, values, selectText);

    public static Text WriteLines(this Text text, IEnumerable<Text> values, Func<Text, int, Text> selectText) =>
      text.WriteSeparatedBy(Text.Line, values, selectText);

    public static Text WriteLines(this Text text, IEnumerable<Text> values) =>
      text.WriteSeparatedBy(Text.Line, values);

    public static Text WriteLines(this Text text, params Text[] values) =>
      text.WriteSeparatedBy(Text.Line, values);

    public static Text WriteLines<T>(this Text text, IEnumerable<T> values, Func<T, Text> selectText) =>
      text.WriteSeparatedBy(Text.Line, values, selectText);

    public static Text WriteLines<T>(this Text text, IEnumerable<T> values, Func<T, int, Text> selectText) =>
      text.WriteSeparatedBy(Text.Line, values, selectText);

    public static Text WriteLines<T>(this Text text, IEnumerable<T> values) =>
      text.WriteSeparatedBy(Text.Line, values);

    public static Text WriteLines<T>(this Text text, params T[] values) =>
      text.WriteSeparatedBy(Text.Line, values);

    //
    // WriteSeparatedBy
    //

    public static Text WriteSeparatedBy(this Text text, Text separator, IEnumerable<Text> values, Func<Text, Text> selectText) =>
      text + values.ToTextSeparatedBy(separator, selectText);

    public static Text WriteSeparatedBy(this Text text, Text separator, IEnumerable<Text> values, Func<Text, int, Text> selectText) =>
      text + values.ToTextSeparatedBy(separator, selectText);

    public static Text WriteSeparatedBy(this Text text, Text separator, IEnumerable<Text> values) =>
      text + values.ToTextSeparatedBy(separator);

    public static Text WriteSeparatedBy(this Text text, Text separator, params Text[] values) =>
      text + values.ToTextSeparatedBy(separator);

    public static Text WriteSeparatedBy<T>(this Text text, Text separator, IEnumerable<T> values, Func<T, Text> selectText) =>
      text + values.ToTextSeparatedBy(separator, selectText);

    public static Text WriteSeparatedBy<T>(this Text text, Text separator, IEnumerable<T> values, Func<T, int, Text> selectText) =>
      text + values.ToTextSeparatedBy(separator, selectText);

    public static Text WriteSeparatedBy<T>(this Text text, Text separator, IEnumerable<T> values) =>
      text + values.ToTextSeparatedBy(separator);

    public static Text WriteSeparatedBy<T>(this Text text, Text separator, params T[] values) =>
      text + values.ToTextSeparatedBy(separator);

    //
    // In
    //

    public static Text InParentheses(this Text value) =>
      $"({value})";

    public static Text InBraces(this Text value) =>
      $"{{{value}}}";

    public static Text InBrackets(this Text value) =>
      $"[{value}]";

    public static Text InAngleBrackets(this Text value) =>
      $"<{value}>";

    public static Text InSingleQuotes(this Text value) =>
      $"'{value}'";

    public static Text InDoubleQuotes(this Text value) =>
      $"\"{value}\"";

    public static Text WriteInParentheses(this Text text, Text value) =>
      text + value.InParentheses();

    public static Text WriteInBraces(this Text text, Text value) =>
      text + value.InBraces();

    public static Text WriteInBrackets(this Text text, Text value) =>
      text + value.InBrackets();

    public static Text WriteInAngleBrackets(this Text text, Text value) =>
      text + value.InAngleBrackets();

    public static Text WriteInSingleQuotes(this Text text, Text value) =>
      text + value.InSingleQuotes();

    public static Text WriteInDoubleQuotes(this Text text, Text value) =>
      text + value.InDoubleQuotes();

    //
    // ToText
    //

    public static Text ToText(this string value) =>
      value;

    public static Text ToText(this IEnumerable<string> values, Func<string, Text> selectText) =>
      Text.OfMany(values, selectText);

    public static Text ToText(this IEnumerable<string> values, Func<string, int, Text> selectText) =>
      Text.OfMany(values, selectText);

    public static Text ToText(this IEnumerable<string> values) =>
      Text.OfMany(values);

    public static Text ToText(this IEnumerable<Text> values, Func<Text, Text> selectText) =>
      Text.OfMany(values, selectText);

    public static Text ToText(this IEnumerable<Text> values, Func<Text, int, Text> selectText) =>
      Text.OfMany(values, selectText);

    public static Text ToText(this IEnumerable<Text> values) =>
      Text.OfMany(values);

    public static Text ToText<T>(this IEnumerable<T> values, Func<T, Text> selectText) =>
      Text.OfMany(values, selectText);

    public static Text ToText<T>(this IEnumerable<T> values, Func<T, int, Text> selectText) =>
      Text.OfMany(values, selectText);

    public static Text ToText<T>(this IEnumerable<T> values) =>
      Text.OfMany(values);

    //
    // ToTextValues
    //

    public static IEnumerable<Text> ToTextValues(this IEnumerable<string> values, Func<string, Text> selectText) =>
      values.Select(selectText);

    public static IEnumerable<Text> ToTextValues(this IEnumerable<string> values, Func<string, int, Text> selectText) =>
      values.Select(selectText);

    public static IEnumerable<Text> ToTextValues(this IEnumerable<string> values) =>
      values.Select(value => Text.Of(value));

    public static IEnumerable<Text> ToTextValues<T>(this IEnumerable<T> values, Func<T, Text> selectText) =>
      values.Select(selectText);

    public static IEnumerable<Text> ToTextValues<T>(this IEnumerable<T> values, Func<T, int, Text> selectText) =>
      values.Select(selectText);

    public static IEnumerable<Text> ToTextValues<T>(this IEnumerable<T> values) =>
      values.Select(value => Text.Of(value));

    //
    // ToTextSeparatedBy
    //

    public static Text ToTextSeparatedBy(this IEnumerable<Text> values, Text separator, Func<Text, Text> selectText) =>
      Text.SeparatedBy(separator, values, selectText);

    public static Text ToTextSeparatedBy(this IEnumerable<Text> values, Text separator, Func<Text, int, Text> selectText) =>
      Text.SeparatedBy(separator, values, selectText);

    public static Text ToTextSeparatedBy(this IEnumerable<Text> values, Text separator) =>
      Text.SeparatedBy(separator, values);

    public static Text ToTextSeparatedBy<T>(this IEnumerable<T> values, Text separator, Func<T, Text> selectText) =>
      Text.SeparatedBy(separator, values, selectText);

    public static Text ToTextSeparatedBy<T>(this IEnumerable<T> values, Text separator, Func<T, int, Text> selectText) =>
      Text.SeparatedBy(separator, values, selectText);

    public static Text ToTextSeparatedBy<T>(this IEnumerable<T> values, Text separator) =>
      Text.SeparatedBy(separator, values);

    //
    // Conditions
    //

    public static Text WriteIf(this Text text, bool condition, Text whenTrue, Text whenFalse) =>
      text + Text.If(condition, whenTrue, whenFalse);

    public static Text WriteIf(this Text text, bool condition, Text whenTrue) =>
      text + Text.If(condition, whenTrue);

    public static Text WriteIf(this Text text, Func<bool> condition, Text whenTrue, Text whenFalse) =>
      text + Text.If(condition, whenTrue, whenFalse);

    public static Text WriteIf(this Text text, Func<bool> condition, Text whenTrue) =>
      text + Text.If(condition, whenTrue);

    //
    // Counts
    //

    public static Text WritePluralized(this Text text, int count, Text singular, Text plural = null) =>
      text + Text.Pluralized(count, singular, plural);

    public static Text WritePluralized(this Text text, long count, Text singular, Text plural = null) =>
      text + Text.Pluralized(count, singular, plural);

    public static Text WriteCount(this Text text, int count, Text singular, Text plural = null) =>
      text + Text.Count(count, singular, plural);

    public static Text WriteCount(this Text text, long count, Text singular, Text plural = null) =>
      text + Text.Count(count, singular, plural);

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
      string indent = Text.TwoSpaceIndent,
      bool retainIndent = false,
      bool retainWhitespace = false,
      string linePrefix = "") =>
      Text.Of(writer =>
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

    public static Text IndentWithTabs(this Text text, int level = 1, bool retainIndent = true, bool retainWhitespace = true) =>
      text.Indent(level, "\t", retainIndent, retainWhitespace);

    public static Text IndentWithSpaces(this Text text, int level = 1, int tabSize = 2, bool retainIndent = true, bool retainWhitespace = true) =>
      text.Indent(level, new string(' ', level * tabSize), retainIndent, retainWhitespace);

    public static Text Scoped(
      this Text text,
      string startToken = "{",
      string endToken = "}",
      int currentIndent = 0,
      string indent = Text.TwoSpaceIndent,
      bool retainIndent = true,
      bool retainWhitespace = true,
      string linePrefix = "") =>
      Text.None
        .WriteLine(startToken)
        .WriteIndented(text, currentIndent + 1, indent, retainIndent, retainWhitespace, linePrefix)
        .WriteLine()
        .Write(endToken);

    public static Text WriteIndented(
      this Text text,
      Text indentedText,
      int level = 1,
      string indent = Text.TwoSpaceIndent,
      bool retainIndent = false,
      bool retainWhitespace = false,
      string linePrefix = "") =>
      text + indentedText.Indent(level, indent, retainIndent, retainWhitespace, linePrefix);

    public static Text WriteIndentedWithTabs(this Text text, Text indentedText, int level = 1, bool retainIndent = true, bool retainWhitespace = true) =>
      text + indentedText.IndentWithTabs(level, retainIndent, retainWhitespace);

    public static Text WriteIndentedWithSpaces(this Text text, Text indentedText, int level = 1, int tabSize = 2, bool retainIndent = true, bool retainWhitespace = true) =>
      text + indentedText.IndentWithSpaces(level, tabSize, retainIndent, retainWhitespace);

    public static Text WriteTabs(this Text text, int count) =>
      text + new string('\t', count);

    public static Text WriteSpaces(this Text text, int count) =>
      text + new string(' ', count);

    public static Text WriteSpaceTabs(this Text text, int count, int size = 2) =>
      text + new string(' ', count * size);

    public static Text WriteScoped(
      this Text text,
      Text body,
      string startToken = "{",
      string endToken = "}",
      int currentIndent = 0,
      string indent = Text.TwoSpaceIndent,
      bool retainIndent = true,
      bool retainWhitespace = true,
      string linePrefix = "") =>
      text + body.Scoped(startToken, endToken, currentIndent, indent, retainIndent, retainWhitespace, linePrefix);

    //
    // Split
    //

    public static IEnumerable<string> Split(this Text text, Text separator, int maximumSubstrings = int.MaxValue, bool includeEmptyValues = false) =>
      text.ToString().Split(
        new string[] { separator },
        maximumSubstrings,
        includeEmptyValues ? StringSplitOptions.None : StringSplitOptions.RemoveEmptyEntries);

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

    public static IEnumerable<string> SplitSpaces(this Text text, int maximumSubstrings = int.MaxValue, bool includeEmptyValues = false) =>
      text.Split(' ', maximumSubstrings, includeEmptyValues);

    public static IEnumerable<string> SplitCommas(this Text text, int maximumSubstrings = int.MaxValue, bool includeEmptyValues = false) =>
      text.Split(',', maximumSubstrings, includeEmptyValues);

    //
    // Compact
    //

    public static Text Compact(this Text text, string ellipsis = Text.Ellipsis) =>
      Text.Of(() =>
      {
        var source = text.ToString();

        if(source.Length < 3)
        {
          return source;
        }

        return source.ToText().Compact(source.Length / 2, ellipsis);
      });

    public static Text Compact(this Text text, int maxLength, string ellipsis = Text.Ellipsis) =>
      Text.Of(() =>
      {
        var source = text.ToString();

        if(source.Length <= maxLength)
        {
          return source;
        }

        var partLength = maxLength / 2;

        var leftPart = source.Substring(0, partLength);
        var rightPart = source.Substring(source.Length - partLength + 1);

        return leftPart + ellipsis + rightPart;
      });

    public static Text CompactLeft(this Text text, int maxLength, string ellipsis = Text.Ellipsis) =>
      Text.Of(() =>
      {
        var source = text.ToString();

        return source.Length <= maxLength
          ? source
          : ellipsis + source.Substring(source.Length - maxLength + 1);
      });

    public static Text CompactRight(this Text text, int maxLength, string ellipsis = Text.Ellipsis) =>
      Text.Of(() =>
      {
        var source = text.ToString();

        return source.Length <= maxLength
          ? source
          : source.Substring(0, maxLength) + ellipsis;
      });

    public static Text WriteCompacted(this Text text, Text value, string ellipsis = Text.Ellipsis) =>
      text + value.Compact(ellipsis);

    public static Text WriteCompacted(this Text text, Text value, int maxLength, string ellipsis = Text.Ellipsis) =>
      text + value.Compact(maxLength, ellipsis);

    public static Text WriteCompactedLeft(this Text text, Text value, int maxLength, string ellipsis = Text.Ellipsis) =>
      text + value.CompactLeft(maxLength, ellipsis);

    public static Text WriteCompactedRight(this Text text, Text value, int maxLength, string ellipsis = Text.Ellipsis) =>
      text + value.CompactRight(maxLength, ellipsis);

    //
    // Comparisons
    //

    public static bool Equals(this Text text, string other) =>
      other != null && text.ToString().Equals(other);

    public static bool Equals(this Text text, string other, StringComparison comparisonType) =>
      other != null && text.ToString().Equals(other, comparisonType);

    public static int Compare(this Text text, string other) =>
      text.ToString().CompareTo(other);

    public static int Compare(this Text text, string other, StringComparison comparisonType) =>
      string.Compare(text.ToString(), other, comparisonType);

    public static int Compare(this Text text, string other, bool ignoreCase) =>
      string.Compare(text.ToString(), other, ignoreCase);

    public static int Compare(this Text text, string other, bool ignoreCase, CultureInfo culture) =>
      string.Compare(text.ToString(), other, ignoreCase, culture);

    public static int Compare(this Text text, string other, CultureInfo culture, CompareOptions options) =>
      string.Compare(text.ToString(), other, culture, options);

    public static int Compare(this Text text, int index, string other, int otherIndex, int length) =>
      string.Compare(text.ToString(), index, other, otherIndex, length);

    public static int Compare(this Text text, int index, string other, int otherIndex, int length, StringComparison comparisonType) =>
      string.Compare(text.ToString(), index, other, otherIndex, length, comparisonType);

    public static int Compare(this Text text, int index, string other, int otherIndex, int length, bool ignoreCase) =>
      string.Compare(text.ToString(), index, other, otherIndex, length, ignoreCase);

    public static int Compare(this Text text, int index, string other, int otherIndex, int length, CultureInfo culture, CompareOptions options) =>
      string.Compare(text.ToString(), index, other, otherIndex, length, culture, options);

    public static int Compare(this Text text, int index, string other, int otherIndex, int length, bool ignoreCase, CultureInfo culture) =>
      string.Compare(text.ToString(), index, other, otherIndex, length, ignoreCase, culture);

    public static int CompareOrdinal(this Text text, string other) =>
      string.CompareOrdinal(text.ToString(), other);

    public static int CompareOrdinal(this Text text, int index, string other, int otherIndex, int length) =>
      string.CompareOrdinal(text.ToString(), index, other, otherIndex, length);
  }
}