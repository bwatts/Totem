using System.Diagnostics.CodeAnalysis;

namespace Totem.Core;

public class QueryResult
{
    QueryResult(Type declaredType, bool isReport, bool isSingleRow, bool isManyRows, Type? rowType)
    {
        DeclaredType = declaredType;
        IsReport = isReport;
        IsSingleRow = isSingleRow;
        IsManyRows = isManyRows;
        RowType = rowType;
    }

    public Type DeclaredType { get; }
    public bool IsReport { get; }
    public bool IsSingleRow { get; }
    public bool IsManyRows { get; }
    public Type? RowType { get; }

    public static bool TryFrom(Type type, [NotNullWhen(true)] out QueryResult? result)
    {
        if(type is null)
            throw new ArgumentNullException(nameof(type));

        if(typeof(IReportRow).IsAssignableFrom(type))
        {
            result = new(type, isReport: true, isSingleRow: true, isManyRows: false, rowType: type);
            return true;
        }

        var rowType = type.GetImplementedInterfaceGenericArguments(typeof(IEnumerable<>)).SingleOrDefault();

        if(rowType is not null)
        {
            var isReport = typeof(IReportRow).IsAssignableFrom(rowType);

            result = new(type, isReport, isSingleRow: false, isManyRows: true, rowType);
            return true;
        }

        result = new(type, isReport: false, isSingleRow: false, isManyRows: false, rowType: null);
        return true;
    }

    public static QueryResult From(Type type)
    {
        if(!TryFrom(type, out var result))
            throw new ArgumentException($"Expected type to implement {typeof(IReportRow)} or {typeof(IEnumerable<IReportRow>)}: {type}", nameof(type));

        return result;
    }
}
