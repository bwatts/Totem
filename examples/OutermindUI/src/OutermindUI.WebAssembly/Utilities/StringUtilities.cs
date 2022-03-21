namespace OutermindUI.Utilities;

public static class StringUtilities
{
    public static string Classes(params string?[] classes)
    {
        if(classes is null)
            throw new ArgumentNullException(nameof(classes));

        return string.Join(" ", classes.Where(x => !string.IsNullOrWhiteSpace(x)));
    }

    public static string Styles(params string?[] styles)
    {
        if(styles is null)
            throw new ArgumentNullException(nameof(styles));

        return string.Join(";", styles.Where(x => !string.IsNullOrWhiteSpace(x)));
    }
}
