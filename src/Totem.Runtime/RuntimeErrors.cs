namespace Totem;

public static class RuntimeErrors
{
    public static readonly ErrorInfo CommandNotHandled = new(nameof(CommandNotHandled), ErrorLevel.NotFound);
    public static readonly ErrorInfo QueryNotHandled = new(nameof(QueryNotHandled));
    public static readonly ErrorInfo QueryHandlerNotFound = new(nameof(QueryHandlerNotFound));


    public static readonly ErrorInfo EventNotHandled = new(nameof(EventNotHandled), ErrorLevel.NotFound);
    public static readonly ErrorInfo ReportNotFound = new(nameof(ReportNotFound), ErrorLevel.NotFound);
    public static readonly ErrorInfo WorkflowNotFound = new(nameof(WorkflowNotFound), ErrorLevel.NotFound);
}
