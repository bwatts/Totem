using System;
using Spectre.Console;

namespace Dream
{
    public static class AnsiConsoleExtensions
    {
        public static IAnsiConsole WriteFiglet(this IAnsiConsole console, FigletText text)
        {
            if(console == null)
                throw new ArgumentNullException(nameof(console));

            if(text == null)
                throw new ArgumentNullException(nameof(text));

            console.Write(text);

            return console;
        }

        public static IAnsiConsole WriteFiglet(this IAnsiConsole console, string text, Color? color = null, Justify? alignment = null) =>
            console.WriteFiglet(new FigletText(text).Color(color).Alignment(alignment));

        public static IAnsiConsole WriteRule(this IAnsiConsole console, Rule rule)
        {
            if(console == null)
                throw new ArgumentNullException(nameof(console));

            if(rule == null)
                throw new ArgumentNullException(nameof(rule));

            console.Write(rule);

            return console;
        }

        public static IAnsiConsole WriteRule(this IAnsiConsole console, string? title = null, Style? style = null, Justify? alignment = null, BoxBorder? border = null)
        {
            var rule = title != null ? new Rule(title) : new Rule();
            rule.Style = style;
            rule.Alignment = alignment;

            if(border != null)
            {
                rule.Border = border;
            }

            return console.WriteRule(rule);
        }
    }
}