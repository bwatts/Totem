namespace Totem;

public static class LoggerExtensions
{
    public static void LogErrorInfo(this ILogger logger, ErrorInfo error)
    {
        if(logger is null)
            throw new ArgumentNullException(nameof(logger));

        if(error is null)
            throw new ArgumentNullException(nameof(error));

        logger.LogError("{Code} {Name} - {Level}", error.Code, error.Name, error.Level);
    }

    public static void LogErrorInfo(this ILogger logger, IEnumerable<ErrorInfo> errors)
    {
        if(logger is null)
            throw new ArgumentNullException(nameof(logger));

        if(errors is null)
            throw new ArgumentNullException(nameof(errors));

        var list = errors.ToList();

        if(list.Count == 0)
        {
            return;
        }
        else if(list.Count == 1)
        {
            logger.LogErrorInfo(list[0]);
        }
        else
        {
            logger.LogError("{Errors}", errors.Select(error => $"{error.Code} {error.Name} - {error.Level}"));
        }
    }

    public static void LogErrorInfo(this ILogger logger, params ErrorInfo[] errors) =>
        logger.LogErrorInfo(errors.AsEnumerable());
}
