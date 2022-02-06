using Microsoft.AspNetCore.Components.Web;

namespace DreamUI;

internal static class MouseButtons
{
    public const int Primary = 0;
    public const int Middle = 1;
    public const int Secondary = 2;

    internal static bool ButtonIsPrimary(this MouseEventArgs args) =>
        args.Button == Primary;

    internal static bool ButtonIsMiddle(this MouseEventArgs args) =>
        args.Button == Middle;

    internal static bool ButtonIsSecondary(this MouseEventArgs args) =>
        args.Button == Secondary;
}
