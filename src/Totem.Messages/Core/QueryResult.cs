using System.Diagnostics.CodeAnalysis;

namespace Totem.Core;

public class QueryResult
{
    QueryResult(Type declaredType, Type? reportRowType, bool isSingleRow, bool isManyRows)
    {
        DeclaredType = declaredType;
        ReportRowType = reportRowType;
        IsSingleRow = isSingleRow;
        IsManyRows = isManyRows;
    }

    public Type DeclaredType { get; }
    public Type? ReportRowType { get; }
    public bool IsSingleRow { get; }
    public bool IsManyRows { get; }

    public static bool TryFrom(Type type, [NotNullWhen(true)] out QueryResult? result)
    {
        if(type is null)
            throw new ArgumentNullException(nameof(type));

        if(typeof(IReportRow).IsAssignableFrom(type))
        {
            result = new(type, type, isSingleRow: true, isManyRows: false);
            return true;
        }

        var rowType = type.GetImplementedInterfaceGenericArguments(typeof(IEnumerable<>)).SingleOrDefault();

        if(rowType is not null && typeof(IReportRow).IsAssignableFrom(rowType))
        {
            result = new(type, rowType, isSingleRow: false, isManyRows: true);
            return true;
        }

        result = new(type, reportRowType: null, isSingleRow: false, isManyRows: false);
        return true;
    }

    public static QueryResult From(Type type)
    {
        if(!TryFrom(type, out var result))
            throw new ArgumentException($"Expected a type assignable to {typeof(IReportRow)} or {typeof(IEnumerable<IReportRow>)}: {type}", nameof(type));

        return result;
    }
}
